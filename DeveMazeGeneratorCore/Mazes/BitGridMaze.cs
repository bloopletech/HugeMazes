using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGridMaze : IMaze
{
    public const short TypeId = 1;

    private readonly int width;
    private readonly int height;
    private readonly BitGrid grid;

    public BitGridMaze(int width, int height) : this(width, height, new(width, height))
    {
    }

    public BitGridMaze(BitGridMaze source) : this(source.Width, source.Height, new(source.grid))
    {
    }

    private BitGridMaze(int width, int height, BitGrid grid)
    {
        if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
        if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");

        this.width = width;
        this.height = height;
        this.grid = grid;
    }

    public int Width => width;
    public int Height => height;

    public IMaze Clone() => new BitGridMaze(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public async Task Write(BinaryWriter writer)
    {
        writer.Write(TypeId);
        writer.Write(Width);
        writer.Write(Height);
        await grid.Write(writer);
    }

    public static async Task<BitGridMaze> Read(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var grid = await BitGrid.Read(reader);
        return new BitGridMaze(width, height, grid);
    }
}
