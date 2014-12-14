using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker.EventResults
{
    public class BuildToPerformanceEventTransformer : GenericPerformanceEventTransformer
    {
        protected override string EventType
        {
            get { return "build"; }
        }

        public BuildToPerformanceEventTransformer(ISolutionInfoQueryer solutionQueryer)
            : base(solutionQueryer)
        {
        }
    }
}