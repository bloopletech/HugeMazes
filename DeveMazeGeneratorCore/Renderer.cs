using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public static class Renderer
{
    public static IImage Render(IMaze maze, IStore destination, RenderColours colours)
    {
        var image = new LongImage(destination, maze.Size);
        foreach(var (x, y) in image.ByPixel()) image[x, y] = maze[x, y] ? colours.Background : colours.Wall;

        if(colours.Start.HasValue) image[1, 1] = colours.Start.Value;
        if(colours.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colours.End.Value;

        return image;
    }

    public static IImage RenderPlain(IMaze maze, IMazePath path, IStore destination, RenderColours colours)
    {
        var image = Render(maze, destination, colours);

        foreach(var point in path) image[point.X, point.Y] = colours.Path;
        if(colours.Start.HasValue) image[1, 1] = colours.Start.Value;
        if(colours.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colours.End.Value;

        return image;
    }

    public static IImage Render(IMaze maze, IMazePath path, IStore destination, RenderColours colours)
    {
        var image = Render(maze, destination, colours);

        for(var i = 0L; i < path.Count; i++)
        {
            var point = path[i];
            var shade = (byte)(i / (double)path.Count * 255.0);
            image[point.X, point.Y] = new Colour(shade, (byte)(255 - shade), 0);
        }

        if(colours.Start.HasValue) image[1, 1] = colours.Start.Value;
        if(colours.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colours.End.Value;

        return image;
    }

    //public static Image<Argb32> CreatePlainImageSorted(IMaze maze, IPointsMazePath path, RenderColors colors)
    //{
    //    var image = Render(maze, colors);

    //    var sortedPoints = path.Points.ToArray();

    //    sortedPoints.Sort((first, second) =>
    //    {
    //        if(first.Y == second.Y) return first.X - second.X;
    //        return first.Y - second.Y;
    //    });

    //    var pointsIndex = 0;

    //    image.ProcessPixelRows(rows =>
    //    {
    //        for(int y = 0; y < rows.Height; y++)
    //        {
    //            var row = rows.GetRowSpan(y);
    //            for(int x = 0; x < row.Length; x++)
    //            {
    //                if(pointsIndex >= sortedPoints.Length) return;

    //                ref var point = ref sortedPoints[pointsIndex];
    //                if(point.X == x && point.Y == y)
    //                {
    //                    ref var pixel = ref row[x];
    //                    pixel = colors.Path;
    //                    pointsIndex++;
    //                }
    //            }
    //        }
    //    });

    //    if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
    //    if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

    //    return image;
    //}

    //public static Image<Argb32> CreateImageSorted(IMaze maze, IPointsMazePath path, RenderColors colors)
    //{
    //    var image = Render(maze, colors);

    //    var sortedPoints = path.Points.ToArray();

    //    sortedPoints.Sort((first, second) =>
    //    {
    //        if(first.Y == second.Y) return first.X - second.X;
    //        return first.Y - second.Y;
    //    });

    //    var pointsIndex = 0;

    //    image.ProcessPixelRows(rows =>
    //    {
    //        for(int y = 0; y < rows.Height; y++)
    //        {
    //            var row = rows.GetRowSpan(y);
    //            for(int x = 0; x < row.Length; x++)
    //            {
    //                if(pointsIndex >= sortedPoints.Length) return;

    //                ref var point = ref sortedPoints[pointsIndex];
    //                if(point.X == x && point.Y == y)
    //                {
    //                    ref var pixel = ref row[x];
    //                    var shade = (byte)(pointsIndex / (double)sortedPoints.Length * 255.0);
    //                    pixel = new Argb32(shade, (byte)(255 - shade), 0);
    //                    pointsIndex++;
    //                }
    //            }
    //        }
    //    });

    //    if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
    //    if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

    //    return image;
    //}
}
