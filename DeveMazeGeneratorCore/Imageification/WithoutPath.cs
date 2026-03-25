using DeveMazeGeneratorCore.Helpers;

#if !BLAZOR
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace DeveMazeGeneratorCore.Imageification;

public class WithoutPath
{
    public static void MazeToImage(Maze maze, Stream stream)
    {
        var roundedUpWidth = MathHelper.RoundUpToNextEven(maze.Width);
        var roundedUpHeight = MathHelper.RoundUpToNextEven(maze.Height);

        var image = new Image<Argb32>(roundedUpWidth - 1, roundedUpHeight - 1);

        var blackPixel = Color.Black;
        var whitePixel = Color.White;

        for (int y = 0; y < roundedUpHeight - 1; y++)
        {
            for (int x = 0; x < roundedUpWidth - 1; x++)
            {
                var wall = maze[x, y];

                if (!wall)
                {
                    image[x, y] = blackPixel;
                }
                else
                {
                    image[x, y] = whitePixel;
                }
            }
        }

        image.Save(stream, new PngEncoder());
    }

    public static void SaveMaze(string fileName, Maze maze)
    {
        var dir = "Images";
        Directory.CreateDirectory(dir);
        var totalFile = Path.Combine(dir, fileName);

        using (var fs = new FileStream(totalFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            MazeToImage(maze, fs);
        }
    }
}
#endif