using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

public static class MazeGenerator
{
    public static Maze Generate(int width, int height, int? seed = null)
    {
        return Generate(new AlgorithmBacktrack2Deluxe2_AsByte(), width, height, seed);
    }

    public static Maze Generate(IAlgorithm algorithm, int width, int height, int? seed = null)
    {
        var maze = new BitArreintjeFastInnerMap(width, height);
        var random = seed != null ? new Random(seed.Value) : new Random();

        algorithm.Generate(maze, random);

        return maze;
    }

    public static void Benchmark(IAlgorithm algorithm)
    {
        var size = 4096 * 2 * 2 * 2;
        var seed = 1337;
        Generate(algorithm, size, size, seed);
    }
}
