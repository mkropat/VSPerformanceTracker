namespace VSPerformanceTracker.Logging
{
    public interface ISerializer
    {
        void SerializeRecord<T>(T record);
    }
}
