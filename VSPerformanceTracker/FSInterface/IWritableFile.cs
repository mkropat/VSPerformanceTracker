using System.IO;

namespace VSPerformanceTracker.FSInterface
{
    public interface IWritableFile
    {
        StreamWriter OpenWriter();
        bool Exists();
    }
}