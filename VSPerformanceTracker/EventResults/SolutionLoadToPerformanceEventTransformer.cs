using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker.EventResults
{
    public class SolutionLoadToPerformanceEventTransformer : GenericPerformanceEventTransformer
    {
        protected override string EventType
        {
            get { return "solution-load"; }
        }

        public SolutionLoadToPerformanceEventTransformer(ISolutionInfoQueryer solutionQueryer)
            : base(solutionQueryer)
        {
        }
    }
}