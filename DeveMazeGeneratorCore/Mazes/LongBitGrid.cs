using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class LongBitGrid : Storable, IBitGrid
{
    private LongBitArray array;
    private Size size;

    public LongBitGrid(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public LongBitGrid(IStore store, Size size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        array = new(store.Offset<Size>(true), size.Area);
    }

    public override long Extent => array.Extent + Size.SizeOf;
    public Size Size => size;
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
        if(x < 0 || x >= size.Width) ExceptionExtensions.ThrowOutOfRangeException(x);
        if(y < 0 || y >= size.Height) ExceptionExtensions.ThrowOutOfRangeException(y);
        return x + ((long)y * size.Height);
    }

    public override void Read()
    {
        size = store.Read<Size>(0);
        array = new(store.Offset<Size>(true), size.Area);
        array.Read();
    }


    public override void Write()
    {
        store.Write(0, size);
        array.Write();
    }

    IBitGrid IBitGrid.Clone() => Clone();
    public LongBitGrid Clone() => Clone(IStore.Create(IsLong));

    IBitGrid IBitGrid.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public LongBitGrid Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitGrid(destination, leaveOpen);
        result.Read();
        return result;
    }
}
