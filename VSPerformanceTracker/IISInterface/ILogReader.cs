using System.Collections.Generic;

namespace VSPerformanceTracker.IISInterface
{
    public interface ILogReader
    {
        IEnumerable<IISLogEvent> ReadEvents();
    }
}
