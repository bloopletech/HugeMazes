using DeveMazeGeneratorCore.InnerMaps;

namespace DeveMazeGeneratorCore;

public static class MazeGenerator
{
    public static Maze Generate(int width, int height, int? seed = null)
    {
        var maze = new BitArreintjeFastInnerMap(width, height);
        var random = seed != null ? new Random(seed.Value) : new Random();

        var alg = new AlgorithmBacktrack(maze, random);
        alg.Generate();
        return maze;
    }
}
