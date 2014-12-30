using System.Collections.Generic;
using System.IO;

namespace VSPerformanceTracker.FSInterface
{
    public class DirectoryEnumerator : VSPerformanceTracker.FSInterface.IPathEnumerator
    {
        public IEnumerable<string> Enumerate(string path)
        {
            return Directory.EnumerateDirectories(path);
        }
    }
}
