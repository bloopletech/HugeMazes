using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public sealed class LongBitGridMazePath(LongBitGrid grid) : IGridMazePath
{
    public LongBitGridMazePath(
        IStore store,
        Size size,
        bool leaveOpen = false) : this(new LongBitGrid(store, size, leaveOpen))
    {
    }

    public IStore Store => grid.Store;
    public bool IsLong => grid.IsLong;
    public long Extent => grid.Extent;
    public Size Size => grid.Size;
    public int Width => grid.Width;
    public int Height => grid.Height; 

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public static LongBitGridMazePath Read(
        IStore store,
        bool leaveOpen = false) => new(LongBitGrid.Read(store, leaveOpen));

    public void Write() => grid.Write();

    public void Dispose() => grid.Dispose();

    public IGridMazePath Clone() => Clone(IStore.Create(IsLong));

    public IGridMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        return new LongBitGridMazePath((LongBitGrid)grid.Clone(destination, leaveOpen));
    }
}
