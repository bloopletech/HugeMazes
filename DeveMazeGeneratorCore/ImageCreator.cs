#if !BLAZOR
using System.Diagnostics;
using DeveMazeGeneratorCore.Mazes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace DeveMazeGeneratorCore;

public static class ImageCreator
{
    public static Image<Argb32> CreateImage(IMaze maze)
    {
        var w = Stopwatch.StartNew();

        var image = new Image<Argb32>(maze.Width, maze.Height, Color.Black);

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
                        pixel = Color.White;
                    }
                }
            }
        });

        w.Stop();
        Debug.WriteLine($"Maze image creation time: {w.Elapsed}");

        return image;
    }

    public static Image<Argb32> CreateImage(IMaze maze, MazePath path)
    {
        var image = CreateImage(maze);

        var w = Stopwatch.StartNew();

        //points.Sort((first, second) =>
        //{
        //    if(first.Y == second.Y)
        //    {
        //        return first.X - second.X;
        //    }
        //    return first.Y - second.Y;
        //});

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
                        pixel = Color.Lime;
                    }
                }
            }
        });

        w.Stop();
        Debug.WriteLine($"Solution image creation time: {w.Elapsed}");

        return image;
    }

    //public static Image<Argb32> CreateImage(IMaze maze, Solution solution)
    //{
    //    var points = solution.Points;

    //    var roundedUpWidth = MathHelper.RoundUpToNextEven(maze.Width);
    //    var roundedUpHeight = MathHelper.RoundUpToNextEven(maze.Height);

    //    points.Sort((first, second) =>
    //    {
    //        if(first.Y == second.Y)
    //        {
    //            return first.X - second.X;
    //        }
    //        return first.Y - second.Y;
    //    });


    //    int curpos = 0;

    //    var w = Stopwatch.StartNew();
    //    var image = new Image<Argb32>(roundedUpWidth - 1, roundedUpHeight - 1);

    //    for(int y = 0; y < roundedUpHeight - 1; y++)
    //    {
    //        for(int x = 0; x < roundedUpWidth - 1; x++)
    //        {
    //            int r = 0;
    //            int g = 0;
    //            int b = 0;

    //            MazePointPos curPathPos;
    //            if(curpos < points.Count)
    //            {
    //                curPathPos = points[curpos];
    //                if(curPathPos.X == x && curPathPos.Y == y)
    //                {
    //                    r = curPathPos.RelativePos;
    //                    g = 255 - curPathPos.RelativePos;
    //                    b = 0;
    //                    curpos++;
    //                }
    //                else if(maze[x, y])
    //                {
    //                    r = 255;
    //                    g = 255;
    //                    b = 255;
    //                }
    //            }
    //            else if(maze[x, y])
    //            {
    //                r = 255;
    //                g = 255;
    //                b = 255;
    //            }
    //            image[x, y] = new Argb32((byte)r, (byte)g, (byte)b);
    //        }
    //        //lineSavingProgress(y, this.Height - 2);
    //    }

    //    w.Stop();
    //    Debug.WriteLine($"First image conversion time: {w.Elapsed}");

    //    return image;
    //}

    public static async Task Serialize(Stream stream, Image image)
    {
        await image.SaveAsync(stream, new PngEncoder() { CompressionLevel = PngCompressionLevel.Level1 });
    }

    public static async Task Save(string fileName, Image image)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Serialize(fs, image);
    }
}
#endif