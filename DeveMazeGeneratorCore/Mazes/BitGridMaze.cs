using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGridMaze : IMaze
{
    private readonly BitGrid grid;

    public BitGridMaze(IBinarySerializer serializer)
    {
        grid = new BitGrid(serializer);
    }

    public BitGridMaze(IBinarySerializer serializer, int width, int height)
    {
        grid = new BitGrid(serializer, width, height);
    }

    //public BitGridMaze(BitGridMaze source) : this(new MemoryStream(), source.Width, source.Height, new(source.grid))
    //{
    //}

    //private BitGridMaze(Stream stream, int width, int height, BitGrid grid)
    //{
    //    if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
    //    if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");

    //    this.stream = stream;
    //    this.width = width;
    //    this.height = height;
    //    this.grid = grid;
    //}

    public IBinarySerializer Serializer => serializer;
    public int Width => grid.Width;
    public int Height => grid.Height;

    //public IMaze Clone() => new BitGridMaze(this);
    public IMaze Clone() => throw new NotImplementedException();

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
        serializer.EnsureCompleted();
    }

    public async Task ReadAsync()
    {
        await grid.ReadAsync();
        serializer.EnsureCompleted();
    }

    public void Write()
    {
        serializer.Write(width);
        serializer.Write(height);
        grid.Write();
    }

    public async Task WriteAsync()
    {
        serializer.Write(width);
        serializer.Write(height);
        await grid.WriteAsync();
    }
}
