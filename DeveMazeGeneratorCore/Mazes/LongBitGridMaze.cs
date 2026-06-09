using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public sealed class LongBitGridMaze(LongBitGrid grid) : IMaze
{
    public LongBitGridMaze(IStore store, bool leaveOpen = false) : this(new LongBitGrid(store, leaveOpen))
    {
    }

    public LongBitGridMaze(
        IStore store,
        int width,
        int height,
        bool leaveOpen = false) : this(new LongBitGrid(store, width, height, leaveOpen))
    {
    }

    public IStore Store => grid.Store;
    public bool IsLong => grid.IsLong;
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

    public IMaze Clone() => Clone(IStore.Create(IsLong));

    public IMaze Clone(IStore destination, bool leaveOpen = false)
    {
        return new LongBitGridMaze((LongBitGrid)grid.Clone(destination, leaveOpen));
    }
}
