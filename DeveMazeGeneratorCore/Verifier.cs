using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public class Verifier
{
    public static bool IsPerfectMaze(IMaze maze)
    {
        var copiedMaze = maze.Clone();

        FloodFill(copiedMaze);

        for(int y = 0; y < copiedMaze.Height; y++)
        {
            for(int x = 0; x < copiedMaze.Width; x++)
            {
                if(!copiedMaze[x, y]) return false;
            }
        }

        return true;
    }

    public static void FloodFill(IMaze maze)
    {
        var stack = new Stack<MazePoint>();
        stack.Push(new(0, 0));

        int width = maze.Width - 1;
        int height = maze.Height - 1;

        while(stack.Count != 0)
        {
            var cur = stack.Pop();
            var x = cur.X;
            var y = cur.Y;

            maze[x, y] = true;

            if(x > 0 && !maze[x - 1, y]) stack.Push(new(x - 1, y));
            if(x < width && !maze[x + 1, y]) stack.Push(new(x + 1, y));
            if(y > 0 && !maze[x, y - 1]) stack.Push(new(x, y - 1));
            if(y < height && !maze[x, y + 1]) stack.Push(new(x, y + 1));
        }
    }
}
