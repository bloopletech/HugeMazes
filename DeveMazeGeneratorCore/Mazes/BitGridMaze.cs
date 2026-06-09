using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public sealed class BitGridMaze(BitGrid grid) : IMaze
{
    public BitGridMaze(IStore store, bool leaveOpen = false) : this(new BitGrid(store, leaveOpen))
    {
    }

    public BitGridMaze(IStore store, Size size, bool leaveOpen = false) : this(new BitGrid(store, size, leaveOpen))
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

    public void Read() => grid.Read();

    public void Write() => grid.Write();

    public void Dispose() => grid.Dispose();

    public IMaze Clone() => Clone(IStore.Create(IsLong));

    public IMaze Clone(IStore destination, bool leaveOpen = false)
    {
        return new BitGridMaze(grid.Clone(destination, leaveOpen));
    }
}