using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

public class LongBitGridMaze : Storable, IMaze
{
    private Guid id;
    private LongBitArray array;
    private MazeSize size;

    public LongBitGridMaze(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public LongBitGridMaze(IStore store, Guid id, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.id = id;
        this.size = size;
        array = new(store.Offset<Header>(true), size.Area);
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
        array = new(store.Offset<Header>(true), size.Area);
        array.Read();
    }


    public override void Write()
    {
        store.Write(0, new Header(id, size));
        array.Write();
    }

    IMaze IMaze.Clone() => Clone();
    public LongBitGridMaze Clone() => Clone(IStore.Create());

    IMaze IMaze.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public LongBitGridMaze Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitGridMaze(destination, leaveOpen);
        result.Read();
        return result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private record struct Header(Guid Id, MazeSize Size)
    {
        public static readonly int SizeOf = IStore.SizeOf<Header>();
    }
}
