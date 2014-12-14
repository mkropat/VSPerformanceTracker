using System;

namespace VSPerformanceTracker.EventResults
{
    public interface IEventResult
    {
        DateTime Start { get; }
        TimeSpan Duration { get; }
    }
}
