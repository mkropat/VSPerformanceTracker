using System;

namespace VSPerformanceTracker.OSInterface
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrent()
        {
            return DateTime.UtcNow;
        }
    }
}
