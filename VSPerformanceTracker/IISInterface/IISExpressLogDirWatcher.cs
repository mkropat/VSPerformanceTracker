using System;
using System.Reactive.Linq;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public class IISExpressLogDirWatcher : IDisposable
    {
        private IISExpressLogFileParserFactory _factory;
        private FileUpdateWatcher _watcher;

        public IISExpressLogDirWatcher(IISExpressLogFileParserFactory factory, FileUpdateWatcher watcher)
        {
            _factory = factory;
            _watcher = watcher;
        }

        public IObservable<IISLogEvent> Watch()
        {
            _factory.InitializeSkipOffsets();
            _watcher.Watch();

            return from updatedFile in _watcher.FilesChanged
                   let parser = _factory.GetParser(updatedFile)
                   from evt in parser.ReadEvents()
                   select evt;
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
