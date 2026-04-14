using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

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

    public void Write(Stream stream)
    {
        WriteHeader(stream);
        grid.Write(stream);
    }

    public async Task WriteAsync(Stream stream)
    {
        WriteHeader(stream);
        await grid.WriteAsync(stream);
    }

    private void WriteHeader(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write((ushort)MazeType.BitGridMaze);
        writer.Write(Width);
        writer.Write(Height);
    }

    public static BitGridMaze Read(Stream stream)
    {
        using var reader = stream.Reader();
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var grid = BitGrid.Read(stream);
        return new BitGridMaze(width, height, grid);
    }

    public static async Task<BitGridMaze> ReadAsync(Stream stream)
    {
        using var reader = stream.Reader();
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var grid = await BitGrid.ReadAsync(stream);
        return new BitGridMaze(width, height, grid);
    }
}
