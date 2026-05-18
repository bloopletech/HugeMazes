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
    public static IMazePath Find(IMaze maze)
    {
        maze.EnsureMinimumSize();

        var start = new MazePoint(1, 1);
        var end = new MazePoint(maze.Width - 2, maze.Height - 2);

        var width = maze.Width;
        var height = maze.Height;

        var path = new MazePath(width, height);
        path[start.X, start.Y] = true;

        var corners = new List<MazePoint>();

        Span<MazePoint> entrances = stackalloc MazePoint[4];
        entrances.Clear();

        MazePoint cur = start;
        MazePoint prev = new(-1, -1);
        MazePoint prevPrev = new(-1, -1);
        MazePoint next = new(-1, -1);

        var lastBackTrackDir = -1;

        while(true)
        {
            Console.WriteLine($"cur: {cur}, prev: {prev}, prevPrev: {prevPrev}, next: {next}, lastBackTrackDir: {lastBackTrackDir}");
            var (x, y) = cur;

            if(x == end.X && y == end.Y)
            {
                Console.WriteLine("Path traced");
                //Path found
                break;
            }

            var entranceCount = 0;
            if(x - 1 > 0 && maze[x - 1, y])
            {
                entrances[entranceCount].Set(x - 1, y);
                entranceCount++;
            }
            if(x + 1 < width - 1 && maze[x + 1, y])
            {
                entrances[entranceCount].Set(x + 1, y);
                entranceCount++;
            }
            if(y - 1 > 0 && maze[x, y - 1])
            {
                entrances[entranceCount].Set(x, y - 1);
                entranceCount++;
            }
            if(y + 1 < height - 1 && maze[x, y + 1])
            {
                entrances[entranceCount].Set(x, y + 1);
                entranceCount++;
            }

            if(entranceCount > 2 && corners[^1] != cur) corners.Add(cur);


            //Make sure the point was not the previous point, also make sure that if we backtracked we don't go to a direction we already went to, also make sure that the point is white
            if((prev.X != x + 1 || prev.Y != y) && lastBackTrackDir < 0 && x + 1 < width - 1 && maze[x + 1, y])
            {
                next = new(x + 1, y);
                path[next.X, next.Y] = true;
                lastBackTrackDir = -1;
                prevPrev = prev;
                prev = cur;
                cur = next;
            }
            else if((prev.X != x || prev.Y != y + 1) && lastBackTrackDir < 1 && y + 1 < height - 1 && maze[x, y + 1])
            {
                next = new(x, y + 1);
                path[next.X, next.Y] = true;
                lastBackTrackDir = -1;
                prevPrev = prev;
                prev = cur;
                cur = next;
            }
            else if((prev.X != x - 1 || prev.Y != y) && lastBackTrackDir < 2 && x - 1 > 0 && maze[x - 1, y])
            {
                next = new(x - 1, y);
                path[next.X, next.Y] = true;
                lastBackTrackDir = -1;
                prevPrev = prev;
                prev = cur;
                cur = next;
            }
            else if((prev.X != x || prev.Y != y - 1) && lastBackTrackDir < 3 && y - 1 > 0 && maze[x, y - 1])
            {
                next = new(x, y - 1);
                path[next.X, next.Y] = true;
                lastBackTrackDir = -1;
                prevPrev = prev;
                prev = cur;
                cur = next;
            }
            else
            {
                if(x == -1 && y == -1)
                {
                    Console.WriteLine("No path can be traced");
                    //No path found
                    break;
                }

                path[x, y] = false;




                //Set the direction we backtracked from
                if(cur.X > prev.X)
                {
                    lastBackTrackDir = 0;
                }
                else if(cur.Y > prev.Y)
                {
                    lastBackTrackDir = 1;
                }
                else if(cur.X < prev.X)
                {
                    lastBackTrackDir = 2;
                }
                else if(cur.Y < prev.Y)
                {
                    lastBackTrackDir = 3;
                }

                //Set the new previous point
                prev = prevPrev;
            }
        }

        return path;
    }
}
