using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class LongArray<T> : Storable, ILongArray<T> where T : struct
{
    private static readonly int ItemSize = IStore.SizeOf<T>();
    private const int MaxChunkByteSize = 256 * 1024 * 1024; // Must be power of 2
    private static readonly int ChunkSize = CalculateChunkSize(MaxChunkByteSize);

    private Chunk[] chunks;

    public LongArray(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        chunks = InitChunks(0);
    }

    public LongArray(IStore store, long length, bool leaveOpen = false) : base(store, leaveOpen)
    {
        chunks = InitChunks(length);
    }

    public override long Extent => chunks[^1].EndOffset;
    public long Length => chunks[^1].End;
    public bool IsReadOnly => false;
    public bool IsFixedSize => true;

    public T this[long index]
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

    public T Peek() => this[Length - 1];

    private Chunk[] InitChunks(long count) => [..Chunk.Produce(this, count, ChunkSize, sizeof(long))];

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
        public T[] Array => array ??= Load();

        public long LastUsedAt { get; private set; }

        public long Start => start;
        public long Count => count;
        public long End => start + count;

        public long Offset => offset;
        public int Length => ItemSize * count;
        public long EndOffset => offset + Length;
        
        private T[] Load()
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new T[count];
            if(owner.Store.Length >= EndOffset) owner.Store.Read(Offset, array);
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(Offset, array);
            array = null;
            LastUsedAt = 0;
        }

        public static IEnumerable<Chunk> Produce(LongArray<T> owner, long count, int chunkSize, long offset)
        {
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
