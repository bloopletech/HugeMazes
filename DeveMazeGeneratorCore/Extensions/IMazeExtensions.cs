using System.Text;
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

    public static string GenerateMapAsString(this IMaze maze)
    {
        var stringBuilder = new StringBuilder();
        for(int y = 0; y < maze.Height; y++)
        {
            for(int x = 0; x < maze.Width; x++)
            {
                bool b = maze[x, y];
                if(b)
                {
                    stringBuilder.Append(' ');
                }
                else
                {
                    stringBuilder.Append('0');
                }
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }
}
