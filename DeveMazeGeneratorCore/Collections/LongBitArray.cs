using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public class LongBitArray : ILongBitArray, IStorable
{
    private const int ChunkByteSize = (256 * 1024 * 1024) - 1;
    private const int ChunkSize = ChunkByteSize * 8; // sizeof(byte) * 8

    private readonly IStore store;
    private readonly bool leaveOpen;
    private Chunk[] chunks;
    private bool disposed;

    public LongBitArray(IStore store, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        chunks = InitChunks(0);
    }

    public LongBitArray(IStore store, long bitLength, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        chunks = InitChunks(bitLength);
    }

    public IStore Store => store;
    public bool IsLong => Extent > int.MaxValue;
    public long Extent => chunks[^1].EndOffset;

    public long Length => chunks[^1].End;
    public int IntLength => (int)Math.Min(Length, int.MaxValue);

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
        if((ulong)index >= (ulong)Length) ThrowArgumentOutOfRangeException(index);
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        chunks = InitChunks(0);
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

    public bool[] ToArray()
    {
        var array = new bool[IntLength];

        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(array.Length, chunk.End) - chunk.Start);
            if(count <= 0) break;
            for(var i = 0; i < count; i++) array[(int)chunk.Start + i] = chunk.Array[i];
        }

        return array;
    }

    public IList<bool> ToList()
    {
        var list = new List<bool>();

        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(IntLength, chunk.End) - chunk.Start);
            if(count <= 0) break;
            for(var i = 0; i < count; i++) list.Add(chunk.Array[i]);
        }

        return list;
    }

    private Chunk[] InitChunks(long bitLength)
    {
        if(bitLength == 0) return [new(this, 0, 0, 0)];
        return [..Chunk.Produce(this, bitLength, ChunkSize)];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public void Read()
    {
        chunks = InitChunks(store.ReadInt64(0));
    }

    public async Task ReadAsync()
    {
        // TODO: Figure out some background queue writer async thingo
        Read();
    }

    public void Write()
    {
        store.SetLength(Extent);
        store.Write(0, Length);

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

    public ILongBitArray Clone() => Clone(IStore.Create(IsLong));

    public ILongBitArray Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitArray(destination, leaveOpen);
        result.Read();
        return result;
    }

    public async Task<ILongBitArray> CloneAsync() => await CloneAsync(IStore.Create(IsLong));

    public async Task<ILongBitArray> CloneAsync(IStore destination, bool leaveOpen = false)
    {
        await WriteAsync();
        await store.CopyToAsync(destination);
        var result = new LongBitArray(destination, leaveOpen);
        await result.ReadAsync();
        return result;
    }

    private static void ThrowArgumentOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");

    // Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static int GetByteArrayLengthFromBitLength(int bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (int)(((uint)bitLength + 7u) >> 3);
    }

    private record struct Chunk(LongBitArray Owner, long Start, int Count, long Offset)
    {
        private BitArray? array;

        public BitArray Array
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                array ??= Load();
                return array;
            }
        }

        public long LastUsedAt { get; set; } = long.MinValue;

        public readonly int Length => GetByteArrayLengthFromBitLength(Count);
        public readonly long EndOffset => Offset + Length;
        public readonly long End => Start + Count;

        public BitArray Load()
        {
            LastUsedAt = Environment.TickCount64;
            Owner.EvictOldest();

            var array = new BitArray(Count);
            if(Owner.Store.Length >= EndOffset) Owner.Store.ReadExactly(Offset, CollectionsMarshal.AsBytes(array));
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            Owner.Store.Write(Offset, CollectionsMarshal.AsBytes(array));
            array = null;
            LastUsedAt = long.MinValue;
        }

        public static IEnumerable<Chunk> Produce(LongBitArray owner, long length, int chunkSize)
        {
            var chunkByteSize = GetByteArrayLengthFromBitLength(chunkSize);
            for(long start = 0, i = 0; start < length; i++)
            {
                var stride = (int)Math.Min(chunkSize, length - start);
                yield return new(owner, start, stride, (i * chunkByteSize) + sizeof(long));
                start += stride;
            }
        }
    }
}