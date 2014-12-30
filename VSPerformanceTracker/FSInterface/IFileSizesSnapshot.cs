using System.Collections.Generic;
using System.Linq;

namespace VSPerformanceTracker.FSInterface
{
    public interface IFileSizesSnapshot
    {
        long? GetSize(string path);
        IEnumerable<string> ListFilenames();
    }

    public static class IFileSizesSnapshotExtensions
    {
        public static IEnumerable<string> GetChangedFiles(this IFileSizesSnapshot before, IFileSizesSnapshot after)
        {
            return GetNewAndRemovedFiles(before, after).Union(GetFilesWithUpdatedSize(before, after));
        }

        private static IEnumerable<string> GetNewAndRemovedFiles(IFileSizesSnapshot before, IFileSizesSnapshot after)
        {
            var beforeFilenames = before == null ? Enumerable.Empty<string>() : before.ListFilenames();
            var afterFilenames = after == null ? Enumerable.Empty<string>() : after.ListFilenames();

            var set = new HashSet<string>(beforeFilenames);
            set.SymmetricExceptWith(afterFilenames);
            return set;
        }

        private static IEnumerable<string> GetFilesWithUpdatedSize(IFileSizesSnapshot before, IFileSizesSnapshot after)
        {
            if (before == null || after == null)
                return Enumerable.Empty<string>();

            return from filename in before.ListFilenames()
                   let beforeSize = before.GetSize(filename)
                   let afterSize = after.GetSize(filename)
                   where afterSize != null && afterSize != beforeSize
                   select filename;
        }
    }
}
