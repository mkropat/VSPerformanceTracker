using VSPerformanceTracker.Logging;
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