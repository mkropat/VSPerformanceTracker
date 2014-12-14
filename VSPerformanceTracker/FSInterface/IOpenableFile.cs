using System.IO;

namespace VSPerformanceTracker.FSInterface
{
    public interface IOpenableFile
    {
        StreamWriter OpenWriter();
    }
}