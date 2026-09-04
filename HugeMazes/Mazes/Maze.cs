using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;

namespace HugeMazes.Mazes;

public class Maze(IStore store, Guid id, MazeSize size, bool leaveOpen = false) : Storable(store, leaveOpen), IMaze
{
    private LongBitArray array = new(store.Offset<Header>(true), size.Area, true);

    public Maze(IStore store, bool leaveOpen = false) : this(store, default, default, leaveOpen)
    {
    }

    public override long Extent => array.Extent + MazeSize.SizeOf;
    public Guid Id => id;
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
        (id, size) = store.Read<Header>(0);
        array = new(store.Offset<Header>(true), size.Area, true);
        array.Read();
    }

    public override void Write()
    {
        store.Write(0, new Header(id, size));
        array.Write();
    }

    IMaze IMaze.Clone() => Clone();
    public Maze Clone() => Clone(IStore.Create());

    IMaze IMaze.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public Maze Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new Maze(destination, leaveOpen);
        result.Read();
        return result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private readonly record struct Header(Guid Id, MazeSize Size)
    {
        public static readonly int SizeOf = IStore.SizeOf<Header>();
    }
}
