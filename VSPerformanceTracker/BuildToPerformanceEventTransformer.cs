using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker
{
    public class BuildToPerformanceEventTransformer
    {
        const string EventType = "build";

        private readonly ISolutionInfoQueryer _solutionQueryer;

        public BuildToPerformanceEventTransformer(ISolutionInfoQueryer solutionQueryer)
        {
            _solutionQueryer = solutionQueryer;
        }

        public PerformanceEvent Transform(BuildResult result)
        {
            return new PerformanceEvent
            {
                Subject = _solutionQueryer.GetCurrent().File,
                EventType = EventType,
                Start = result.Start,
                Duration = result.Duration,
            };
        }
    }
}