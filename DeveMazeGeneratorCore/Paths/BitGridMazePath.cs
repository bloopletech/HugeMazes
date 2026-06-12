using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public sealed class BitGridMazePath(BitGrid grid) : IGridMazePath
{
    public BitGridMazePath(IStore store, Size size, bool leaveOpen = false) : this(new BitGrid(store, size, leaveOpen))
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

    public static BitGridMazePath Read(IStore store, bool leaveOpen = false) => new(BitGrid.Read(store, leaveOpen));

    public void Write() => grid.Write();

    public void Dispose() => grid.Dispose();

    public IGridMazePath Clone() => Clone(IStore.Create(IsLong));

    public IGridMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        return new BitGridMazePath((BitGrid)grid.Clone(destination, leaveOpen));
    }
}
