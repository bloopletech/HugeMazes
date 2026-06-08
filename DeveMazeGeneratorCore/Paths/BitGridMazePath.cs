using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Paths;

public sealed class BitGridMazePath(BitGrid grid) : IGridMazePath
{
    public BitGridMazePath(IStore store, bool leaveOpen = false) : this(new BitGrid(store, leaveOpen))
    {
    }

    public BitGridMazePath(
        IStore store,
        int width,
        int height,
        bool leaveOpen = false) : this(new BitGrid(store, width, height, leaveOpen))
    {
    }

    public IStore Store => grid.Store;
    public bool IsBig => grid.IsBig;
    public long Extent => grid.Extent;
    public int Width => grid.Width;
    public int Height => grid.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public void Read() => grid.Read();

    public async Task ReadAsync() => Read();

    public void Write() => grid.Write();

    public async Task WriteAsync() => Write();

    public void Dispose() => grid.Dispose();

    public IGridMazePath Clone() => Clone(IStore.Create(IsBig));

    public IGridMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        return new BitGridMazePath((BitGrid)grid.Clone(destination, leaveOpen));
    }
}
