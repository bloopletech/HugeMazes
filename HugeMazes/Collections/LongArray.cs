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
        InitChunks();
    }

    public LongArray(IStore store, long length, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.length = length;
        InitChunks();
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
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, (ulong)ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        foreach(var chunk in chunks) Array.Clear(chunk.WriteArray);
    }

    public bool Contains(T item) => chunks.Any(c => c.ReadArray.Contains(item));

    public IEnumerator<T> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            foreach(var item in chunk.ReadArray) yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long IndexOf(T item)
    {
        foreach(var chunk in chunks)
        {
            var index = chunk.ReadArray.IndexOf(item);
            if(index >= 0) return chunk.Start + index;
        }
        return -1;
    }

    public T Peek() => this[length - 1];

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

    private class Chunk(LongArray<T> owner, long start, int count, long offset)
    {
        private T[]? array;
        public T[] ReadArray => array ??= Load(true);
        public T[] WriteArray => array ??= Load(false);

        public long LastUsedAt { get; private set; }

        public long Start => start;
        public long EndOffset => offset + (ItemSize * count);
        
        private T[] Load(bool read)
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new T[count];
            if(read) owner.Store.Read(offset, array);
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, array);
            array = null;
            LastUsedAt = 0;
        }

        public static IEnumerable<Chunk> Produce(LongArray<T> owner, long count, int chunkSize, long offset)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(chunkSize, Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count.DivCeil(chunkSize), Array.MaxLength);
            var _ = checked((count * ItemSize) + offset);

            if(count == 0)
            {
                yield return new(owner, 0, 0, offset);
                yield break;
            }

            var chunkByteSize = chunkSize * ItemSize;
            for(long start = 0, i = 0; start < count; i++)
            {
                var stride = (int)Math.Min(chunkSize, count - start);
                yield return new(owner, start, stride, (i * chunkByteSize) + offset);
                start += stride;
            }
        }
    }
}
