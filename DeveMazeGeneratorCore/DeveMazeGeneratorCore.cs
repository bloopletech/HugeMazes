using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Solvers;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static IMaze Generate(
        int width,
        int height,
        int? seed = null,
        IStore? store = null,
        MazeType mazeType = MazeType.LongBitGridMaze,
        GeneratorType generatorType = GeneratorType.Backtrack)
    {
        store ??= IStore.Create(IMaze.MaxExtent(width, height));

        var maze = MazeSerializer.Create(mazeType, store, new(width, height));
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        var generator = IGenerator.Create(generatorType, maze, random);
        generator.Generate();

        return maze;
    }

    public static IMazePath Solve(
        IMaze maze,
        IStore? store = null,
        MazePathType pathType = MazePathType.DirectionMazePath,
        SolverType solverType = SolverType.DepthFirstSmart)
    {
        store ??= IStore.Create(maze.IsLong);

        var path = MazePathSerializer.Create(pathType, store, maze.Size);

        var solver = ISolver.Create(solverType, maze, path);
        solver.Solve();

        return path;
    }

    public static IImage Render(
        IMaze maze,
        IStore? store = null,
        //ImageType imageType = ImageType.LongImage,
        RenderColours? colours = null)
    {
        store ??= IStore.Create(maze.IsLong);
        colours ??= RenderColours.Default;
        return Renderer.Render(maze, store, colours.Value);
    }

    public static IImage Render(
        IMaze maze,
        IMazePath path,
        IStore? store = null,
        //ImageType imageType = ImageType.LongImage,
        RenderColours? colours = null)
    {
        store ??= IStore.Create(maze.IsLong);
        colours ??= RenderColours.Default;
        return Renderer.Render(maze, path, store, colours.Value);
    }

    public static IMaze BenchmarkBaseline() => Generate(
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        null,
        MazeType.BitGridMaze,
        GeneratorType.Backtrack);

    public static IMaze BenchmarkFast() => Generate(
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        null,
        MazeType.BitGridMaze,
        GeneratorType.Backtrack2_Deluxe2_AsByte);

    public static IMaze BenchmarkLongBitGrid() => Generate(
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        IStore.Create(true),
        MazeType.BitGridMaze,
        GeneratorType.Backtrack);

    private const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    private const int BenchmarkSeed = 1337;
}
