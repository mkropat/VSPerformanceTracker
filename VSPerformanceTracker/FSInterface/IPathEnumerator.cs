using System.Collections.Generic;

namespace VSPerformanceTracker.FSInterface
{
    public interface IPathEnumerator
    {
        IEnumerable<string> Enumerate(string path);
    }
}
