using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace VSPerformanceTracker.FSInterface
{
    public sealed class ListeningDirUpdateWatcher : IDirUpdateWatcher
    {
        private readonly string _dir;
        private FileSystemWatcher _watcher;
        private readonly Subject<string> _dirChanged = new Subject<string>();

        public ListeningDirUpdateWatcher(string dir)
        {
            _dir = dir;
        }

        public void StartWatching()
        {
            if (_watcher != null)
                return;

            _watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                Path = _dir,
            };
            _watcher.Changed += DirChanged;

            _watcher.EnableRaisingEvents = true; // start watching
        }

        private void DirChanged(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
                _dirChanged.OnNext(e.FullPath);
        }

        public IObservable<string> DirsChanged
        {
            get { return _dirChanged.AsObservable(); }
        }

        public void Dispose()
        {
            _watcher.Dispose();
            _dirChanged.Dispose();
        }
    }
}
