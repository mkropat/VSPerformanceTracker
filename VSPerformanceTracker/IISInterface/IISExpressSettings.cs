using System;
using System.IO;
using System.Xml.Linq;

namespace VSPerformanceTracker.IISInterface
{
    public static class IISExpressSettings
    {
        public static string IISUserHome
        {
            get
            {
                // Should also support 'CustomUserHome' override at HKCU\Software\Microsoft\IISExpress
                // http://www.iis.net/learn/extensions/introduction-to-iis-express/iis-80-express-readme

                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IISExpress");
            }
        }

        public static string LogPath
        {
            get
            {
                var applicationHostConfigPath = Path.Combine(IISUserHome, "config", "applicationhost.config");

                using (var reader = new StreamReader(applicationHostConfigPath))
                {
                    var config = XDocument.Load(reader);
                    var defaultsLogFileElement = config
                        .Element("configuration")
                        .Element("system.applicationHost")
                        .Element("sites")
                        .Element("siteDefaults")
                        .Element("logFile");

                    var path = defaultsLogFileElement.Attribute("directory").Value;
                    path = path.Replace("%IIS_USER_HOME%", IISUserHome);

                    return path;
                }
            }
        }
    }
}
