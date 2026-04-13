using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Generators;

public interface IAlgorithm
{
    void Generate();

    public static IAlgorithm Create(AlgorithmType type, IMaze maze, Random random) => type switch
    {
        AlgorithmType.Backtrack => new AlgorithmBacktrack(maze, random),
        AlgorithmType.Backtrack2_Deluxe2_AsByte => new AlgorithmBacktrack2Deluxe2_AsByte(maze, random),
        _ => throw new InvalidDataException($"Unknown algorithm type {type}")
    };
}
