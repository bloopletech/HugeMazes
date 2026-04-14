using DeveMazeGeneratorCore.Collections;
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
        BinarySerializer.CreateFile(),
        width,
        height,
        seed);

    public static IMaze Generate(IBinarySerializer serializer, int width, int height, int? seed = null) => Generate(
        DetermineMazeType(width, height),
        AlgorithmType.Backtrack,
        serializer,
        width,
        height,
        seed);

    public static IMaze Generate(MazeType mazeType, AlgorithmType algorithmType, int width, int height, int? seed = null) => Generate(
        mazeType,
        algorithmType,
        BinarySerializer.CreateFile(),
        width,
        height,
        seed);

    public static IMaze Generate(
        MazeType mazeType,
        AlgorithmType algorithmType,
        IBinarySerializer serializer,
        int width,
        int height,
        int? seed = null)
    {
        var maze = MazeSerializer.Create(mazeType, serializer, width, height);
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        using var stack = BigList<MazePoint>.CreateFile();
        var algorithm = IAlgorithm.Create(algorithmType, maze, stack, random);
        algorithm.Generate();

        return maze;
    }

    public static MazeType DetermineMazeType(int width, int height)
    {
        var size = (long)width * height;
        var byteSize = size.DivCeil(8);
        return byteSize > int.MaxValue ? MazeType.BigBitGridMaze : MazeType.BitGridMaze;
    }

    public static IMazePath Solve(IMaze maze) => Solve(maze, BinarySerializer.CreateFile());

    public static IMazePath Solve(IMaze maze, IBinarySerializer serializer) => Solve(
        maze,
        MazePathType.MazePath,
        serializer);

    public static IMazePath Solve(IMaze maze, MazePathType pathType, IBinarySerializer serializer)
    {
        var path = MazePathSerializer.Create(pathType, serializer, maze.Width, maze.Height);
        if(path is IGridMazePath gridPath) PathFinder.Find(maze, gridPath);
        else if(path is IPointsMazePath pointsPath) PathFinder.Find(maze, pointsPath);
        return path;
    }

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

    private const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    private const int BenchmarkSeed = 1337;
}
