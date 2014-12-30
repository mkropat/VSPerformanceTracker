using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

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
                _factory.LastOffset = reader.GetPosition(); //FIXME: write PositionStreamReader class and use that
                return line;
            }

            public void Dispose()
            {
                _fs.Dispose();
            }
        }
    }

    public static class StreamReaderExtensions
    {
        // GetPosition based on code written by Granger Godbold [1].
        // [1] http://stackoverflow.com/a/17457085/27581
        public static long GetPosition(this StreamReader reader)
        {
            // The current buffer of decoded characters
            var charBuffer = (char[])GetNonPublicField(reader, "charBuffer");

            // The current position in the buffer of decoded characters
            var charPos = (int)GetNonPublicField(reader, "charPos");

            // The number of bytes that the already-read characters need when encoded.
            var numReadBytes = reader.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

            // The number of encoded bytes that are in the current buffer
            var byteLen = (int)GetNonPublicField(reader, "byteLen");

            return reader.BaseStream.Position - byteLen + numReadBytes;
        }

        private static object GetNonPublicField(object target, string fieldName)
        {
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
            return target.GetType().InvokeMember(fieldName, flags, null, target, null);
        }
    }
}
