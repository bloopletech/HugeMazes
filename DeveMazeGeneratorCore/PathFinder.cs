using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
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
    public static MazePoint[] Find(IMaze maze)
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

        var prev = MazePoint.Empty;
        var lastBackTrackDir = -1;

        while(points.Count != 0)
        {
            var cur = points[^1];
            var (x, y) = cur;

            if(cur == end) break; //Path found

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
                points.Pop();

                if(points.Count == 0) break; //No path found

                var next = points[^1];

                //Set the direction we backtracked from
                if(x > next.X) lastBackTrackDir = 0;
                else if(y > next.Y) lastBackTrackDir = 1;
                else if(x < next.X) lastBackTrackDir = 2;
                else if(y < next.Y) lastBackTrackDir = 3;

                //Set the new previous point
                prev = points.Count == 1 ? MazePoint.Empty : points[^2];
            }
        }

        return [..points];
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

    public static void Find(IMaze maze, IGridMazePath path)
    {
        var points = Find(maze);
        foreach(var point in points) path[point.X, point.Y] = true;
    }

    /// <summary>
    /// Finds the path between the start and the endpoint in a maze
    /// </summary>
    /// <param name="maze">The maze.InnerMap</param>
    /// <returns>The shortest path in a list of points</returns>
    public static void Find(IMaze maze, IPointsMazePath path)
    {
        maze.EnsureMinimumSize();

        var points = path.Points;

        var start = new MazePoint(1, 1);
        var end = new MazePoint(maze.Width - 2, maze.Height - 2);

        var width = maze.Width;
        var height = maze.Height;
        
        points.Clear();
        points.Add(start);

        var prev = MazePoint.Empty;
        var lastBackTrackDir = -1;

        while(points.Count != 0)
        {
            var cur = points[points.Count - 1];
            var (x, y) = cur;

            if(cur == end) break; //Path found

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
                points.Pop();

                if(points.Count == 0) break; //No path found

                var next = points[points.Count - 1];

                //Set the direction we backtracked from
                if(x > next.X) lastBackTrackDir = 0;
                else if(y > next.Y) lastBackTrackDir = 1;
                else if(x < next.X) lastBackTrackDir = 2;
                else if(y < next.Y) lastBackTrackDir = 3;

                //Set the new previous point
                prev = points.Count == 1 ? MazePoint.Empty : points[points.Count - 2];
            }
        }
    }
}
