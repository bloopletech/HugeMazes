using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Structures;

namespace HugeMazes;

public class Verifier
{
    public static bool IsPerfectMaze(IMaze maze)
    {
        using var copiedMaze = maze.Clone();

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
        using var stack = new LongList<MazePoint>(IStore.Create(maze.IsLong));
        stack.Clear();
        stack.Push(new(0, 0));

        int width = maze.Width - 1;
        int height = maze.Height - 1;

        while(stack.Count != 0)
        {
            var cur = stack.Pop();
            var (x, y) = cur;

            maze[x, y] = true;

            if(x > 0 && !maze[x - 1, y]) stack.Push(new(x - 1, y));
            if(x < width && !maze[x + 1, y]) stack.Push(new(x + 1, y));
            if(y > 0 && !maze[x, y - 1]) stack.Push(new(x, y - 1));
            if(y < height && !maze[x, y + 1]) stack.Push(new(x, y + 1));
        }
    }

    public static bool IsPerfectPath(IMaze maze, IMazePath path)
    {
        MazePoint last = MazePoint.Empty;

        foreach(var point in path)
        {
            if(last == MazePoint.Empty)
            {
                if(point != MazePoint.Start) return false;
            }
            else
            {
                var xDelta = Math.Abs(point.X - last.X);
                var yDelta = Math.Abs(point.Y - last.Y);
                if(xDelta != 1 && yDelta != 1) return false;
            }

            last = point;
        }

        if(last == MazePoint.Empty) return false;
        if(last.X != maze.Width - 2 || last.Y != maze.Height - 2) return false;
        return true;
    }
}
