using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace VSPerformanceTracker.FSInterface
{
    public sealed class FileUpdateWatcher : IFileUpdateWatcher
    {
        // FileSystemWatcher would be more efficient, unfortunately it doesn't do a good job of notifying on files that
        // are being actively held open and written to.  The only other alternative is to poll on the directory listing,
        // watching for file size changes, so that's what we do.

        private readonly Subject<string> _fileChanged = new Subject<string>();
        private readonly Func<IFileSizesSnapshot> _takeSnapshot;
        private CancellationTokenSource _cancelSource;
        private IFileSizesSnapshot _snapshot;

        const int _pollInterval = 1000;//ms

        public FileUpdateWatcher(Func<IFileSizesSnapshot> takeSnapshot)
        {
            _takeSnapshot = takeSnapshot;
        }

        public IObservable<string> FilesChanged
        {
            get { return _fileChanged.AsObservable(); }
        }

        public void StartWatching()
        {
            if (_cancelSource != null)
                return;

            _cancelSource = new CancellationTokenSource();

            StartWatcherThread();
        }

        private void StartWatcherThread()
        {
            var token = _cancelSource.Token;

            Action watcherLoop = async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    ReportOnChangedFiles();

                    await Task.Delay(_pollInterval);
                }
            };

            Task.Factory.StartNew(watcherLoop, token);
        }

        private void ReportOnChangedFiles()
        {
            var newSnapshot = _takeSnapshot();

            if (_snapshot != null)
                foreach (var filename in _snapshot.GetChangedFiles(newSnapshot))
                    _fileChanged.OnNext(filename);

            _snapshot = newSnapshot;
        }

        public void Dispose()
        {
            if (_cancelSource != null)
            {
                _cancelSource.Cancel();
                _cancelSource.Dispose();
            }

            _fileChanged.Dispose();
        }
    }
}
