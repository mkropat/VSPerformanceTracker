using System;
using System.Collections.Generic;

namespace VSPerformanceTracker.FSInterface
{
    public interface ILineReader : IDisposable
    {
        IEnumerable<string> ReadLines();
        IEnumerable<string> ReadLinesTill(long toOffset);
    }
}
