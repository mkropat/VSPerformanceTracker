using System;
using VSPerformanceTracker.Utils;

namespace VSPerformanceTracker
{
    public class PerformanceEvent
    {
        public string Subject      { get; set; }
        public string EventType    { get; set; }
        public DateTime Start      { get; set; }
        public TimeSpan Duration   { get; set; }
    }

    public class PerformanceEventLogger
    {
        public static void Run(IObservable<PerformanceEvent> eventSource, IOpenableFile logFile)
        {
            var logger = new PerformanceEventLogger(logFile);
            eventSource.Subscribe(logger.OnEvent);
        }

        private readonly IOpenableFile _logFile;
        private PerformanceEventLogger(IOpenableFile logFile)
        {
            _logFile = logFile;
        }

        private void OnEvent(PerformanceEvent evt)
        {
            var line = new object[]
            {
                evt.Subject,
                evt.EventType,
                evt.Start.ToLocalTime(),
                evt.Duration,
            };

            using (var writer = _logFile.OpenWriter())
                writer.WriteLine(string.Join(",", line));
        }
    }
}