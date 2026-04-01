using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

public class MazePath
{
    public const short TypeId = 1;

    private readonly BitGrid grid;

    public MazePath(int width, int height) : this(new(width, height))
    {
    }

    private MazePath(BitGrid grid)
    {
        this.grid = grid;
    }

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

    public async Task Write(BinaryWriter writer)
    {
        writer.Write(TypeId);
        await grid.Write(writer);
    }

    public static async Task<MazePath> Read(BinaryReader reader)
    {
        return new MazePath(await BitGrid.Read(reader));
    }
}
