using System.Collections;

namespace DeveMazeGeneratorCore.Collections;

public readonly record struct ChunkSpan(long Offset, long Start, int Length)
{
    public int Count => Length;
    public long End => Start + Length;

    public static Enumerator Chunk(long length, int chunkLength, int chunkByteLength) =>
        new(length, chunkLength, chunkByteLength);

    public class Enumerator(long length, int chunkLength, int chunkByteLength) : IEnumerable<ChunkSpan>
    {
        public IEnumerator<ChunkSpan> GetEnumerator()
        {
            for(long start = 0, i = 0; start < length; i++)
            {
                var stride = (int)Math.Min(chunkLength, length - start);
                yield return new(i * chunkByteLength, start, stride);
                start += stride;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


