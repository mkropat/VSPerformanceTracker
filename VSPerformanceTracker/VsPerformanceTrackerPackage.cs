using System;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSPerformanceTracker.EventResults;
using VSPerformanceTracker.FSInterface;
using VSPerformanceTracker.IISInterface;
using VSPerformanceTracker.Logging;
using VSPerformanceTracker.OSInterface;
using VSPerformanceTracker.UI;
using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker
{
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof (VsPerformanceTrackerOptionsPage), VsPerformanceTrackerPackage.PackageName, "General", 0, 0, supportsAutomation: true)]
    [Guid(GuidList.guidVSPackage1PkgString)]
    public sealed class VsPerformanceTrackerPackage : Package, IDisposable
    {
        public const string PackageName = "VSPerformanceTracker";

        private BuildListener _buildListener;
        private DebuggerListener _debuggerListener;
        private SolutionLoadListener _solutionLoadListener;

        protected override void Initialize()
        {
            base.Initialize();

            StartLogging(StartEventListeners());
        }

        private void StartLogging(IObservable<PerformanceEvent> events)
        {
            Func<VsPerformanceTrackerOptionsPage> getOptions = () => (VsPerformanceTrackerOptionsPage)GetDialogPage(typeof(VsPerformanceTrackerOptionsPage));
            var outputFile = new AppendableFile(new LogPathQueryer(getOptions));

            PerformanceEventLogger.Run(events, new CsvSerializer(outputFile));
        }

        private IObservable<PerformanceEvent> StartEventListeners()
        {
            var buildManager = (IVsSolutionBuildManager)GetService(typeof(SVsSolutionBuildManager));
            var buildManager5 = (IVsSolutionBuildManager5)GetService(typeof(SVsSolutionBuildManager));

            var debuggerService = (IVsDebugger)GetService(typeof(IVsDebugger));

            var dteService = (DTE)GetGlobalService(typeof(SDTE));

            var solutionService = (IVsSolution)GetService(typeof(SVsSolution));
            var solutionQueryer = new SolutionInfoQueryer(solutionService);

            var timeService = new TimeService();

            return Observable.Merge(
                ListenForSolutionLoadEvents(solutionService, solutionQueryer, timeService),
                ListenForBuildEvents(buildManager, buildManager5, solutionQueryer, timeService),
                ListenForDebugStartedEvents(debuggerService, dteService, solutionQueryer, timeService)
            );
        }

        private IObservable<PerformanceEvent> ListenForSolutionLoadEvents(IVsSolution solutionService, SolutionInfoQueryer solutionQueryer, ITimeService timeService)
        {
            _solutionLoadListener = new SolutionLoadListener(solutionService, timeService);
            var solutionLoadTransformer = new SolutionLoadToPerformanceEventTransformer(solutionQueryer);
            return _solutionLoadListener.LoadFinished.Select(solutionLoadTransformer.Transform);
        }

        private IObservable<PerformanceEvent> ListenForBuildEvents(IVsSolutionBuildManager buildManager, IVsSolutionBuildManager5 buildManager5, SolutionInfoQueryer solutionQueryer, ITimeService timeService)
        {
            _buildListener = new BuildListener(buildManager, buildManager5, timeService);
            var buildTransformer = new BuildToPerformanceEventTransformer(solutionQueryer);
            return _buildListener.LoadFinished.Select(buildTransformer.Transform);
        }

        private IObservable<PerformanceEvent> ListenForDebugStartedEvents(IVsDebugger debugger, DTE dteService, SolutionInfoQueryer solutionQueryer, ITimeService timeService)
        {
            var aggregator = new DebugStartAggregator(new BrowseToUrlQueryer(dteService), timeService);

            _debuggerListener = new DebuggerListener(debugger, timeService);
            var logWatcher = IISExpressLogWatcher.Watch(IISExpressSettings.LogPath, new DirectoryEnumerator(), new DirUpdateWatcherFactory(), new LogDirWatcherFactory());
            aggregator.Aggregate(_debuggerListener.Events, logWatcher);

            var transformer = new DebugStartedToPerformanceEventTransformer(solutionQueryer);
            return aggregator.DebugStarted.Select(transformer.Transform);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_buildListener != null)
                _buildListener.Dispose();

            if (_debuggerListener != null)
                _debuggerListener.Dispose();

            if (_solutionLoadListener != null)
                _solutionLoadListener.Dispose();
        }
    }

    public class LogPathQueryer : IPathQueryer
    {
        private readonly Func<VsPerformanceTrackerOptionsPage> _getOptions;

        public LogPathQueryer(Func<VsPerformanceTrackerOptionsPage> getOptions)
        {
            _getOptions = getOptions;
        }

        public string GetCurrent()
        {
            return _getOptions().LogPath;
        }
    }

    public class LogDirWatcherFactory : ILogListenerFactory
    {
        public ILogListener Create(string dir)
        {
            Func<IFileSizesSnapshot> takeSnapshot = () => FileSizesSnapshot.TakeSnapshot(dir);

            var readerRegistry = new IISExpressLogReaderRegistry(takeSnapshot, new LogReaderFactory());
            var fileWatcher = new PollingFileUpdateWatcher(takeSnapshot);

            return new IISExpressLogDirWatcher(readerRegistry, fileWatcher);
        }
    }

    public class LogReaderFactory : ILogReaderFactory
    {
        public ILogReader Create(string logPath, long initialOffset)
        {
            return new IISExpressLogFileReader(initialOffset, new TailReadingFile(logPath), new W3cLogParser());
        }
    }

    public class DirUpdateWatcherFactory : IDirUpdateWatcherFactory
    {
        public IDirUpdateWatcher Create(string dir)
        {
            return new ListeningDirUpdateWatcher(dir);
        }
    }
}
