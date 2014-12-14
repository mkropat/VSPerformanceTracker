using VSPerformanceTracker.Logging;
using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker.EventResults
{
    public abstract class GenericPerformanceEventTransformer
    {
        protected abstract string EventType { get; }

        private readonly ISolutionInfoQueryer _solutionQueryer;

        public GenericPerformanceEventTransformer(ISolutionInfoQueryer solutionQueryer)
        {
            _solutionQueryer = solutionQueryer;
        }

        public PerformanceEvent Transform(IEventResult result)
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
