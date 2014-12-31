using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using VSPerformanceTracker.OSInterface;

namespace VSPerformanceTracker.UI
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("34c35d00-6b13-4368-bc34-a706ca2a0b73")]
    public class VsPerformanceTrackerOptionsPage : DialogPage
    {
        public const string DefaultLogFileName = "build-log.csv";

        public string DefaultLogPath
        {
            get
            {
                return Path.Combine(DefaultLogDirectory, DefaultLogFileName);
            }
        }

        public string DefaultLogDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        private const string logPathKey = "LogPath";
        private string _logPath;
        public string LogPath
        {
            get
            {
                //var configValue = _store.ReadSetting<string>(logPathKey);
                //return configValue == null
                //    ? DefaultLogPath
                //    : configValue;
                if (_logPath == null)
                    _logPath = DefaultLogPath;

                return _logPath;
            }
            set
            {
                _logPath = value;
                //_store.WriteSetting(logPathKey, value);
            }
        }

        protected override IWin32Window Window
        {
            get
            {
                var page = new GeneralOptionsControl
                {
                    OptionsPage = this
                };
                page.Initialize();
                return page;
            }
        }
    }
}
