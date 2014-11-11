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
            var solution = _solutionQueryer.GetCurrent();

            return new PerformanceEvent
            {
                Subject = solution.File,
                Branch = GitInterface.GetBranchName(solution.Directory),
                EventType = EventType,
                Start = result.Start,
                Duration = result.Duration,
            };
        }
    }
}