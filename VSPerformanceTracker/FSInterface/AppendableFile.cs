using System.IO;

namespace VSPerformanceTracker.FSInterface
{
    public class AppendableFile : IWritableFile
    {
        private IPathQueryer _queryer;

        public AppendableFile(IPathQueryer queryer)
        {
            _queryer = queryer;
        }

        public StreamWriter OpenWriter()
        {
            return File.AppendText(_queryer.GetCurrent());
        }

        public bool Exists()
        {
            return File.Exists(_queryer.GetCurrent());
        }
    }
}