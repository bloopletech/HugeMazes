using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public sealed class BigBitGridMaze(BigBitGrid grid) : IMaze
{
    public BigBitGridMaze(IStore store, bool leaveOpen = false) : this(store, 0, 0, leaveOpen)
    {
    }

    public BigBitGridMaze(
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

    public IMaze Clone() => Clone(IStore.Create(IsBig));

    public IMaze Clone(IStore destination, bool leaveOpen = false)
    {
        return new BigBitGridMaze((BigBitGrid)grid.Clone(destination, leaveOpen));
    }
}
