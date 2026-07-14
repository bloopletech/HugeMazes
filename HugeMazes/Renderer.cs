using HugeMazes.Images;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Structures;

namespace HugeMazes;

public static class Renderer
{
    public static IIndexedImage Render(IStore destination, IMaze maze, RenderPalette palette)
    {
        var image = new LongIndexedTiffImage(destination, maze.Size, palette.Indexed.Palette);

        for(int y = 0; y < image.Height; y++)
        {
            for(int x = 0; x < image.Width; x++)
            {
                image[x, y] = maze[x, y] ? RenderPalette.Index.Background : RenderPalette.Index.Wall;
            }
        }

        if(palette.Start.HasValue) image[1, 1] = RenderPalette.Index.Start;
        if(palette.End.HasValue) image[maze.Width - 2, maze.Height - 2] = RenderPalette.Index.End;

        return image;
    }

    public static IImage<MazeColor> RenderShaded(IStore destination, IMaze maze, RenderPalette palette)
    {
        var image = new LongTiffImage(destination, maze.Size);

        for(int y = 0; y < image.Height; y++)
        {
            for(int x = 0; x < image.Width; x++)
            {
                image[x, y] = maze[x, y] ? palette.Background : palette.Wall;
            }
        }

        if(palette.Start.HasValue) image[1, 1] = palette.Start.Value;
        if(palette.End.HasValue) image[maze.Width - 2, maze.Height - 2] = palette.End.Value;

        return image;
    }

    public static IImage Render(IStore destination, IMaze maze, IMazePath path, RenderPalette palette, bool plain)
    {
        return plain ? Render(destination, maze, path, palette) : RenderShaded(destination, maze, path, palette);
    }

    public static IIndexedImage Render(IStore destination, IMaze maze, IMazePath path, RenderPalette palette)
    {
        var image = Render(destination, maze, palette);

        foreach(var point in path) image[point.X, point.Y] = RenderPalette.Index.Path;
        if(palette.Start.HasValue) image[1, 1] = RenderPalette.Index.Start;
        if(palette.End.HasValue) image[maze.Width - 2, maze.Height - 2] = RenderPalette.Index.End;

        return image;
    }

    public static IImage<MazeColor> RenderShaded(IStore destination, IMaze maze, IMazePath path, RenderPalette palette)
    {
        var image = RenderShaded(destination, maze, palette);

        var i = 0;
        foreach(var point in path)
        {
            var shade = (byte)(i / (double)path.Count * 255.0);
            image[point.X, point.Y] = new MazeColor(shade, (byte)(255 - shade), 0);
            i++;
        }

        if(palette.Start.HasValue) image[1, 1] = palette.Start.Value;
        if(palette.End.HasValue) image[maze.Width - 2, maze.Height - 2] = palette.End.Value;

        return image;
    }
}
