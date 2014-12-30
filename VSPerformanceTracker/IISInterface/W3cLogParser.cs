using System.Collections.Generic;

namespace VSPerformanceTracker.IISInterface
{
    public class W3cLogParser : ILogParser
    {
        private string[] _fieldNames = new string[0];

        public Dictionary<string, string> ParseLine(string line)
        {
            const string fieldsDirecive = "#Fields:";

            Dictionary<string, string> entry = null;

            if (line.StartsWith(fieldsDirecive))
                _fieldNames = Split(line.Substring(fieldsDirecive.Length));

            else if (!line.StartsWith("#"))
                entry = CreateEntry(_fieldNames, Split(line));

            return entry;
        }

        private static Dictionary<string, string> CreateEntry(string[] fieldNames, string[] fields)
        {
            var entry = new Dictionary<string, string>();

            for (var i = 0; i < fields.Length; i++)
                if (i < fieldNames.Length)
                    entry[fieldNames[i]] = fields[i];

            return entry;
        }

        private static string[] Split(string line)
        {
            var w3cFieldSeparators = new char[] { ' ', '\t' };
            return line
                .Trim()
                .Split(w3cFieldSeparators);
        }
    }
}
