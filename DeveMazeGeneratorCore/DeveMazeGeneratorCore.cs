using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static IMaze Generate(int width, int height, int? seed = null) => Generate(
        MazeType.BitGridMaze,
        AlgorithmType.Backtrack,
        width,
        height,
        seed);

    public static IMaze Generate(MazeType mazeType, AlgorithmType algorithmType, int width, int height, int? seed = null)
    {
        var maze = IMaze.Create(mazeType, width, height);
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        var algorithm = IAlgorithm.Create(algorithmType, maze, random);
        algorithm.Generate();

        return maze;
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
