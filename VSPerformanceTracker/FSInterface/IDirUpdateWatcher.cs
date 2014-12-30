using System;

namespace VSPerformanceTracker.FSInterface
{
    public interface IDirUpdateWatcher : IDisposable
    {
        IObservable<string> DirsChanged { get; }
        void StartWatching();
    }
}
