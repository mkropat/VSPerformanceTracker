using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSPerformanceTracker.VSInterface
{
    public class SolutionInfo
    {
        public static SolutionInfo GetCurrent(IVsSolution solutionService)
        {
            string dir, file, optionsFile;
            ErrorHandler.ThrowOnFailure(solutionService.GetSolutionInfo(out dir, out file, out optionsFile));

            return new SolutionInfo
            {
                Directory = dir,
                File = file,
                OptionsFile = optionsFile,
            };
        }

        public string Directory { get; private set; }
        public string File { get; private set; }
        public string OptionsFile { get; private set; }
    }
}