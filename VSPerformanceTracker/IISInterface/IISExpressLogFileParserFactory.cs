using System.Collections.Generic;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public class IISExpressLogFileParserFactory
    {
        private readonly Dictionary<string, IISExpressLogFileParser> _parsers = new Dictionary<string, IISExpressLogFileParser>();

        private readonly IFileSizesSnapshot _sizeSnapshot;

        public IISExpressLogFileParserFactory(IFileSizesSnapshot sizeSnapshot)
        {
            _sizeSnapshot = sizeSnapshot;
        }

        public void InitializeSkipOffsets()
        {
            _sizeSnapshot.TakeSnapshot();
        }

        public IISExpressLogFileParser GetParser(string path)
        {
            if (!_parsers.ContainsKey(path))
            {
                var parser = new IISExpressLogFileParser(
                    _sizeSnapshot.GetSize(path) ?? 0,
                    new TailReadingFile(path),
                    new W3cLogParser());
                _parsers.Add(path, parser);
            }

            return _parsers[path];
        }
    }
}
