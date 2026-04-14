using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

// Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs
public class BigList<T> : IBigList<T>, IBinarySerializable where T : struct
{
    private readonly IBinarySerializer serializer;
    private readonly long offset;
    private List<ListHolder<T>> chunks;
    private bool disposed;

    //public const int ChunkItemSize = 256 * 1024 * 1024;
    public const int ChunkItemSize = 1024;

    public BigList(IBinarySerializer serializer, long offset)
    {
        this.serializer = serializer;
        this.offset = offset;
        chunks = [new(this, offset + sizeof(long))];
    }

    public IBinarySerializer Serializer => serializer;
    public long Offset => offset;

    public long Count => chunks[^1].End;
    public int IntCount => (int)Math.Min(Count, int.MaxValue);

    public bool IsReadOnly => false;
    public bool IsFixedSize => false;

    public T this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var (chunkIndex, chunkOffset) = Index(index);
            return chunks[chunkIndex].List[chunkOffset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (chunkIndex, chunkOffset) = Index(index);
            chunks[chunkIndex].List[chunkOffset] = value;
        }
    }

    private (int chunkIndex, int chunkOffset) Index(long index)
    {
        if((ulong)index >= (ulong)Count) ThrowArgumentOutOfRangeException(index);
        var (chunkIndex, chunkOffset) = Math.DivRem((ulong)index, ChunkItemSize);
        return ((int)chunkIndex, (int)chunkOffset);
    }

    //private ListHolder<T> Last => chunks[^1];
    private ListHolder<T> CreateChunk(int chunkSize = 0) => new(this, offset + sizeof(long), 0, chunkSize);

    private void GrowIfNeeded()
    {
        if(chunks[^1].Count == ChunkItemSize) chunks.Add(chunks[^1].Next());
    }

    private void ShrinkIfNeeded()
    {
        if(chunks[^1].Count == 0 && chunks.Count > 1) chunks.Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        GrowIfNeeded();
        chunks[^1].List.Add(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        chunks = [CreateChunk()];
    }

    public bool Contains(T item) => chunks.Any(c => c.List.Contains(item));

    public IEnumerator<T> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            foreach(var item in chunk.List) yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long IndexOf(T item)
    {
        foreach(var chunk in chunks)
        {
            var index = chunk.List.IndexOf(item);
            if(index >= 0) return chunk.Start + index;
        }
        return -1;
    }

    public void Insert(long index, T item)
    {
        GrowIfNeeded();

        var (chunkIndex, chunkOffset) = Index(index);

        for(var i = chunks.Count - 1; i > chunkIndex; i--)
        {
            chunks[i].List.Unshift(chunks[i - 1].List.Pop());
        }

        chunks[chunkIndex].List.Insert(chunkOffset, item);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if(index < 0) return false;
        RemoveAt(index);
        return true;
    }

    public void RemoveAt(long index)
    {
        var (chunkIndex, chunkOffset) = Index(index);

        chunks[chunkIndex].List.RemoveAt(chunkOffset);

        for(var i = chunkIndex; i < chunks.Count - 1; i++)
        {
            chunks[i].List.Add(chunks[i + 1].List.Shift());
        }

        ShrinkIfNeeded();
    }

    public T Pop()
    {
        var item = this[Count - 1];
        RemoveAt(Count - 1);
        return item;
    }

    public void Push(T item) => Add(item);

    public T Shift()
    {
        var item = this[0];
        RemoveAt(0);
        return item;
    }

    public void Unshift(T item) => Insert(0, item);

    public T Peek() => this[Count - 1];

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public T[] ToArray()
    {
        var array = new T[IntCount];

        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(array.Length, chunk.End) - chunk.Start);
            if(count <= 0) break;
            chunk.List.CopyTo(0, array, (int)chunk.Start, count);
        }

        return array;
    }

    public IList<T> ToList()
    {
        var list = new List<T>();

        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(IntCount, chunk.End) - chunk.Start);
            if(count <= 0) break;
            list.AddRange(chunk.List[..count]);
        }

        return list;
    }

    public void Read()
    {
        var remaining = serializer.ReadInt64(offset);

        if(remaining == 0)
        {
            Clear();
            return; 
        }

        chunks = [];
        while(remaining > 0)
        {
            var chunkSize = (int)Math.Min(remaining, ChunkItemSize);

            if(chunks.Count == 0)
            {
                chunks.Add(new(this, offset + sizeof(long), 0, chunkSize));
            }
            else
            {
                chunks.Add(chunks[^1].Next(chunkSize));
            }

            remaining -= ChunkItemSize;
        }
    }

    public async Task ReadAsync()
    {
        // TODO: Figure out some background queue reader async thingo
        Read();
    }

    public void Write()
    {
        serializer.SetLength(chunks[^1].EndOffset);
        serializer.Write(offset, Count);

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
            if(disposing) Write();
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public IBigList<T> Clone() => Clone(serializer.Create(), 0);

    public IBigList<T> Clone(IBinarySerializer destination, long offset)
    {
        Write();
        serializer.CopyTo(destination);
        var result = new BigList<T>(destination, offset);
        result.Read();
        return result;
    }

    public async Task<IBigList<T>> CloneAsync() => await CloneAsync(serializer.Create(), 0);

    public async Task<IBigList<T>> CloneAsync(IBinarySerializer destination, long offset)
    {
        await WriteAsync();
        await serializer.CopyToAsync(destination);
        var result = new BigList<T>(destination, offset);
        await result.ReadAsync();
        return result;
    }

    public static BigList<T> CreateFile() => new(BinarySerializer.CreateFile(), 0);

    public static BigList<T> CreateMemory() => new(BinarySerializer.CreateMemory(), 0);

    public BigList<T> Create() => chunks[^1].EndOffset > int.MaxValue ? CreateFile() : CreateMemory();

    private static void ThrowArgumentOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");
}
