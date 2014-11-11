using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSPerformanceTracker
{
    public static class GitInterface
    {
        public static string GetBranchName(string dir)
        {
            try
            {
                using (var repo = new Repository(dir))
                {
                    return repo.Head.Name;
                };
            }
            catch (RepositoryNotFoundException)
            {
                return null;
            }
        }
    }
}
