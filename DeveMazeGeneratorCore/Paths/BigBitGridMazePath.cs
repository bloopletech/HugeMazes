using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Paths;

public class BigBitGridMazePath(BigBitGrid grid) : IGridMazePath
{
    public BigBitGridMazePath(IBinarySerializer serializer, long offset) : this(new BigBitGrid(serializer, offset))
    {
    }

    public BigBitGridMazePath(
        IBinarySerializer serializer,
        long offset,
        int width,
        int height) : this(new BigBitGrid(serializer, offset, width, height))
    {
    }

    public MazePathType Type => MazePathType.BigBitGridMazePath;
    public IBinarySerializer Serializer => grid.Serializer;
    public long Offset => grid.Offset;
    public int Width => grid.Width;
    public int Height => grid.Height;

    public IGridMazePath Clone() => new BigBitGridMazePath((BigBitGrid)grid.Clone());

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
    }

    public async Task ReadAsync()
    {
        await grid.ReadAsync();
    }

    public void Write()
    {
        //RandomAccess.Write(handle, ref offset, (ushort)MazePathType.BitGridMazePath);
        grid.Write();
    }

    public async Task WriteAsync()
    {
        await grid.WriteAsync();
    }
}
