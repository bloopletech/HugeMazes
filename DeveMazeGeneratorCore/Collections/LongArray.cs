using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class LongArray<T> : ILongArray<T>, IStorable where T : struct
{
    private const int MaxChunkByteSize = 256 * 1024 * 1024; // Must be power of 2
    private static readonly int ChunkSize = ChunkSpan<T>.CalculateChunkSize(MaxChunkByteSize);

    private readonly IStore store;
    private readonly bool leaveOpen;
    private Chunk[] chunks;
    private long length;
    private bool disposed;

    public LongArray(IStore store, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        chunks = null!;
    }

    public LongArray(IStore store, long length, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        this.length = length;
        chunks = InitChunks(true);
    }

    public IStore Store => store;
    public bool IsLong => Extent > int.MaxValue;
    public long Extent => (length * ChunkSpan<T>.ItemSize) + sizeof(long);

    public long Length => length;
    public int IntLength => (int)Math.Min(length, int.MaxValue);

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
    private static (int, int) ChunkOffset(long offset)
    {
        var (chunk, chunkOffset) = Math.DivRem((ulong)offset, (ulong)ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int, int) Index(long index)
    {
        if((ulong)index >= (ulong)length) ThrowArgumentOutOfRangeException(index);
        return ChunkOffset(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        chunks = [];
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

    public T[] ToArray()
    {
        var array = new T[IntLength];

        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(array.Length, chunk.End) - chunk.Start);
            if(count <= 0) break;
            Array.Copy(chunk.Array, 0, array, (int)chunk.Start, count);
        }

        return array;
    }

    public IList<T> ToList()
    {
        var list = new List<T>();

        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(IntLength, chunk.End) - chunk.Start);
            if(count <= 0) break;
            list.AddRange(chunk.Array[..count]);
        }

        return list;
    }

    private Chunk[] InitChunks(bool skipFirstLoad)
    {
        return [..ChunkSpan<T>.Chunk(length, ChunkSize).Select(span => new Chunk(this, span, skipFirstLoad))];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public void Read()
    {
        length = store.ReadInt64(0);
        chunks = InitChunks(false);
    }

    public async Task ReadAsync()
    {
        // TODO: Figure out some background queue writer async thingo
        Read();
    }

    public void Write()
    {
        store.SetLength(Extent);
        store.Write(0, length);

        foreach(var chunk in chunks) chunk.Evict();
    }

    public async Task WriteAsync()
    {
        // TODO: Figure out some background queue writer async thingo
        Write();
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!disposed)
        {
            if(disposing)
            {
                Write();
                if(!leaveOpen) store.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
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

    public async Task<ILongArray<T>> CloneAsync() => await CloneAsync(IStore.Create(IsLong));

    public async Task<ILongArray<T>> CloneAsync(IStore destination, bool leaveOpen = false)
    {
        await WriteAsync();
        await store.CopyToAsync(destination);
        var result = new LongArray<T>(destination, leaveOpen);
        await result.ReadAsync();
        return result;
    }

    private static void ThrowArgumentOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");

    private class Chunk(LongArray<T> owner, ChunkSpan<T> span, bool skipFirstLoad)
    {
        private T[]? array;

        public T[] Array
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                array ??= Load();
                return array;
            }
        }

        public long LastUsedAt { get; set; } = long.MinValue;

        public long Offset => span.Offset + sizeof(long);
        public long Start => span.Start;
        public int Length => span.Count;
        public long End => span.End;

        private T[] Load()
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new T[Length];
            if(skipFirstLoad) skipFirstLoad = false;
            else owner.Store.Read(Offset, array);
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(Offset, array);
            array = null;
            LastUsedAt = long.MinValue;
        }
    }
}