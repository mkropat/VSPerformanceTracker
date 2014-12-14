using System;

namespace VSPerformanceTracker.Logging
{
    public class PerformanceEvent
    {
        public string Subject { get; set; }
        public string Branch { get; set; }
        public string EventType { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
