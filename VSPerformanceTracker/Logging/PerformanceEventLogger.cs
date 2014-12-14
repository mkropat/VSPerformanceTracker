using System;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.Logging
{
    public static class PerformanceEventLogger
    {
        public static void Run(IObservable<PerformanceEvent> eventSource, IOpenableFile logFile)
        {
            eventSource.Subscribe(evt => WriteEvent(evt, logFile));
        }

        private static void WriteEvent(PerformanceEvent evt, IOpenableFile logFile)
        {
            var line = new object[]
            {
                evt.Subject,
                evt.Branch,
                evt.EventType,
                evt.Start.ToLocalTime(),
                evt.Duration,
            };

            using (var writer = logFile.OpenWriter())
                writer.WriteLine(string.Join(",", line));
        }
    }
}
