using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

/// <summary>
/// This class specifically ads a position to the path it returns which can be used to more efficiently save the maze path later
/// </summary>
public static class PathFinder
{
    /// <summary>
    /// Finds the path between the start and the endpoint in a maze
    /// </summary>
    /// <param name="maze">The maze.InnerMap</param>
    /// <returns>The shortest path in a list of points</returns>
    public static MazePath Find(IMaze maze)
    {
        maze.EnsureMinimumSize();

        var start = new MazePoint(1, 1);
        var end = new MazePoint(maze.Width - 2, maze.Height - 2);

        var width = maze.Width;
        var height = maze.Height;

        var points = new List<MazePoint>()
        {
            start
        };

        MazePoint cur;
        MazePoint prev = new(-1, -1);

        var lastBackTrackDir = -1;

        while(points.Count != 0)
        {
            cur = points[^1];
            var (x, y) = cur;

            if(x == end.X && y == end.Y)
            {
                //Path found
                break;
            }

            //Make sure the point was not the previous point, also make sure that if we backtracked we don't go to a direction we already went to, also make sure that the point is white
            if((prev.X != x + 1 || prev.Y != y) && lastBackTrackDir < 0 && x + 1 < width - 1 && maze[x + 1, y])
            {
                points.Add(new(x + 1, y));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((prev.X != x || prev.Y != y + 1) && lastBackTrackDir < 1 && y + 1 < height - 1 && maze[x, y + 1])
            {
                points.Add(new(x, y + 1));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((prev.X != x - 1 || prev.Y != y) && lastBackTrackDir < 2 && x - 1 > 0 && maze[x - 1, y])
            {
                points.Add(new(x - 1, y));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else if((prev.X != x || prev.Y != y - 1) && lastBackTrackDir < 3 && y - 1 > 0 && maze[x, y - 1])
            {
                points.Add(new(x, y - 1));
                lastBackTrackDir = -1;
                prev = cur;
            }
            else
            {
                var prepoppy = points[^1];
                points.RemoveAt(points.Count - 1);

                if(points.Count == 0)
                {
                    //No path found
                    break;
                }

                var newcur = points[^1];

                //Set the new previous point
                if(points.Count == 1)
                {
                    prev = new MazePoint(-1, -1);
                }
                else
                {
                    prev = points.ElementAt(points.Count - 2);
                }

                //Set the direction we backtracked from
                if(prepoppy.X > newcur.X)
                {
                    lastBackTrackDir = 0;
                }
                else if(prepoppy.Y > newcur.Y)
                {
                    lastBackTrackDir = 1;
                }
                else if(prepoppy.X < newcur.X)
                {
                    lastBackTrackDir = 2;
                }
                else if(prepoppy.Y < newcur.Y)
                {
                    lastBackTrackDir = 3;
                }
            }
        }

        var path = new MazePath(width, height);
        foreach(var point in points) path[point.X, point.Y] = true;
        return path;
    }
}
