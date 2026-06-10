using System.Collections;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public readonly record struct ChunkSpan<T>(long Start, int Count, long Offset) where T : struct
{
    public static readonly int ItemSize = IStore.SizeOf<T>();

    public int Length => ItemSize * Count;
    public long EndOffset => Offset + Length;
    public long End => Start + Count;

    public static int CalculateChunkSize(int maxChunkByteSize)
    {
        var result = (int.MaxValue / ItemSize).RoundDownToPowerOf2();
        while((result * ItemSize) > maxChunkByteSize) result >>= 1;
        return result;
    }

    public static Enumerator Chunk(long length, int chunkSize) => new(length, chunkSize);

    public class Enumerator(long length, int chunkSize) : IEnumerable<ChunkSpan<T>>
    {
        public IEnumerator<ChunkSpan<T>> GetEnumerator()
        {
            var chunkByteSize = chunkSize * ItemSize;
            for(long start = 0, i = 0; start < length; i++)
            {
                var stride = (int)Math.Min(chunkSize, length - start);
                yield return new(start, stride, i * chunkByteSize);
                start += stride;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
