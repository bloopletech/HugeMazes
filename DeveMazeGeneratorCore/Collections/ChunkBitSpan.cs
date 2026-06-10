using System.Collections;
using System.Diagnostics;

namespace DeveMazeGeneratorCore.Collections;

public readonly record struct ChunkBitSpan(long Start, int Count, long Offset)
{
    public int Length => GetByteArrayLengthFromBitLength(Count);
    public long EndOffset => Offset + Length;
    public long End => Start + Count;

    public static Enumerator Chunk(long length, int chunkSize) => new(length, chunkSize);

    public class Enumerator(long length, int chunkSize) : IEnumerable<ChunkBitSpan>
    {
        public IEnumerator<ChunkBitSpan> GetEnumerator()
        {
            var chunkByteSize = GetByteArrayLengthFromBitLength(chunkSize);
            for(long start = 0, i = 0; start < length; i++)
            {
                var stride = (int)Math.Min(chunkSize, length - start);
                yield return new(start, stride, i * chunkByteSize);
                start += stride;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static int GetByteArrayLengthFromBitLength(int bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (int)(((uint)bitLength + 7u) >> 3);
    }
}
