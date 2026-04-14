using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

// Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs
public class ListChunks<T> : IBigList<T> where T : struct
{
    private readonly IBinarySerializer serializer;
    private readonly long offset;
    private List<ListHolder<T>> chunks;

    //public const int ChunkItemSize = 256 * 1024 * 1024;
    public const int ChunkItemSize = 1024;

    public ListChunks()
    {
        chunks = [CreateChunk()];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get(long index)
    {
        if((ulong)index >= (ulong)Count) ThrowArgumentOutOfRangeException(index);

        var (chunksIndex, chunkOffset) = Math.DivRem((ulong)index, ChunkItemSize);

        var chunk = chunks[(int)chunksIndex];
        return chunk.List[(int)chunkOffset];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(long index, T value)
    {
        if((ulong)index >= (ulong)Count) ThrowArgumentOutOfRangeException(index);

        var (chunksIndex, chunkOffset) = Math.DivRem((ulong)index, ChunkItemSize);

        var chunk = chunks[(int)chunksIndex];
        chunk.List[(int)chunkOffset] = value;
    }

    public long Count => chunks[^1].End;
    public long End => chunks[^1].EndOffset;

    public bool IsReadOnly => false;
    public bool IsFixedSize => false;

    private ListHolder<T> CreateChunk(int chunkSize = 0) => new(this, offset + sizeof(long), 0, chunkSize);

    private void GrowIfNeeded()
    {
        if(chunks[^1].Count == ChunkItemSize) chunks.Add(chunks[^1].CreateNext());
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

        var (chunksIndex, chunkOffset) = Math.DivRem((ulong)index, ChunkItemSize);

        for(var i = chunks.Count - 1; i > (int)chunksIndex; i--)
        {
            chunks[i].List.Unshift(chunks[i - 1].List.Pop());
        }

        var chunk = chunks[(int)chunksIndex];
        chunk.List.Insert((int)chunkOffset, item);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if(index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(long index)
    {
        var (chunksIndex, chunkOffset) = Math.DivRem((ulong)index, ChunkItemSize);

        var chunk = chunks[(int)chunksIndex];
        chunk.List.RemoveAt((int)chunkOffset);

        for(var i = (int)chunksIndex; i < chunks.Count - 1; i++)
        {
            chunks[i].List.Add(chunks[i + 1].List.Shift());
        }

        ShrinkIfNeeded();
    }

    public T Pop()
    {
        var index = Count - 1;
        var item = this[index];
        RemoveAt(index);
        return item;
    }

    public void Push(T item) => Add(item);

    public T Shift()
    {
        var index = 0;
        var item = this[index];
        RemoveAt(index);
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
        var array = new T[(int)Math.Min(Count, int.MaxValue)];

        foreach(var chunk in chunks)
        {
            var count = Math.Min(array.Length, chunk.End) - chunk.Start;
            if(count <= 0) break;
            chunk.List.CopyTo(0, array, (int)chunk.Start, (int)count);
        }

        return array;
    }

    public IList<T> ToList()
    {
        var length = (int)Math.Min(Count, int.MaxValue);

        var list = new List<T>();

        foreach(var chunk in chunks)
        {
            var count = Math.Min(length, chunk.End) - chunk.Start;
            if(count <= 0) break;
            list.AddRange(chunk.List[..(int)count]);
        }

        return list;
    }

    public void AddChunk

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
                chunks.Add(CreateChunk(chunkSize));
            }
            else
            {
                chunks.Add(new(this, chunks[^1].EndOffset, chunks[^1].End, chunkSize));
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

    private static void ThrowArgumentOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");
}
