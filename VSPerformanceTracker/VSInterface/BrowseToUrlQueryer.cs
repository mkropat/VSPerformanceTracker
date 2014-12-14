using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace VSPerformanceTracker.VSInterface
{
    public class BrowseToUrlQueryer
    {
        private DTE _dte;

        public BrowseToUrlQueryer(DTE dte)
        {
            _dte = dte;
        }

        public string GetCurrent()
        {
            var url = GetBrowseToUrl() ?? "/";
            if (url.StartsWith("~/"))
                url = url.TrimPrefix("~");
            return url;
        }

        private string GetBrowseToUrl()
        {
            var item = _dte.ActiveDocument.ProjectItem;
            var browseToUrlProperty = item.Properties
                .Cast<Property>()
                .FirstOrDefault(p => p.Name.EndsWith("BrowseToURL", StringComparison.InvariantCulture));
            return browseToUrlProperty == null
                ? null
                : (string)browseToUrlProperty.Value;
        }
    }
}
