namespace VSPerformanceTracker.FSInterface
{
    public interface IReadableFile
    {
        ILineReader Open();
    }
}
