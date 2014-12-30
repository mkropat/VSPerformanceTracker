using System;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.Logging
{
    public static class PerformanceEventLogger
    {
        public static void Run(IObservable<PerformanceEvent> eventSource, IWritableFile logFile)
        {
            if (!logFile.Exists())
                WriteHeader(logFile);

            eventSource.Subscribe(evt => WriteEvent(evt, logFile));
        }

        private static void WriteHeader(IWritableFile logFile)
        {
            using (var writer = logFile.OpenWriter())
                CsvSerializer.SerializeHeader(typeof(LocalTimePerformanceEvent), writer);
        }

        private static void WriteEvent(PerformanceEvent evt, IWritableFile logFile)
        {
            var records = new[] { LocalTimePerformanceEvent.From(evt) };

            using (var writer = logFile.OpenWriter())
                CsvSerializer.SerializeRecords(records, writer);
        }
    }
}
