using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

public class JaggedBitGrid : Storable, IBitGrid, IEnumerable
{
    private Chunk[] chunks = null!;
    private MazeSize size;

    public JaggedBitGrid(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        InitChunks();
    }

    public JaggedBitGrid(IStore store, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        InitChunks();
    }

    public override long Extent => chunks[^1].EndOffset;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => chunks[y].Array()[x];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => chunks[y].Array(false)[x] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        foreach(var chunk in chunks) chunk.Array().SetAll(false);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            for(var i = 0; i < size.Width; i++) yield return chunk.Array()[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Peek() => this[size.Width - 1, size.Height - 1];

    private void InitChunks()
    {
        chunks = [..Chunk.Produce(this, size.Height, size.Width, MazeSize.SizeOf)];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public override void Read()
    {
        size = store.Read<MazeSize>(0);
        InitChunks();
    }

    public override void Write()
    {
        store.SetLength(Extent);
        store.Write(0, size);

        foreach(var chunk in chunks) chunk.Evict();
    }

    IBitGrid IBitGrid.Clone() => Clone();
    public JaggedBitGrid Clone() => Clone(IStore.Create(IsLong));

    IBitGrid IBitGrid.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public JaggedBitGrid Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new JaggedBitGrid(destination, leaveOpen);
        result.Read();
        return result;
    }

    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static int GetByteArrayLengthFromBitLength(int bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (int)(((uint)bitLength + 7u) >> 3);
    }

    private class Chunk(JaggedBitGrid owner, int count, long offset)
    {
        private BitArray? array;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitArray Array(bool read = true) => array ??= Load(read);

        public long LastUsedAt { get; private set; }

        public long EndOffset => offset + GetByteArrayLengthFromBitLength(count);

        public BitArray Load(bool read)
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new BitArray(count);
            if(read) owner.Store.ReadExactly(offset, CollectionsMarshal.AsBytes(array));
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, CollectionsMarshal.AsBytes(array));
            array = null;
            LastUsedAt = 0;
        }

        public static IEnumerable<Chunk> Produce(JaggedBitGrid owner, int chunkCount, int chunkSize, long offset)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(chunkCount, System.Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(chunkSize, System.Array.MaxLength);

            if(chunkCount == 0)
            {
                yield return new(owner, chunkSize, offset);
                yield break;
            }

            long chunkByteSize = GetByteArrayLengthFromBitLength(chunkSize);
            for(var i = 0; i < chunkCount; i++)
            {
                yield return new(owner, chunkSize, (i * chunkByteSize) + offset);
            }
        }
    }
}
