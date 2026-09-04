using HugeMazes.Mazes;
using HugeMazes.Paths;

namespace HugeMazes.Solvers;

public class BacktrackSolver(IMaze maze, IMazePath path) : ISolver
{
    public void Solve()
    {
        maze.EnsureMinimumSize();

        var start = MazePoint.Start;
        var end = new MazePoint(maze.Width - 2, maze.Height - 2);

        var width = maze.Width - 1;
        var height = maze.Height - 1;

        path.Clear();
        path.Add(start);

        var prev = MazePoint.Empty;
        var lastBackTrackDir = -1;

        while(path.Count != 0)
        {
            var cur = path.Peek();
            var (x, y) = cur;
            var (px, py) = prev;

            if(cur == end) break; //Path found

            //Make sure the point was not the previous point, also make sure that if we backtracked we don't go to a direction we already went to, also make sure that the point is white
            if((px != x + 1 || py != y) && lastBackTrackDir < 0 && x + 1 < width && maze[x + 1, y])
            {
                path.Add(new(x + 1, y));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((px != x || py != y + 1) && lastBackTrackDir < 1 && y + 1 < height && maze[x, y + 1])
            {
                path.Add(new(x, y + 1));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((px != x - 1 || py != y) && lastBackTrackDir < 2 && x - 1 > 0 && maze[x - 1, y])
            {
                path.Add(new(x - 1, y));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((px != x || py != y - 1) && lastBackTrackDir < 3 && y - 1 > 0 && maze[x, y - 1])
            {
                path.Add(new(x, y - 1));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else
            {
                path.PopIgnore();

                if(path.Count == 0) break; //No path found

                var (nx, ny) = path.Peek();

                //Set the direction we backtracked from
                if(x > nx) lastBackTrackDir = 0;
                else if(y > ny) lastBackTrackDir = 1;
                else if(x < nx) lastBackTrackDir = 2;
                else if(y < ny) lastBackTrackDir = 3;

                //Set the new previous point
                prev = path.Count == 1 ? MazePoint.Empty : path[path.Count - 2];
            }
        }
    }
}
