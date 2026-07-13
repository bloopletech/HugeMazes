using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

public class BitGridMaze : Storable, IMaze
{
    private BitArray array;
    private MazeSize size;

    public BitGridMaze(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public BitGridMaze(IStore store, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        array = new((int)size.Area);
    }

    public override long Extent => CollectionsMarshal.AsBytes(array).Length + MazeSize.SizeOf;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[Index(x, y)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[Index(x, y)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Index(int x, int y)
    {
        if((uint)x >= (uint)size.Width) ExceptionExtensions.ThrowOutOfRangeException(x);
        if((uint)y >= (uint)size.Height) ExceptionExtensions.ThrowOutOfRangeException(y);
        return x + (y * size.Width);
    }

    public override void Read()
    {
        size = store.Read<MazeSize>(0);
        array = new((int)size.Area);
        store.ReadExactly(MazeSize.SizeOf, CollectionsMarshal.AsBytes(array));
    }

    public override void Write()
    {
        store.Write(0, size);
        store.Write(MazeSize.SizeOf, CollectionsMarshal.AsBytes(array));
    }

    IMaze IMaze.Clone() => Clone();
    public BitGridMaze Clone() => Clone(IStore.Create(IsLong));

    IMaze IMaze.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public BitGridMaze Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new BitGridMaze(destination, leaveOpen);
        result.Read();
        return result;
    }
}
