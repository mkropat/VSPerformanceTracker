using System;

namespace VSPerformanceTracker.VSInterface
{
    public class BuildResult
    {
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
    }
}