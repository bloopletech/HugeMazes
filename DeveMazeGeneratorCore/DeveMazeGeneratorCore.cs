using DeveMazeGeneratorCore.Algorithms;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static Maze Generate(int width, int height, int? seed = null)
    {
        var maze = new ContiguousArrayMaze(width, height);
        var random = seed != null ? new Random(seed.Value) : new Random();

        var algorithm = new AlgorithmBacktrack2Deluxe2_AsByte(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static Maze BenchmarkBaseline()
    {
        var maze = new ContiguousArrayMaze(BenchmarkSize, BenchmarkSize);
        var random = new Random(BenchmarkSeed);

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static Maze BenchmarkFast()
    {
        var maze = new ContiguousArrayMaze(BenchmarkSize, BenchmarkSize);
        var random = new Random(BenchmarkSeed);

        var algorithm = new AlgorithmBacktrack2Deluxe2_AsByte(maze, random);
        algorithm.Generate();

        return maze;
    }

    private const int BenchmarkSize = 4096 * 2 * 2 * 2;
    private const int BenchmarkSeed = 1337;
}
