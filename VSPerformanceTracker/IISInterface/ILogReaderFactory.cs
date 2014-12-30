namespace VSPerformanceTracker.IISInterface
{
    public interface ILogReaderFactory
    {
        ILogReader Create(string logPath, long initialOffset);
    }
}
