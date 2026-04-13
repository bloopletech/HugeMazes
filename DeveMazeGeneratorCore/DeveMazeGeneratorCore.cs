using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static IMaze Generate(int width, int height, int? seed = null)
    {
        var maze = new BitGridMaze(width, height);
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static IMaze BenchmarkBaseline()
    {
        var maze = new BitGridMaze(BenchmarkSize, BenchmarkSize);
        var random = new Random(BenchmarkSeed);
        var realSeed = random.GetSeed();

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static IMaze BenchmarkFast()
    {
        var maze = new BitGridMaze(BenchmarkSize, BenchmarkSize);
        var random = new Random(BenchmarkSeed);
        var realSeed = random.GetSeed();

        var algorithm = new AlgorithmBacktrack2Deluxe2_AsByte(maze, random);
        algorithm.Generate();

        return maze;
    }

    private const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    private const int BenchmarkSeed = 1337;
}
