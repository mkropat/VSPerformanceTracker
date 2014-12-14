using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSPerformanceTracker.OSInterface;

namespace VSPerformanceTracker.VSInterface
{
    public sealed class DebuggerListener : IDisposable
    {
        private readonly uint _eventsSinkCookie;
        private readonly IVsDebugger _debuggerService;
        private readonly ITimeService _timeSerivce;
        private readonly Subject<DebuggerEvent> _debuggerEvents = new Subject<DebuggerEvent>();

        public DebuggerListener(IVsDebugger debuggerService, ITimeService timeService)
        {
            _debuggerService = debuggerService;
            _timeSerivce = timeService;

            ErrorHandler.ThrowOnFailure(_debuggerService.AdviseDebuggerEvents(new DebuggerEventsSink { Listener = this }, out _eventsSinkCookie));
        }

        public IObservable<DebuggerEvent> Events
        {
            get { return _debuggerEvents.AsObservable(); }
        }

        class DebuggerEventsSink : IVsDebuggerEvents
        {
            public DebuggerListener Listener { get; set; }

            public int OnModeChange(DBGMODE dbgmodeNew)
            {
                switch (dbgmodeNew)
                {
                    case DBGMODE.DBGMODE_Run:
                        ReportAction(DebuggerAction.Start);
                        break;

                    case DBGMODE.DBGMODE_Design:
                        ReportAction(DebuggerAction.Stop);
                        break;
                }

                return VSConstants.S_OK;
            }

            private void ReportAction(DebuggerAction action)
            {
                Listener._debuggerEvents.OnNext(new DebuggerEvent
                {
                    Action = action,
                    Time = Listener._timeSerivce.GetCurrent(),
                });
            }
        }

        public void Dispose()
        {
            ErrorHandler.CallWithCOMConvention(() => _debuggerService.UnadviseDebugEventCallback(_eventsSinkCookie));
            _debuggerEvents.Dispose();
        }

        // Thanks to Thomas LEBRUN for his example code [1].
        //
        // [1] http://blog.thomaslebrun.net/2014/07/vsx-how-to-be-notified-of-the-debug-events/
    }
}
