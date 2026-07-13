using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

public class LongBitGridMaze : Storable, IMaze
{
    private LongBitArray array;
    private MazeSize size;

    public LongBitGridMaze(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public LongBitGridMaze(IStore store, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        array = new(store.Offset<MazeSize>(true), size.Area);
    }

    public override long Extent => array.Extent + MazeSize.SizeOf;
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
    private long Index(int x, int y)
    {
        if((uint)x >= (uint)size.Width) ExceptionExtensions.ThrowOutOfRangeException(x);
        if((uint)y >= (uint)size.Height) ExceptionExtensions.ThrowOutOfRangeException(y);
        return x + ((long)y * size.Width);
    }

    public override void Read()
    {
        size = store.Read<MazeSize>(0);
        array = new(store.Offset<MazeSize>(true), size.Area);
        array.Read();
    }


    public override void Write()
    {
        store.Write(0, size);
        array.Write();
    }

    IMaze IMaze.Clone() => Clone();
    public LongBitGridMaze Clone() => Clone(IStore.Create(IsLong));

    IMaze IMaze.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public LongBitGridMaze Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitGridMaze(destination, leaveOpen);
        result.Read();
        return result;
    }
}
