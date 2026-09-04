using HugeMazes.Extensions;
using HugeMazes.Generators;
using HugeMazes.Images;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Solvers;

namespace HugeMazes;

public static class HugeMazes
{
    public static IStore Create(string fileName) => IStore.Create(fileName);
    public static IStore Open(string fileName) => IStore.Open(fileName);

    public static IMaze Load(IStore store) => MazeSerializer.Read(store);
    public static IMazePath LoadPath(IStore store) => MazePathSerializer.Read(store);

    public static IMaze Generate(
        IStore store,
        Guid id,
        int width,
        int height,
        int? seed = null,
        MazeType mazeType = MazeType.Maze,
        GeneratorType generatorType = GeneratorType.Backtrack)
    {
        var maze = MazeSerializer.Create(store, mazeType, id, new(width, height));
        var random = new Random();
        if(seed.HasValue) random.SetSeed(seed.Value);
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
        var path = MazePathSerializer.Create(store, pathType, maze.Id);

        var solver = ISolver.Create(solverType, maze, path);
        solver.Solve();

        return path;
    }

    public static IImage Render(
        IStore store,
        IMaze maze,
        //ImageType imageType = ImageType.LongImage,
        RenderPalette? colours = null)
    {
        colours ??= RenderPalette.Default;
        return Renderer.Render(store, maze, colours.Value);
    }

    public static IImage Render(
        IStore store,
        IMaze maze,
        IMazePath path,
        //ImageType imageType = ImageType.LongImage,
        RenderPalette? colours = null,
        bool plain = true)
    {
        colours ??= RenderPalette.Default;
        IMazePath.EnsureRelated(maze, path);
        return Renderer.Render(store, maze, path, colours.Value, plain);
    }

    public static IMaze BenchmarkBaseline() => Generate(
        IStore.Create(),
        Guid.NewGuid(),
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        MazeType.Maze,
        GeneratorType.Backtrack);

    public static IMaze BenchmarkFast() => Generate(
        IStore.Create(),
        Guid.NewGuid(),
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        MazeType.Maze,
        GeneratorType.Backtrack2_Deluxe2_AsByte);

    public const int BenchmarkSize = 8192 + 1;
    public const int BenchmarkSeed = 1337;
}
