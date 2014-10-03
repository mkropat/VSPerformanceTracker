using System;

namespace VSPerformanceTracker.VSInterface
{
    public class SolutionLoadResult
    {
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
    }
}