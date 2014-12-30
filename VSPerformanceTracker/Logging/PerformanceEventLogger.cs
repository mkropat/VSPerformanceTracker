using System;

namespace VSPerformanceTracker.Logging
{
    public static class PerformanceEventLogger
    {
        public static void Run(IObservable<PerformanceEvent> eventSource, ISerializer serializer)
        {
            eventSource.Subscribe(evt =>
            {
                var record = LocalTimePerformanceEvent.From(evt);

                serializer.SerializeRecord(record);
            });
        }
    }
}
