using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker.EventResults
{
    public class DebugStartedToPerformanceEventTransformer : GenericPerformanceEventTransformer
    {
        protected override string EventType
        {
            get { return "debug-started"; }
        }

        public DebugStartedToPerformanceEventTransformer(ISolutionInfoQueryer solutionQueryer)
            : base(solutionQueryer)
        {
        }
    }
}
