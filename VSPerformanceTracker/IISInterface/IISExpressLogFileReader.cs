using System;
using System.Collections.Generic;
using System.Linq;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public class IISExpressLogFileReader : ILogReader
    {
        private readonly IReadableFile _logFile;
        private readonly ILogParser _logParser;

        public IISExpressLogFileReader(long toSkip, IReadableFile logFile, ILogParser logParser)
        {
            _logFile = logFile;
            _logParser = logParser;

            SkipTo(toSkip);
        }

        private void SkipTo(long offset)
        {
            using (var reader = _logFile.Open())
            {
                foreach (var line in reader.ReadLinesTill(offset))
                    _logParser.ParseLine(line);
            }
        }

        public IEnumerable<IISLogEvent> ReadEvents()
        {
            using (var reader = _logFile.Open())
            {
                var events =
                    from line in reader.ReadLines()
                    let entry = _logParser.ParseLine(line)
                    where entry != null
                    select ParseEntry(entry);
                return events.ToArray();
            }
        }

        private static IISLogEvent ParseEntry(Dictionary<string, string> entry)
        {
            var evt = new IISLogEvent();

            if (entry.ContainsKey("date") && entry.ContainsKey("time"))
            {
                DateTime parsed;
                if (DateTime.TryParse(entry["date"] + " " + entry["time"], out parsed))
                    evt.Time = parsed;
            }

            if (entry.ContainsKey("cs-method"))
                evt.Method = entry["cs-method"];

            if (entry.ContainsKey("cs-uri-stem"))
                evt.Path = entry["cs-uri-stem"];

            if (entry.ContainsKey("cs-uri-query"))
                evt.QueryString = entry["cs-uri-query"];

            return evt;
        }
    }
}
