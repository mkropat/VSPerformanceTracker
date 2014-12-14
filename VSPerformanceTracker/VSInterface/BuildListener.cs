using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSPerformanceTracker.EventResults;

namespace VSPerformanceTracker.VSInterface
{
    public sealed class BuildListener : IDisposable
    {
        private readonly IVsSolutionBuildManager _buildManager;
        private readonly IVsSolutionBuildManager5 _buildManager5;
        private readonly uint _sinkHandle;
        private readonly UpdateSolutionEventSink _eventSink;
        private readonly uint _sinkHandle2;

        private readonly Subject<GenericEventResult> _resultSubject = new Subject<GenericEventResult>();

        public IObservable<IEventResult> LoadFinished
        {
            get { return _resultSubject.AsObservable(); }
        }

        public BuildListener(IVsSolutionBuildManager buildManager, IVsSolutionBuildManager5 buildManager5)
        {
            _buildManager = buildManager;
            _eventSink = new UpdateSolutionEventSink { Listner = this };
            ErrorHandler.ThrowOnFailure(_buildManager.AdviseUpdateSolutionEvents(_eventSink, out _sinkHandle));

            _buildManager5 = buildManager5;
            var eventSink2 = new UpdateSolutionEventSink2();
            _buildManager5.AdviseUpdateSolutionEvents4(eventSink2, out _sinkHandle2);
        }

        private void OnBuildFinished(DateTime start)
        {
            _eventSink.Reset();

            _resultSubject.OnNext(new GenericEventResult
            {
                Start = start,
                Duration = DateTime.UtcNow - start,
            });
        }

        public void Dispose()
        {
            ErrorHandler.CallWithCOMConvention(() => _buildManager.UnadviseUpdateSolutionEvents(_sinkHandle));
            ErrorHandler.CallWithCOMConvention(() => _buildManager5.UnadviseUpdateSolutionEvents4(_sinkHandle2));

            _resultSubject.Dispose();
        }

        class UpdateSolutionEventSink2 : IVsUpdateSolutionEvents4
        {
            public void UpdateSolution_QueryDelayFirstUpdateAction(out int pfDelay)
            {
                pfDelay = 0;
            }

            public void UpdateSolution_BeginFirstUpdateAction()
            {
            }

            public void UpdateSolution_EndLastUpdateAction()
            {
            }

            public void UpdateSolution_BeginUpdateAction(uint dwAction)
            {
                var action = (VSSOLNBUILDUPDATEFLAGS) dwAction;
            }

            public void UpdateSolution_EndUpdateAction(uint dwAction)
            {
                var action = (VSSOLNBUILDUPDATEFLAGS) dwAction;
            }

            public void OnActiveProjectCfgChangeBatchBegin()
            {
            }

            public void OnActiveProjectCfgChangeBatchEnd()
            {
            }
        }

        class UpdateSolutionEventSink : IVsUpdateSolutionEvents
        {
            private DateTime? _start;
            public BuildListener Listner { private get; set; }

            public void Reset()
            {
                _start = null;
            }

            public int UpdateSolution_Begin(ref int pfCancelUpdate)
            {
                _start = DateTime.UtcNow;

                return VSConstants.S_OK;
            }

            public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
            {
                if (_start != null)
                {
                    var start = _start.Value;
                    Reset();
                    Listner.OnBuildFinished(start);
                }

                return VSConstants.S_OK;
            }

            public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
            {
                return VSConstants.S_OK;
            }

            public int UpdateSolution_Cancel()
            {
                Reset();

                return VSConstants.S_OK;
            }

            public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
            {
                return VSConstants.S_OK;
            }
        }
    }
}