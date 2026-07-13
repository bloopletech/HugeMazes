using System.Collections;
using System.Runtime.CompilerServices;
using HugeMazes.Extensions;
using HugeMazes.IO;

namespace HugeMazes.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class LongArray<T> : Storable, ILongArray<T> where T : struct
{
    private static readonly int ItemSize = IStore.SizeOf<T>();
    private const int MaxChunkByteSize = 256 * 1024 * 1024; // Must be power of 2
    private static readonly int ChunkSize = CalculateChunkSize(MaxChunkByteSize);

    private Chunk[] chunks = null!;
    private long length;

    public LongArray(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        InitChunks(false);
    }

    public LongArray(IStore store, long length, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.length = length;
        InitChunks(false);
    }

    public override long Extent => chunks[^1].EndOffset;
    public long Length => length;
    public bool IsReadOnly => false;
    public bool IsFixedSize => true;

    public T this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var (chunkIndex, chunkOffset) = LongArray<T>.Index(index);
            return chunks[chunkIndex].Array[chunkOffset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (chunkIndex, chunkOffset) = LongArray<T>.Index(index);
            chunks[chunkIndex].Array[chunkOffset] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int, int) Index(long index)
    {
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, (ulong)ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        foreach(var chunk in chunks) Array.Clear(chunk.Array);
    }

    public bool Contains(T item) => chunks.Any(c => c.Array.Contains(item));

    public IEnumerator<T> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            foreach(var item in chunk.Array) yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long IndexOf(T item)
    {
        foreach(var chunk in chunks)
        {
            var index = chunk.Array.IndexOf(item);
            if(index >= 0) return chunk.Start + index;
        }
        return -1;
    }

    public T Peek() => this[length - 1];

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

    public ILongArray<T> Clone() => Clone(IStore.Create(IsLong));

    public ILongArray<T> Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongArray<T>(destination, leaveOpen);
        result.Read();
        return result;
    }

    private static int CalculateChunkSize(int maxChunkByteSize)
    {
        var result = (int.MaxValue / ItemSize).RoundDownToPowerOf2();
        while((result * ItemSize) > maxChunkByteSize) result >>= 1;
        return result;
    }

    private class Chunk(LongArray<T> owner, long start, int count, long offset, bool read)
    {
        private T[]? array;
        public T[] Array => array ??= Load();

        public long LastUsedAt { get; private set; }

        public long Start => start;
        public long EndOffset => offset + (ItemSize * count);

        private T[] Load()
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new T[count];
            if(read) owner.Store.Read(offset, array);
            else read = true;
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, array);
            array = null;
            LastUsedAt = 0;
        }

        public static IEnumerable<Chunk> Produce(LongArray<T> owner, long count, int chunkSize, long offset, bool read)
        {
            if(count == 0)
            {
                yield return new(owner, 0, 0, offset, read);
                yield break;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)chunkSize, (uint)System.Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)count.DivCeil(chunkSize), (uint)System.Array.MaxLength);
            var _ = checked((ItemSize * count) + offset);

            var chunkByteSize = ItemSize * chunkSize;
            for(long start = 0, i = 0; start < count; i++)
            {
                var stride = (int)Math.Min(chunkSize, count - start);
                yield return new(owner, start, stride, (i * chunkByteSize) + offset, read);
                start += stride;
            }
        }
    }
}
