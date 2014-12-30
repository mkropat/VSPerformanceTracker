namespace VSPerformanceTracker.IISInterface
{
    public interface ILogListenerFactory
    {
        ILogListener Create(string dir);
    }
}
