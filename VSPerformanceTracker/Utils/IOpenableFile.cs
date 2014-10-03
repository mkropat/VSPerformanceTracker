using System.IO;

namespace VSPerformanceTracker.Utils
{
    public interface IOpenableFile
    {
        StreamWriter OpenWriter();
    }
}