#if !BLAZOR
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace DeveMazeGeneratorCore;

public static class Renderer
{
    public static Image<Argb32> Render(IMaze maze, RenderColors colors)
    {
        var image = new Image<Argb32>(maze.Width, maze.Height, colors.Wall);

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(maze[x, y])
                    {
                        ref var pixel = ref row[x];
                        pixel = colors.Background;
                    }
                }
            }
        });

        return image;
    }

    public static Image<Argb32> CreateImage(IMaze maze, IMazePath path, RenderColors colors)
    {
        if(path is IGridMazePath gridPath) return CreateImage(maze, gridPath, colors);
        else if(path is IPointsMazePath pointsPath) return CreateImage(maze, pointsPath, colors);
        throw new InvalidDataException($"Unknown path type");
    }

    public static Image<Argb32> CreateImage(IMaze maze, IGridMazePath path, RenderColors colors)
    {
        var image = Render(maze, colors);

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(path[x, y])
                    {
                        ref var pixel = ref row[x];
                        pixel = colors.Path;
                    }
                }
            }
        });

        if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
        if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

        return image;
    }

    public static Image<Argb32> CreatePlainImage(IMaze maze, IPointsMazePath path, RenderColors colors)
    {
        var image = Render(maze, colors);

        var points = path.Points;
        foreach(var point in points) image[point.X, point.Y] = colors.Path;
        if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
        if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

        return image;
    }

    public static Image<Argb32> CreateImage(IMaze maze, IPointsMazePath path, RenderColors colors)
    {
        var image = Render(maze, colors);

        var points = path.Points;
        for(var i = 0L; i < points.Count; i++)
        {
            var point = points[i];
            var shade = (byte)(i / (double)points.Count * 255.0);
            image[point.X, point.Y] = new Argb32(shade, (byte)(255 - shade), 0);
        }

        if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
        if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

        return image;
    }

    public static Image<Argb32> CreatePlainImageSorted(IMaze maze, IPointsMazePath path, RenderColors colors)
    {
        var image = Render(maze, colors);

        var sortedPoints = path.Points.ToArray();

        sortedPoints.Sort((first, second) =>
        {
            if(first.Y == second.Y) return first.X - second.X;
            return first.Y - second.Y;
        });

        var pointsIndex = 0;

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(pointsIndex >= sortedPoints.Length) return;

                    ref var point = ref sortedPoints[pointsIndex];
                    if(point.X == x && point.Y == y)
                    {
                        ref var pixel = ref row[x];
                        pixel = colors.Path;
                        pointsIndex++;
                    }
                }
            }
        });

        if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
        if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

        return image;
    }

    public static Image<Argb32> CreateImageSorted(IMaze maze, IPointsMazePath path, RenderColors colors)
    {
        var image = Render(maze, colors);

        var sortedPoints = path.Points.ToArray();

        sortedPoints.Sort((first, second) =>
        {
            if(first.Y == second.Y) return first.X - second.X;
            return first.Y - second.Y;
        });

        var pointsIndex = 0;

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(pointsIndex >= sortedPoints.Length) return;

                    ref var point = ref sortedPoints[pointsIndex];
                    if(point.X == x && point.Y == y)
                    {
                        ref var pixel = ref row[x];
                        var shade = (byte)(pointsIndex / (double)sortedPoints.Length * 255.0);
                        pixel = new Argb32(shade, (byte)(255 - shade), 0);
                        pointsIndex++;
                    }
                }
            }
        });

        if(colors.Start.HasValue) image[1, 1] = colors.Start.Value;
        if(colors.End.HasValue) image[maze.Width - 2, maze.Height - 2] = colors.End.Value;

        return image;
    }

    public static void Serialize(Stream stream, Image image)
    {
        image.Save(stream, new PngEncoder() { CompressionLevel = PngCompressionLevel.Level1 });
    }

    public static void Save(string fileName, Image image)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        Serialize(fs, image);
    }
}
#endif