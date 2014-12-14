using System;

namespace VSPerformanceTracker.IISInterface
{
    public class IISLogEvent
    {
        public DateTime Time { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
    }
}
