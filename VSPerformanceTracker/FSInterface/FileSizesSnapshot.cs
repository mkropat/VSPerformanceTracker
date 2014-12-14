using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPerformanceTracker.FSInterface
{
    public class FileSizesSnapshot : IFileSizesSnapshot
    {
        private Dictionary<string, long> _sizes = new Dictionary<string, long>();
        private readonly string _dir;

        public static FileSizesSnapshot TakeSnapshot(string dir)
        {
            var snapshot = new FileSizesSnapshot(dir);
            snapshot.TakeSnapshot();
            return snapshot;
        }

        public FileSizesSnapshot(string dir)
        {
            _dir = dir;
        }

        public void TakeSnapshot()
        {
            // The Windows API returns cached file sizes whenever you're
            // dealing with an open file handle that's actively being written
            // to [1].  That's why we must call Refresh() every time to get
            // the latest size.
            //
            // [1] http://stackoverflow.com/a/8654645/27581

            _sizes = GetCurrentFileListing(_dir)
                .ToDictionary(fi => fi.FullName, fi => GetCurrentFileSize(fi));
        }

        private IEnumerable<FileInfo> GetCurrentFileListing(string dir)
        {
            var di = new DirectoryInfo(dir);
            di.Refresh();
            return di.EnumerateFiles();
        }

        private long GetCurrentFileSize(FileInfo file)
        {
            file.Refresh();
            return file.Length;
        }

        public long? GetSize(string path)
        {
            return _sizes.ContainsKey(path)
                ? (long?)_sizes[path]
                : null;
        }

        public IEnumerable<string> ListFilenames()
        {
            return _sizes.Keys;
        }
    }

}
