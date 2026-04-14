using System.IO.Compression;
using System.Text;

namespace DeveMazeGeneratorCore.Extensions;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        //public static bool IsCompleted(this Stream stream) => stream.Length == 0 || (stream.Position + 1) == stream.Length;
        public bool IsCompleted() => stream.Position == stream.Length;
        public bool HasMore() => !stream.IsCompleted();

        public void EnsureCompleted()
        {
            if(stream.HasMore()) throw new InvalidDataException("Stream contains more data than expected");
        }

        public BinaryReader Reader() => new(stream, Encoding.UTF8, true);
        public BinaryWriter Writer() => new(stream, Encoding.UTF8, true);
        public ZstandardStream Compressor(CompressionLevel level = CompressionLevel.Optimal) => new(stream, level, true);
        public ZstandardStream Decompressor() => new(stream, CompressionMode.Decompress, true);
    }
}
