using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Paths;

public sealed class BigBitGridMazePath(BigBitGrid grid) : IGridMazePath
{
    public BigBitGridMazePath(IStore store, bool leaveOpen = false) : this(store, 0, 0, leaveOpen)
    {
    }

    public BigBitGridMazePath(
        IStore store,
        int width,
        int height,
        bool leaveOpen = false) : this(new BigBitGrid(store, width, height, leaveOpen))
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

    public async Task ReadAsync() => await grid.ReadAsync();

    public void Write() => grid.Write();

    public async Task WriteAsync() => await grid.WriteAsync();

    public void Dispose() => grid.Dispose();

    public IGridMazePath Clone() => Clone(IStore.Create(IsBig));

    public IGridMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        return new BigBitGridMazePath((BigBitGrid)grid.Clone(destination, leaveOpen));
    }
}
