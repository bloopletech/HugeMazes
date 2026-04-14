using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Generators;

public interface IAlgorithm
{
    void Generate();

    public static IAlgorithm Create(AlgorithmType type, IMaze maze, IBigList<MazePoint> stack, Random random) => type switch
    {
        AlgorithmType.Backtrack => new AlgorithmBacktrack(maze, stack, random),
        AlgorithmType.Backtrack2_Deluxe2_AsByte => new AlgorithmBacktrack2Deluxe2_AsByte(maze, stack, random),
        _ => throw new InvalidDataException($"Unknown algorithm type {type}")
    };
}
