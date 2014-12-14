using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VSPerformanceTracker.FSInterface
{
    public class TailReadingFile
    {
        private readonly string _path;
        private long _lastOffset;

        public TailReadingFile(string path)
        {
            _path = path;
        }

        public ILineReader Open()
        {
            return new LineReader(this);
        }

        private long LastOffset
        {
            get { return _lastOffset; }
            set
            {
                if (value > _lastOffset)
                    _lastOffset = value;
            }
        }

        class LineReader : ILineReader
        {
            private readonly FileStream _fs;
            private readonly TailReadingFile _factory;

            public LineReader(TailReadingFile factory)
            {
                _factory = factory;

                _fs = new FileStream(factory._path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _fs.Seek(factory.LastOffset, SeekOrigin.Begin);
            }

            public IEnumerable<string> ReadLines()
            {
                return ReadLinesTill(long.MaxValue);
            }

            public IEnumerable<string> ReadLinesTill(long toOffset)
            {
                using (var reader = OpenReader(_fs))
                {
                    while (_factory.LastOffset < toOffset && !reader.EndOfStream)
                        yield return ReadLine(reader);
                }
            }

            private StreamReader OpenReader(Stream stream)
            {
                return new StreamReader(stream,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true);
            }

            private string ReadLine(StreamReader reader)
            {
                var line = reader.ReadLine();
                _factory.LastOffset = _fs.Position;
                return line;
            }

            public void Dispose()
            {
                _factory.LastOffset = _fs.Position;
                _fs.Dispose();
            }
        }
    }
}
