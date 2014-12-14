using System;

namespace VSPerformanceTracker.VSInterface
{
    public class DebuggerEvent
    {
        public DebuggerAction Action { get; set; }
        public DateTime Time { get; set; }
    }

    public enum DebuggerAction
    {
        Start,
        Stop
    }
}
