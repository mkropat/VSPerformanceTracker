using System;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSPerformanceTracker.EventResults;
using VSPerformanceTracker.FSInterface;
using VSPerformanceTracker.Logging;
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
    [Guid(GuidList.guidVSPackage1PkgString)]
    public sealed class VsPerformanceTrackerPackage : Package, IDisposable
    {
        private BuildListener _buildListener;
        private SolutionLoadListener _solutionLoadListener;
        private ISolutionInfoQueryer _solutionQueryer;

        protected override void Initialize()
        {
            base.Initialize();

            var solutionService = (IVsSolution) GetService(typeof (SVsSolution));
            var buildManager = (IVsSolutionBuildManager) GetService(typeof (SVsSolutionBuildManager));
            var buildManager5 = (IVsSolutionBuildManager5) GetService(typeof (SVsSolutionBuildManager));

            _solutionQueryer = new SolutionInfoQueryer(solutionService);

            var events = Observable.Merge(new []
            {
                ListenForBuildEvents(buildManager, buildManager5),
                ListenForSolutionLoadEvents(solutionService),
            });

            var logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "build-log.csv");

            PerformanceEventLogger.Run(events, new AppendableFile(logFile));
        }

        private IObservable<PerformanceEvent> ListenForSolutionLoadEvents(IVsSolution solutionService)
        {
            _solutionLoadListener = new SolutionLoadListener(solutionService);
            var solutionLoadTransformer = new SolutionLoadToPerformanceEventTransformer(_solutionQueryer);
            return _solutionLoadListener.LoadFinished.Select(solutionLoadTransformer.Transform);
        }

        private IObservable<PerformanceEvent> ListenForBuildEvents(IVsSolutionBuildManager buildManager, IVsSolutionBuildManager5 buildManager5)
        {
            _buildListener = new BuildListener(buildManager, buildManager5);
            var buildTransformer = new BuildToPerformanceEventTransformer(_solutionQueryer);
            return _buildListener.LoadFinished.Select(buildTransformer.Transform);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _buildListener.Dispose();
            _solutionLoadListener.Dispose();
        }
    }
}
