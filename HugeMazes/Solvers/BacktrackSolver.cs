using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Structures;

namespace HugeMazes.Solvers;

public class BacktrackSolver(IMaze maze, IMazePath path) : ISolver
{
    public void Solve()
    {
        maze.EnsureMinimumSize();

        var start = new MazePoint(1, 1);
        var end = new MazePoint(maze.Width - 2, maze.Height - 2);

        var width = maze.Width;
        var height = maze.Height;

        path.Clear();
        path.Add(start);

        var prev = MazePoint.Empty;
        var lastBackTrackDir = -1;

        while(path.Count != 0)
        {
            var cur = path.Peek();
            var (x, y) = cur;

            if(cur == end) break; //Path found

            //Make sure the point was not the previous point, also make sure that if we backtracked we don't go to a direction we already went to, also make sure that the point is white
            if((prev.X != x + 1 || prev.Y != y) && lastBackTrackDir < 0 && x + 1 < width - 1 && maze[x + 1, y])
            {
                path.Add(new(x + 1, y));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((prev.X != x || prev.Y != y + 1) && lastBackTrackDir < 1 && y + 1 < height - 1 && maze[x, y + 1])
            {
                path.Add(new(x, y + 1));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((prev.X != x - 1 || prev.Y != y) && lastBackTrackDir < 2 && x - 1 > 0 && maze[x - 1, y])
            {
                path.Add(new(x - 1, y));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((prev.X != x || prev.Y != y - 1) && lastBackTrackDir < 3 && y - 1 > 0 && maze[x, y - 1])
            {
                path.Add(new(x, y - 1));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else
            {
                path.Pop();

                if(path.Count == 0) break; //No path found

                var next = path.Peek();

                //Set the direction we backtracked from
                if(x > next.X) lastBackTrackDir = 0;
                else if(y > next.Y) lastBackTrackDir = 1;
                else if(x < next.X) lastBackTrackDir = 2;
                else if(y < next.Y) lastBackTrackDir = 3;

                //Set the new previous point
                prev = path.Count == 1 ? MazePoint.Empty : path[path.Count - 2];
            }
        }
    }

    public static MazePointPos[] WithPos(MazePoint[] points)
    {
        var pointsPos = new MazePointPos[points.Length];

        for(var i = 0; i < points.Length; i++)
        {
            ref var point = ref points[i];
            var shade = (byte)(i / (double)points.Length * 255.0);
            pointsPos[i] = new MazePointPos(point.X, point.Y, shade);
        }

        return pointsPos;
    }
}
