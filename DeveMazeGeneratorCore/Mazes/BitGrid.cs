using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid : Storable, IBitGrid
{
    private BitArray array;
    private Size size;

    public BitGrid(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public BitGrid(IStore store, Size size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        array = new((int)size.Area);
    }

    public override long Extent => CollectionsMarshal.AsBytes(array).Length + Size.SizeOf;
    public Size Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * size.Height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * size.Height)] = value;
    }

    public override void Read()
    {
        size = store.Read<Size>(0);
        array = new((int)size.Area);
        store.ReadExactly(Size.SizeOf, CollectionsMarshal.AsBytes(array));
    }

    public override void Write()
    {
        store.Write(0, size);
        store.Write(Size.SizeOf, CollectionsMarshal.AsBytes(array));
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
