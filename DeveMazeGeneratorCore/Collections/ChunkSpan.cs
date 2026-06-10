using System.Collections;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Collections;

public readonly record struct ChunkSpan(long Start, int Count, long Offset, int ItemSize)
{
    public int Length => ItemSize * Count;
    public long EndOffset => Offset + Length;
    public long End => Start + Count;

    public static Enumerator Chunk(long length, int chunkStride, int itemSize) => new(length, chunkStride, itemSize);

    public class Enumerator(long length, int chunkStride, int itemSize) : IEnumerable<ChunkSpan>
    {
        public IEnumerator<ChunkSpan> GetEnumerator()
        {
            var byteStride = chunkStride * itemSize;
            for(long start = 0, i = 0; start < length; i++)
            {
                var stride = (int)Math.Min(chunkStride, length - start);
                yield return new(start, stride, i * byteStride, itemSize);
                start += stride;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

public readonly record struct ChunkBitSpan(long Start, int Count, long Offset)
{
    public int Length => Count.DivCeil(8);
    public long EndOffset => Offset + Length;
    public long End => Start + Count;

    public static Enumerator Chunk(long length, int chunkStride) => new(length, chunkStride);

    public class Enumerator(long length, int chunkStride) : IEnumerable<ChunkBitSpan>
    {
        public IEnumerator<ChunkBitSpan> GetEnumerator()
        {
            var byteStride = chunkStride.DivCeil(8);
            for(long start = 0, i = 0; start < length; i++)
            {
                var stride = (int)Math.Min(chunkStride, length - start);
                yield return new(start, stride, i * byteStride);
                start += stride;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
