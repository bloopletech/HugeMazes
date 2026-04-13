using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Paths;

public class MazePath : IMazePath
{
    public const short TypeId = 1;

    private readonly BitGrid grid;

    public MazePath(int width, int height) : this(new BitGrid(width, height))
    {
    }

    public MazePath(MazePath source) : this(new BitGrid(source.grid))
    {
    }

    private MazePath(BitGrid grid)
    {
        this.grid = grid;
    }

    public IMazePath Clone() => new MazePath(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public void Highlight()
    {
        //var points = new MazePointPos[stack.Count];

        //foreach(var item in stack)
        //{
        //    byte formulathing = (byte)(points.Count / (double)stack.Count * 255.0);
        //    points.Add(new MazePointPos(item.X, item.Y, formulathing));
        //}
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(TypeId);
        grid.Write(writer);
    }

    public async Task WriteAsync(BinaryWriter writer)
    {
        writer.Write(TypeId);
        await grid.WriteAsync(writer);
    }

    public static IMazePath Read(BinaryReader reader)
    {
        return new MazePath(BitGrid.Read(reader));
    }

    public static async Task<IMazePath> ReadAsync(BinaryReader reader)
    {
        return new MazePath(await BitGrid.ReadAsync(reader));
    }
}
