using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

public class JaggedBitGridMaze : Storable, IMaze, IEnumerable
{
    private const int MaxChunkByteSize = 256 * 1024 * 1024; // Must be power of 2

    private Chunk[] chunks = null!;
    private MazeSize size;
    private int keepChunks;

    public JaggedBitGridMaze(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        InitChunks(false);
    }

    public JaggedBitGridMaze(IStore store, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        InitChunks(false);
    }

    public override long Extent => chunks[^1].EndOffset;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => chunks[y].Array[x];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => chunks[y].Array[x] = value;
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
            for(var i = 0; i < size.Width; i++) yield return chunk.Array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Peek() => this[size.Width - 1, size.Height - 1];

    private void InitChunks(bool read)
    {
        chunks = [..Chunk.Produce(this, size.Height, size.Width, MazeSize.SizeOf, read)];
        keepChunks = 0;
        foreach(var chunk in chunks)
        {
            if(keepChunks + chunk.Length > MaxChunkByteSize) break;
            keepChunks += chunk.Length;
        }
        keepChunks = Math.Max(keepChunks, 3);
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(keepChunks);
        foreach(var c in toEvict) c.Evict();
    }

    public override void Read()
    {
        size = store.Read<MazeSize>(0);
        InitChunks(true);
    }

    public override void Write()
    {
        store.SetLength(Extent);
        store.Write(0, size);

        foreach(var chunk in chunks) chunk.Evict();
    }

    IMaze IMaze.Clone() => Clone();
    public JaggedBitGridMaze Clone() => Clone(IStore.Create(IsLong));

    IMaze IMaze.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public JaggedBitGridMaze Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new JaggedBitGridMaze(destination, leaveOpen);
        result.Read();
        return result;
    }

    private class Chunk(JaggedBitGridMaze owner, int count, long offset, bool read)
    {
        private BitArray? array;
        public BitArray Array => array ??= Load();

        public long LastUsedAt { get; private set; }

        public int Length => count.DivCeil(8);
        public long EndOffset => offset + Length;

        public BitArray Load()
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

        public static IEnumerable<Chunk> Produce(JaggedBitGridMaze owner, int chunkCount, int chunkSize, long offset, bool read)
        {
            if(chunkCount == 0)
            {
                yield return new(owner, 0, offset, read);
                yield break;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)chunkCount, (uint)System.Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)chunkSize, (uint)System.Array.MaxLength);

            long chunkByteSize = chunkSize.DivCeil(8);
            var _ = checked((chunkCount * chunkByteSize) + offset);

            for(var i = 0; i < chunkCount; i++)
            {
                yield return new(owner, chunkSize, (i * chunkByteSize) + offset, read);
            }
        }
    }
}
