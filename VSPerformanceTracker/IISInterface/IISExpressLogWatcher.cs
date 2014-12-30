using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace VSPerformanceTracker.IISInterface
{
    public static class IISExpressLogWatcher
    {
        public static IObservable<IISLogEvent> Watch(ILogListenerFactory dirWatcherFactory)
        {
            var watchers =
                from dir in Directory.EnumerateDirectories(IISExpressSettings.LogPath)
                let dirWatcher = dirWatcherFactory.Create(dir)
                select dirWatcher.ListenForEvents();

            return Observable.Merge(watchers.ToArray());
        }
    }
}
