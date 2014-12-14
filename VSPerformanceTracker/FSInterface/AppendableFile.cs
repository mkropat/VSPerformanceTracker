using System.IO;

namespace VSPerformanceTracker.FSInterface
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

        public bool Exists()
        {
            return File.Exists(Path);
        }
    }
}