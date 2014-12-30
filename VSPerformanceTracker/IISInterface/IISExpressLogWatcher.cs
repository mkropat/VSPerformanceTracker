using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public static class IISExpressLogWatcher
    {
        public static IObservable<IISLogEvent> Watch(string logPath, IPathEnumerator directoryEnumerator, IDirUpdateWatcherFactory dirWatcherFactory, ILogListenerFactory dirListenerFactory)
        {
            var registry = new HashSet<string>();

            var dirWatcher = dirWatcherFactory.Create(logPath);

            var eventsForNewDirs =
                from dir in dirWatcher.DirsChanged
                where registry.Add(dir)
                let listener = dirListenerFactory.Create(dir)
                from evt in listener.ListenForEvents()
                select evt;

            dirWatcher.StartWatching();

            var eventsForExisting = directoryEnumerator.Enumerate(logPath).ToObservable()
                .Where(dir => registry.Add(dir))
                .Select(dir => dirListenerFactory.Create(dir))
                .SelectMany(listener => listener.ListenForEvents());

            return Observable.Merge(eventsForExisting, eventsForNewDirs);
        }
    }
}
