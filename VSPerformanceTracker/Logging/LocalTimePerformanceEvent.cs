using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSPerformanceTracker.Logging
{
    public class LocalTimePerformanceEvent
    {
        public static LocalTimePerformanceEvent From(PerformanceEvent evt)
        {
            return new LocalTimePerformanceEvent
            {
                Subject = evt.Subject,
                Branch = evt.Branch,
                EventType = evt.EventType,
                Start = evt.Start.ToLocalTime(),
                Duration = evt.Duration,
            };
        }

        [CsvSerializer.ColumnOrder(0)]
        public string Subject { get; set; }

        [CsvSerializer.ColumnOrder(1)]
        public string Branch { get; set; }

        [CsvSerializer.ColumnOrder(2)]
        public string EventType { get; set; }

        [CsvSerializer.ColumnOrder(3)]
        public DateTime Start { get; set; }

        [CsvSerializer.ColumnOrder(4)]
        public TimeSpan Duration { get; set; }
    }
}
