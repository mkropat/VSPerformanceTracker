namespace VSPerformanceTracker.FSInterface
{
    public interface IDirUpdateWatcherFactory
    {
        IDirUpdateWatcher Create(string dir);
    }
}
