using System;

namespace VSPerformanceTracker.OSInterface
{
    public interface ITimeService
    {
        DateTime GetCurrent();
    }
}
