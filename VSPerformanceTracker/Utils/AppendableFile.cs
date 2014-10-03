using System.IO;

namespace VSPerformanceTracker.Utils
{
    public class AppendableFile : IOpenableFile
    {
        public string Path { get; private set; }

        public AppendableFile(string path)
        {
            Path = path;
        }

        public StreamWriter OpenWriter()
        {
            return File.AppendText(Path);
        }
    }
}