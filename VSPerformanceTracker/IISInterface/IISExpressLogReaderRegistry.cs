using System;
using System.Collections.Generic;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.IISInterface
{
    public class IISExpressLogReaderRegistry : ILogFileReaderRegistry
    {
        private readonly Dictionary<string, ILogReader> _parsers = new Dictionary<string, ILogReader>();

        private readonly Func<IFileSizesSnapshot> _takeSizeSnapshot;
        private readonly ILogReaderFactory _readerFactory;
        private IFileSizesSnapshot _sizeSnapshot;

        public IISExpressLogReaderRegistry(Func<IFileSizesSnapshot> takeSizeSnapshot, ILogReaderFactory readerFactory)
        {
            _takeSizeSnapshot = takeSizeSnapshot;
            _readerFactory = readerFactory;
        }

        public void InitializeSkipOffsets()
        {
            _sizeSnapshot = _takeSizeSnapshot();
        }

        public ILogReader GetReader(string path)
        {
            if (!_parsers.ContainsKey(path))
                _parsers.Add(path, CreateReader(path));

            return _parsers[path];
        }

        private ILogReader CreateReader(string path)
        {
            var offset = _sizeSnapshot.GetSize(path) ?? 0;
            return _readerFactory.Create(path, offset);
        }
    }
}
