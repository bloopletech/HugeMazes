using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Extensions;
using HugeMazes.IO;

namespace HugeMazes.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class LongBitArray : Storable, ILongBitArray
{
    private const int ChunkSize = 0x40000000; // (2 ^ 30)

    private Chunk[] chunks = null!;
    private long length;

    public LongBitArray(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        InitChunks();
    }

    public LongBitArray(IStore store, long length, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.length = length;
        InitChunks();
    }

    public override long Extent => chunks[^1].EndOffset;
    public long Length => length;
    public bool IsReadOnly => false;
    public bool IsFixedSize => true;

    public bool this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var (chunkIndex, chunkOffset) = Index(index);
            return chunks[chunkIndex].ReadArray[chunkOffset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (chunkIndex, chunkOffset) = Index(index);
            chunks[chunkIndex].WriteArray[chunkOffset] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int, int) Index(long index)
    {
        if((ulong)index >= (ulong)length) ExceptionExtensions.ThrowOutOfRangeException(index);
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        foreach(var chunk in chunks) chunk.WriteArray.SetAll(false);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            for(var i =  0; i < chunk.ReadArray.Length; i++) yield return chunk.ReadArray[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Peek() => this[length - 1];

    private void InitChunks()
    {
        chunks = [..Chunk.Produce(this, length, ChunkSize, sizeof(long))];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public override void Read()
    {
        length = store.ReadInt64(0);
        InitChunks();
    }

    public override void Write()
    {
        store.SetLength(Extent);
        store.Write(0, length);

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

    private class Chunk(LongBitArray owner, int count, long offset)
    {
        private BitArray? array;
        public BitArray ReadArray => array ??= Load(true);
        public BitArray WriteArray => array ??= Load(false);

        public long LastUsedAt { get; private set; }

        public long EndOffset => offset + GetByteArrayLengthFromBitLength(count);

        private BitArray Load(bool read)
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new BitArray(count);
            if(read) owner.Store.ReadExactly(offset, CollectionsMarshal.AsBytes(array));
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
            ArgumentOutOfRangeException.ThrowIfGreaterThan(chunkSize, Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count.DivCeil(chunkSize), Array.MaxLength);

            if(count == 0)
            {
                yield return new(owner, 0, offset);
                yield break;
            }

            var chunkByteSize = GetByteArrayLengthFromBitLength(chunkSize);
            for(long start = 0, i = 0; start < count; i++)
            {
                var stride = (int)Math.Min(chunkSize, count - start);
                yield return new(owner, stride, (i * chunkByteSize) + offset);
                start += stride;
            }
        }
    }
}
