using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class LongBitArray : Storable, ILongBitArray
{
    private const int ChunkByteSize = (256 * 1024 * 1024) - 1;
    private const int ChunkSize = ChunkByteSize * 8; // sizeof(byte) * 8

    private Chunk[] chunks;

    public LongBitArray(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        chunks = InitChunks(0);
    }

    public LongBitArray(IStore store, long bitLength, bool leaveOpen = false) : base(store, leaveOpen)
    {
        chunks = InitChunks(bitLength);
    }

    public override long Extent => chunks[^1].EndOffset;
    public long Length => chunks[^1].End;
    public bool IsReadOnly => false;
    public bool IsFixedSize => true;

    public bool this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var (chunkIndex, chunkOffset) = Index(index);
            return chunks[chunkIndex].Array[chunkOffset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (chunkIndex, chunkOffset) = Index(index);
            chunks[chunkIndex].Array[chunkOffset] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int, int) Index(long index)
    {
        if(index < 0 || (ulong)index >= (ulong)Length) ExceptionExtensions.ThrowOutOfRangeException(index);
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            for(var i = 0; i < chunk.Count; i++) yield return chunk.Array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Peek() => this[Length - 1];

    private Chunk[] InitChunks(long bitLength) => [.. Chunk.Produce(this, bitLength, ChunkSize, sizeof(long))];

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public override void Read()
    {
        chunks = InitChunks(store.ReadInt64(0));
    }

    public override void Write()
    {
        store.SetLength(Extent);
        store.Write(0, Length);

        foreach(var chunk in chunks) chunk.Evict();
    }

    public ILongBitArray Clone() => Clone(IStore.Create(IsLong));

    public ILongBitArray Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitArray(destination, leaveOpen);
        result.Read();
        return result;
    }

    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static int GetByteArrayLengthFromBitLength(int bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (int)(((uint)bitLength + 7u) >> 3);
    }

    private class Chunk(LongBitArray owner, long start, int count, long offset)
    {
        private BitArray? array;
        public BitArray Array => array ??= Load();

        public long LastUsedAt { get; private set; }

        public long Start => start;
        public long Count => count;
        public long End => start + count;

        public long Offset => offset;
        public int Length => GetByteArrayLengthFromBitLength(count);
        public long EndOffset => offset + Length;
        
        public BitArray Load()
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new BitArray(count);
            if(owner.Store.Length >= EndOffset) owner.Store.ReadExactly(offset, CollectionsMarshal.AsBytes(array));
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, CollectionsMarshal.AsBytes(array));
            array = null;
            LastUsedAt = 0;
        }

        public static IEnumerable<Chunk> Produce(LongBitArray owner, long count, int chunkSize, long offset)
        {
            if(count == 0)
            {
                yield return new(owner, 0, 0, offset);
                yield break;
            }

            var chunkByteSize = GetByteArrayLengthFromBitLength(chunkSize);
            for(long start = 0, i = 0; start < count; i++)
            {
                var stride = (int)Math.Min(chunkSize, count - start);
                yield return new(owner, start, stride, (i * chunkByteSize) + offset);
                start += stride;
            }
        }
    }
}
