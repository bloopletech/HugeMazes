using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGridMaze(BigBitGrid grid) : IMaze
{
    public BigBitGridMaze(IBinarySerializer serializer, long offset) : this(new BigBitGrid(serializer, offset))
    {
    }

    public BigBitGridMaze(
        IBinarySerializer serializer,
        long offset,
        int width,
        int height) : this(new BigBitGrid(serializer, offset, width, height))
    {
    }

    public MazeType Type => MazeType.BigBitGridMaze;
    public IBinarySerializer Serializer => grid.Serializer;
    public long Offset => grid.Offset;
    public int Width => grid.Width;
    public int Height => grid.Height;

    public IMaze Clone() => new BigBitGridMaze((BigBitGrid)grid.Clone());

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public void Read()
    {
    }

    public async Task ReadAsync()
    {
    }

    public void Write()
    {
        //RandomAccess.Write(handle, ref offset, (ushort)MazeType.BitGridMaze);
        grid.Write();
    }

    public async Task WriteAsync()
    {
        await grid.WriteAsync();
    }
}
