using System;

namespace VSPerformanceTracker.FSInterface
{
    public interface IFileUpdateWatcher : IDisposable
    {
        IObservable<string> FilesChanged { get; }
        void StartWatching();
    }
}
