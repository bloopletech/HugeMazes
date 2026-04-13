using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGridMaze : IMaze
{
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

    public void Write(BinaryWriter writer)
    {
        WriteHeader(writer);
        grid.Write(writer);
    }

    public async Task WriteAsync(BinaryWriter writer)
    {
        WriteHeader(writer);
        await grid.WriteAsync(writer);
    }

    private void WriteHeader(BinaryWriter writer)
    {
        writer.Write((ushort)MazeType.BitGridMaze);
        writer.Write(Width);
        writer.Write(Height);
    }

    public static BitGridMaze Read(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var grid = BitGrid.Read(reader);
        return new BitGridMaze(width, height, grid);
    }

    public static async Task<BitGridMaze> ReadAsync(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var grid = await BitGrid.ReadAsync(reader);
        return new BitGridMaze(width, height, grid);
    }
}
