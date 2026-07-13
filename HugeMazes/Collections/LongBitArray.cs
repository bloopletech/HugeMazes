using System.Collections;
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
        InitChunks(false);
    }

    public LongBitArray(IStore store, long length, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.length = length;
        InitChunks(false);
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
    private static (int, int) Index(long index)
    {
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        foreach(var chunk in chunks) chunk.Array.SetAll(false);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            for(var i = 0; i < chunk.Array.Length; i++) yield return chunk.Array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Peek() => this[length - 1];

    private void InitChunks(bool read)
    {
        chunks = [..Chunk.Produce(this, length, ChunkSize, sizeof(long), read)];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public override void Read()
    {
        length = store.ReadInt64(0);
        InitChunks(true);
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

    private class Chunk(LongBitArray owner, int count, long offset, bool read)
    {
        private BitArray? array;
        public BitArray Array => array ??= Load();

        public long LastUsedAt { get; private set; }

        public long EndOffset => offset + count.DivCeil(8);

        private BitArray Load()
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new BitArray(count);
            if(read) owner.Store.ReadExactly(offset, CollectionsMarshal.AsBytes(array));
            else read = true;
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, CollectionsMarshal.AsBytes(array));
            array = null;
            LastUsedAt = 0;
        }

        public static IEnumerable<Chunk> Produce(LongBitArray owner, long count, int chunkSize, long offset, bool read)
        {
            if(count == 0)
            {
                yield return new(owner, 0, offset, read);
                yield break;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)chunkSize, (uint)System.Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)count.DivCeil(chunkSize), (uint)System.Array.MaxLength);

            var chunkByteSize = chunkSize.DivCeil(8);
            for(long start = 0, i = 0; start < count; i++)
            {
                var stride = (int)Math.Min(chunkSize, count - start);
                var chunk = new Chunk(owner, stride, (i * chunkByteSize) + offset, read);
                var _ = checked(chunk.EndOffset);
                yield return chunk;
                start += stride;
            }
        }
    }
}
