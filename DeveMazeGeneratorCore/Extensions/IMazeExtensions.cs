using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Extensions;

public static class IMazeExtensions
{
    public static void EnsureMinimumSize(this IMaze maze)
    {
        if(maze.Width < 3) throw new ArgumentOutOfRangeException("maze.Width", maze.Width, "Value must >= 3");
        if(maze.Height < 3) throw new ArgumentOutOfRangeException("maze.Height", maze.Height, "Value must >= 3");
    }

    public static void EnsureOddSize(this IMaze maze)
    {
        if(int.IsEvenInteger(maze.Width)) throw new ArgumentException("Value must be an odd number", "maze.Width");
        if(int.IsEvenInteger(maze.Height)) throw new ArgumentException("Value must be an odd number", "maze.Height");
    }
}
