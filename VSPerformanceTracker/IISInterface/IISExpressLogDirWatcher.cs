using System;
using System.Reactive.Linq;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public sealed class IISExpressLogDirWatcher : ILogListener
    {
        private ILogFileReaderRegistry _readerRegistry;
        private IFileUpdateWatcher _watcher;

        public IISExpressLogDirWatcher(ILogFileReaderRegistry readerRegistry, IFileUpdateWatcher watcher)
        {
            _readerRegistry = readerRegistry;
            _watcher = watcher;
        }

        public IObservable<IISLogEvent> ListenForEvents()
        {
            _readerRegistry.InitializeSkipOffsets();
            _watcher.StartWatching();

            return from updatedFile in _watcher.FilesChanged
                   let parser = _readerRegistry.GetReader(updatedFile)
                   from evt in parser.ReadEvents()
                   select evt;
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
