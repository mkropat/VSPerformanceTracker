using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker
{
    public class SolutionLoadToPerformanceEventTransformer
    {
        const string EventType = "solution-load";

        private readonly ISolutionInfoQueryer _solutionQueryer;

        public SolutionLoadToPerformanceEventTransformer(ISolutionInfoQueryer solutionQueryer)
        {
            _solutionQueryer = solutionQueryer;
        }

        public PerformanceEvent Transform(SolutionLoadResult result)
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