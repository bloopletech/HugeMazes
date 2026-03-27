using DeveMazeGeneratorCore.Helpers;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public class Verifier
{
    public static bool IsPerfectMaze(Maze maze)
    {
        var copiedMaze = maze.Clone();

        FloodFill(copiedMaze);

        //Make uneven because a maze actually is this size and not an even number
        var unevenHeight = MathHelper.MakeUneven(copiedMaze.Height);
        var unevenWidth = MathHelper.MakeUneven(copiedMaze.Width);

        for(int y = 0; y < unevenHeight; y++)
        {
            for(int x = 0; x < unevenWidth; x++)
            {
                if(copiedMaze[x, y] == false)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static void FloodFill(Maze maze)
    {
        var stack = new Stack<MazePoint>();
        stack.Push(new(0, 0));

        int x;
        int y;

        int width = maze.Width;
        int height = maze.Height;

        while(stack.Count != 0)
        {
            var cur = stack.Pop();
            x = cur.X;
            y = cur.Y;

            maze[x, y] = true;

            if(x - 1 > 0 && !maze[x - 1, y]) stack.Push(new(x - 1, y));
            if(x + 1 < width - 1 && !maze[x + 1, y]) stack.Push(new(x + 1, y));
            if(y - 1 > 0 && !maze[x, y - 1]) stack.Push(new(x, y - 1));
            if(y + 1 < height - 1 && !maze[x, y + 1]) stack.Push(new(x, y + 1));
        }
    }
}
