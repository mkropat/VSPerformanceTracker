using System;

namespace VSPerformanceTracker.EventResults
{
    public class GenericEventResult : IEventResult
    {
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
