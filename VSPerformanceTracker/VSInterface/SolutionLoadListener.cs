using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSPerformanceTracker.EventResults;

namespace VSPerformanceTracker.VSInterface
{
    public sealed class SolutionLoadListener : IDisposable
    {
        private readonly uint _loadSinkCookie;
        private readonly uint _eventSinkCookie;
        private readonly IVsSolution _solutionService;
        private readonly SolutionEventSink _eventSink;

        private readonly Subject<GenericEventResult> _resultSubject = new Subject<GenericEventResult>();

        public IObservable<IEventResult> LoadFinished
        {
            get { return _resultSubject.AsObservable(); }
        }

        public SolutionLoadListener(IVsSolution solutionService)
        {
            _solutionService = solutionService;

            _eventSink = new SolutionEventSink();
            ErrorHandler.ThrowOnFailure(_solutionService.AdviseSolutionEvents(_eventSink, out _eventSinkCookie));
            ErrorHandler.ThrowOnFailure(_solutionService.AdviseSolutionEvents(new SolutionLoadEventSink { Listener = this }, out _loadSinkCookie));
        }

        private void OnSolutionLoaded()
        {
            if (_eventSink.Start == null)
                return;

            var start = _eventSink.Start.Value;
            _eventSink.Reset();

            _resultSubject.OnNext(new GenericEventResult
            {
                Start = start,
                Duration = DateTime.UtcNow - start,
            });
        }

        public void Dispose()
        {
            ErrorHandler.CallWithCOMConvention(() => _solutionService.UnadviseSolutionEvents(_loadSinkCookie));
            ErrorHandler.CallWithCOMConvention(() => _solutionService.UnadviseSolutionEvents(_eventSinkCookie));

            _resultSubject.Dispose();
        }

        class SolutionEventSink : IVsSolutionEvents
        {
            public DateTime? Start { get; private set; }

            public void Reset()
            {
                Start = null;
            }

            public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
            {
                Start = DateTime.UtcNow;

                return VSConstants.S_OK;
            }

            public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeCloseSolution(object pUnkReserved)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterCloseSolution(object pUnkReserved)
            {
                Reset();

                return VSConstants.S_OK;
            }
        }

        class SolutionLoadEventSink : IVsSolutionEvents, IVsSolutionLoadEvents
        {
            public SolutionLoadListener Listener { private get; set; }

            public int OnBeforeOpenSolution(string pszSolutionFilename)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeBackgroundSolutionLoadBegins()
            {
                return VSConstants.S_OK;
            }

            public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
            {
                pfShouldDelayLoadToNextIdle = false;
                return VSConstants.S_OK;
            }

            public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterBackgroundSolutionLoadComplete()
            {
                Listener.OnSolutionLoaded();

                return VSConstants.S_OK;
            }

            public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeCloseSolution(object pUnkReserved)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterCloseSolution(object pUnkReserved)
            {
                return VSConstants.S_OK;
            }
        }
    }
}