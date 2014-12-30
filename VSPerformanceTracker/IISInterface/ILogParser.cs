using System.Collections.Generic;

namespace VSPerformanceTracker.IISInterface
{
    public interface ILogParser
    {
        Dictionary<string, string> ParseLine(string line);
    }
}
