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
            var result = solutionService.GetSolutionInfo(out dir, out file, out optionsFile);
            if (result != VSConstants.S_OK)
                throw new Exception("VS Error code: " + result);

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