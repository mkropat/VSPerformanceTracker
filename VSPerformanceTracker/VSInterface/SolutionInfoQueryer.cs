using Microsoft.VisualStudio.Shell.Interop;

namespace VSPerformanceTracker.VSInterface
{
    public class SolutionInfoQueryer : ISolutionInfoQueryer
    {
        private readonly IVsSolution _solutionService;

        public SolutionInfoQueryer(IVsSolution solutionService)
        {
            _solutionService = solutionService;
        }

        public SolutionInfo GetCurrent()
        {
            return SolutionInfo.GetCurrent(_solutionService);
        }
    }
}