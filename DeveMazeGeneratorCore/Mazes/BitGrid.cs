using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid : Storable, IBitGrid
{
    private BitArray array;
    private MazeSize size;

    public BitGrid(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public BitGrid(IStore store, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
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
        if(x < 0 || x >= size.Width) ExceptionExtensions.ThrowOutOfRangeException(x);
        if(y < 0 || y >= size.Height) ExceptionExtensions.ThrowOutOfRangeException(y);
        return x + (y * size.Height);
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

    IBitGrid IBitGrid.Clone() => Clone();
    public BitGrid Clone() => Clone(IStore.Create(IsLong));

    IBitGrid IBitGrid.Clone(IStore destination, bool leaveOpen) => Clone(destination, leaveOpen);
    public BitGrid Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new BitGrid(destination, leaveOpen);
        result.Read();
        return result;
    }


}
