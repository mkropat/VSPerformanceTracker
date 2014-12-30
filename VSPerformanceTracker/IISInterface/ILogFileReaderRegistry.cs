namespace VSPerformanceTracker.IISInterface
{
    public interface ILogFileReaderRegistry
    {
        ILogReader GetReader(string path);
        void InitializeSkipOffsets();
    }
}
