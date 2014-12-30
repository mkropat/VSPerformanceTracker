using System;
using System.Collections.Generic;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public class IISExpressLogFileParserFactory
    {
        private readonly Dictionary<string, IISExpressLogFileParser> _parsers = new Dictionary<string, IISExpressLogFileParser>();

        private readonly Func<IFileSizesSnapshot> _takeSizeSnapshot;
        private IFileSizesSnapshot _sizeSnapshot;

        public IISExpressLogFileParserFactory(Func<IFileSizesSnapshot> takeSizeSnapshot)
        {
            _takeSizeSnapshot = takeSizeSnapshot;
        }

        public void InitializeSkipOffsets()
        {
            _sizeSnapshot = _takeSizeSnapshot();
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
