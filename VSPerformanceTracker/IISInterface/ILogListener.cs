using System;

namespace VSPerformanceTracker.IISInterface
{
    public interface ILogListener : IDisposable
    {
        IObservable<IISLogEvent> ListenForEvents();
    }
}
