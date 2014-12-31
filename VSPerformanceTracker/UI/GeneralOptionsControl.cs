using System;
using System.IO;
using System.Windows.Forms;

namespace VSPerformanceTracker.UI
{
    public partial class GeneralOptionsControl : UserControl
    {
        internal VsPerformanceTrackerOptionsPage OptionsPage { get; set; }

        public GeneralOptionsControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            logPath.Text = OptionsPage.LogPath;
        }

        private void logPath_Leave(object sender, EventArgs e)
        {
            OptionsPage.LogPath = logPath.Text;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists     = false,
                FileName            = ParseFilename(logPath.Text) ?? VsPerformanceTrackerOptionsPage.DefaultLogFileName,
                InitialDirectory    = ParseDirectory(logPath.Text) ?? OptionsPage.DefaultLogDirectory,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                OptionsPage.LogPath = logPath.Text = dialog.FileName;
        }

        private static string ParseFilename(string path)
        {
            return ClampEmptyInvalidToNull(Path.GetFileName, path);
        }

        private static string ParseDirectory(string path)
        {
            return ClampEmptyInvalidToNull(Path.GetDirectoryName, path);
        }

        private static string ClampEmptyInvalidToNull(Func<string, string> transformer, string value)
        {
            try
            {
                var result = transformer(value);
                return string.IsNullOrEmpty(result)
                    ? null
                    : result;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
