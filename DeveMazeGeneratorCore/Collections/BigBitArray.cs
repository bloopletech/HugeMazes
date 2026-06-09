using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class BigBitArray : IBigBitArray, IStorable
{
    private const int ChunkByteSize = (256 * 1024 * 1024) - 1;
    private const int BitsPerByte = 8; // sizeof(byte) * 8
    private const int ChunkBitSize = ChunkByteSize * BitsPerByte;

    private readonly IStore store;
    private readonly bool leaveOpen;
    private Chunk[] chunks;
    private long bitLength;
    private long byteLength;
    private bool disposed;

    public BigBitArray(IStore store, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        chunks = null!;
    }

    public BigBitArray(IStore store, long bitLength, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        this.bitLength = bitLength;
        byteLength = GetByteArrayLengthFromBitLength(bitLength);
        chunks = InitChunks(true);
    }

    public IStore Store => store;
    public bool IsBig => Extent > int.MaxValue;
    public long Extent => byteLength + sizeof(long);

    public long Length => bitLength;
    public int IntLength => (int)Math.Min(bitLength, int.MaxValue);

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
    private static (int, int) ChunkOffset(long offset, int chunkSize)
    {
        var (chunk, chunkOffset) = Math.DivRem((ulong)offset, (ulong)chunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (int, int) Index(long index)
    {
        if((ulong)index >= (ulong)bitLength) ThrowArgumentOutOfRangeException(index);
        return ChunkOffset(index, ChunkBitSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        chunks = [];
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            var count = (int)(Math.Min(IntLength, chunk.End) - chunk.Start);
            if(count <= 0) break;
            for(var i = 0; i < count; i++) yield return chunk.Array[i];
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

    private Chunk[] InitChunks(bool skipFirstLoad)
    {
        var remaining = byteLength;

        var chunks = new List<Chunk>();
        while(remaining > 0)
        {
            var chunkByteLength = (int)Math.Min(remaining, ChunkByteSize);

            if(chunks.Count == 0)
            {
                chunks.Add(new(this, sizeof(long), 0, chunkByteLength, skipFirstLoad));
            }
            else
            {
                chunks.Add(chunks[^1].Next(chunkByteLength));
            }

            remaining -= chunkByteLength;
        }

        return [..chunks];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public void Read()
    {
        bitLength = store.ReadInt64(0);
        byteLength = GetByteArrayLengthFromBitLength(bitLength);
        chunks = InitChunks(false);
    }

    public async Task ReadAsync()
    {
        // TODO: Figure out some background queue writer async thingo
        Read();
    }

    public void Write()
    {
        store.SetLength(byteLength);
        store.Write(0, bitLength);

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

    public IBigBitArray Clone() => Clone(IStore.Create(IsBig));

    public IBigBitArray Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new BigBitArray(destination, leaveOpen);
        result.Read();
        return result;
    }

    public async Task<IBigBitArray> CloneAsync() => await CloneAsync(IStore.Create(IsBig));

    public async Task<IBigBitArray> CloneAsync(IStore destination, bool leaveOpen = false)
    {
        await WriteAsync();
        await store.CopyToAsync(destination);
        var result = new BigBitArray(destination, leaveOpen);
        await result.ReadAsync();
        return result;
    }

    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static long GetByteArrayLengthFromBitLength(long bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (long)(((ulong)bitLength + 7u) >> 3);
    }

    private static void ThrowArgumentOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");

    private class Chunk(BigBitArray owner, long offset, long start, int byteLength, bool skipFirstLoad)
    {
        private BitArray? array;

        public BitArray Array
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                LastUsedAt = Environment.TickCount64;
                array ??= Load();
                return array;
            }
        }

        public long LastUsedAt { get; set; } = long.MinValue;

        public long EndOffset => offset + byteLength;

        public long Start => start;
        public int Length => byteLength * BitsPerByte;
        public long End => Start + Length;

        public BitArray Load()
        {
            owner.EvictOldest();
            var array = new BitArray(byteLength * BitsPerByte);
            if(skipFirstLoad) skipFirstLoad = false;
            else owner.Store.ReadExactly(offset, CollectionsMarshal.AsBytes(array));
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, CollectionsMarshal.AsBytes(array));
            array = null;
            LastUsedAt = long.MinValue;
        }

        public Chunk Next(int byteLength) => new(owner, EndOffset, End, byteLength, skipFirstLoad);
    }
}