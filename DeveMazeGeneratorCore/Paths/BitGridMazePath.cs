using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Paths;

public class BitGridMazePath(BitGrid grid) : IGridMazePath
{
    public BitGridMazePath(IBinarySerializer serializer, long offset) : this(new BitGrid(serializer, offset))
    {
    }

    public BitGridMazePath(
        IBinarySerializer serializer,
        long offset,
        int width,
        int height) : this(new BitGrid(serializer, offset, width, height))
    {
    }

    public IBinarySerializer Serializer => grid.Serializer;
    public long Offset => grid.Offset;
    public int Width => grid.Width;
    public int Height => grid.Height;

    public IGridMazePath Clone() => new BitGridMazePath((BitGrid)grid.Clone());

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public void Read()
    {
        grid.Read();
        Serializer.EnsureCompleted();
    }

    public async Task ReadAsync()
    {
        Read();
    }

    public void Write()
    {
        grid.Write();
    }

    public async Task WriteAsync()
    {
        Write();
    }
}
