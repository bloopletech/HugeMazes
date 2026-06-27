using HugeMazes.Extensions;
using HugeMazes.Generators;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Solvers;
using HugeMazes.Structures;

namespace HugeMazes;

public static class HugeMazes
{
    public static IStore Create(string fileName) => new StreamStore(File.Open(fileName, FileMode.CreateNew));
    public static IStore Open(string fileName) => new StreamStore(File.Open(fileName, FileMode.Open));

    public static IMaze Generate(
        IStore store,
        int width,
        int height,
        int? seed = null,
        MazeType mazeType = MazeType.LongBitGridMaze,
        GeneratorType generatorType = GeneratorType.Backtrack)
    {
        var maze = MazeSerializer.Create(store, mazeType, new(width, height));
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        var generator = IGenerator.Create(generatorType, maze, random);
        generator.Generate();

        return maze;
    }

    public static IMazePath Solve(
        IStore store,
        IMaze maze,
        MazePathType pathType = MazePathType.DirectionMazePath,
        SolverType solverType = SolverType.Backtrack)
    {
        var path = MazePathSerializer.Create(store, pathType);

        var solver = ISolver.Create(solverType, maze, path);
        solver.Solve();

        return path;
    }

    public static IImage Render(
        IStore store,
        IMaze maze,
        //ImageType imageType = ImageType.LongImage,
        RenderColours? colours = null)
    {
        colours ??= RenderColours.Default;
        return Renderer.Render(store, maze, colours.Value);
    }

    public static IImage Render(
        IStore store,
        IMaze maze,
        IMazePath path,
        //ImageType imageType = ImageType.LongImage,
        RenderColours? colours = null)
    {
        colours ??= RenderColours.Default;
        return Renderer.Render(store, maze, path, colours.Value);
    }

    public static IMaze BenchmarkBaseline() => Generate(
        IStore.Create(false),
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        MazeType.BitGridMaze,
        GeneratorType.Backtrack);

    public static IMaze BenchmarkFast() => Generate(
        IStore.Create(false),
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        MazeType.BitGridMaze,
        GeneratorType.Backtrack2_Deluxe2_AsByte);

    public static IMaze BenchmarkLongBitGrid() => Generate(
        IStore.Create(true),
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        MazeType.LongBitGridMaze,
        GeneratorType.Backtrack);

    public const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    public const int BenchmarkSeed = 1337;
}
