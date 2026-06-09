using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public sealed class BitGridMaze(BitGrid grid) : IMaze
{
    public BitGridMaze(IStore store, bool leaveOpen = false) : this(new BitGrid(store, leaveOpen))
    {
    }

    public BitGridMaze(
        IStore store,
        int width,
        int height,
        bool leaveOpen = false) : this(new BitGrid(store, width, height, leaveOpen))
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

    public async Task ReadAsync() => Read();

    public void Write() => grid.Write();

    public async Task WriteAsync() => Write();

    public void Dispose() => grid.Dispose();

    public IMaze Clone() => Clone(IStore.Create(IsLong));

    public IMaze Clone(IStore destination, bool leaveOpen = false)
    {
        return new BitGridMaze((BitGrid)grid.Clone(destination, leaveOpen));
    }
}
