using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static IMaze Generate(int width, int height, int? seed = null) => Generate(
        IStore.Create(IMaze.MaxExtent(width, height)),
        width,
        height,
        seed);

    public static IMaze Generate(IStore store, int width, int height, int? seed = null) => Generate(
        MazeSerializer.DetermineMazeType(new(width, height)),
        AlgorithmType.Backtrack,
        store,
        width,
        height,
        seed);

    public static IMaze Generate(
        MazeType mazeType,
        AlgorithmType algorithmType,
        int width,
        int height,
        int? seed = null) => Generate(
        mazeType,
        algorithmType,
        IStore.Create(IMaze.MaxExtent(width, height)),
        width,
        height,
        seed);

    public static IMaze Generate(
        MazeType mazeType,
        AlgorithmType algorithmType,
        IStore store,
        int width,
        int height,
        int? seed = null)
    {
        var maze = MazeSerializer.Create(mazeType, store, new(width, height));
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        var algorithm = IAlgorithm.Create(algorithmType, maze, random);
        algorithm.Generate();

        return maze;
    }

    public static IMazePath Solve(IMaze maze) => Solve(maze, IStore.Create(maze.IsLong));

    //public static IMazePath Solve(IMaze maze, IStore store) => Solve(maze, MazePathType.MazePath, store);
    public static IMazePath Solve(IMaze maze, IStore store) => Solve(maze, MazePathType.DirectionMazePath, store);

    public static IMazePath Solve(IMaze maze, MazePathType pathType, IStore store)
    {
        var path = MazePathSerializer.Create(pathType, store, maze.Size);
        Solver.Solve(maze, path);
        return path;
    }

    public static IImage Render(
        IMaze maze,
        RenderColours? colours = null) => Render(maze, IStore.Create(maze.IsLong), colours);

    //public static IImage Render(IMaze maze, IStore store, RenderColors? colours = null) => Render(maze, ImageType.LongImage, store, colours);

    //public static IImage Render(IMaze maze, ImageType imageType, IStore store, RenderColors? colours = null)
    public static IImage Render(
        IMaze maze,
        IStore store,
        RenderColours? colours = null) => Renderer.Render(maze, store, colours ?? RenderColours.Default);

    public static IImage Render(
        IMaze maze,
        IMazePath path,
        RenderColours? colours = null) => Render(maze, path, IStore.Create(maze.IsLong), colours);

    //public static IImage Render(IMaze maze, IMazePath path, IStore store, RenderColors? colours = null) => Render(maze, path, ImageType.LongImage, store, colours);

    //public static IImage Render(IMaze maze, IMazePath path, ImageType imageType, IStore store, RenderColors? colours = null)
    public static IImage Render(
        IMaze maze,
        IMazePath path,
        IStore store,
        RenderColours? colours = null) => Renderer.Render(maze, path, store, colours ?? RenderColours.Default);

    public static IMaze BenchmarkBaseline()
    {
        return Generate(MazeType.BitGridMaze, AlgorithmType.Backtrack, BenchmarkSize, BenchmarkSize, BenchmarkSeed);
    }

    public static IMaze BenchmarkFast()
    {
        return Generate(
            MazeType.BitGridMaze,
            AlgorithmType.Backtrack2_Deluxe2_AsByte,
            BenchmarkSize,
            BenchmarkSize,
            BenchmarkSeed);
    }

    public static IMaze BenchmarkBigBitGrid()
    {
        return Generate(
            MazeType.BitGridMaze,
            AlgorithmType.Backtrack,
            IStore.Create(true),
            BenchmarkSize,
            BenchmarkSize,
            BenchmarkSeed);
    }

    private const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    private const int BenchmarkSeed = 1337;
}
