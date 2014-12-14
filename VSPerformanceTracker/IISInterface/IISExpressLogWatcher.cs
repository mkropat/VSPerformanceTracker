using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public static class IISExpressLogWatcher
    {
        public static IObservable<IISLogEvent> Watch()
        {
            var watchers =
                from dir in Directory.EnumerateDirectories(IISExpressSettings.LogPath)
                let factory = new IISExpressLogFileParserFactory(new FileSizesSnapshot(dir))
                let fileWatcher = new FileUpdateWatcher(() => FileSizesSnapshot.TakeSnapshot(dir))
                let logDirWatcher = new IISExpressLogDirWatcher(factory, fileWatcher)
                select logDirWatcher.Watch();

            return Observable.Merge(watchers.ToArray());
        }
    }
}
