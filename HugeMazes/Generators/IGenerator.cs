using HugeMazes.Mazes;

namespace HugeMazes.Generators;

public interface IGenerator
{
    void Generate();

    public static IGenerator Create(GeneratorType type, IMaze maze, Random random) => type switch
    {
        GeneratorType.Backtrack => new BacktrackGenerator(maze, random),
        GeneratorType.Backtrack2_Deluxe2_AsByte => new Backtrack2Deluxe2_AsByteGenerator(maze, random),
        _ => throw new InvalidDataException($"Unknown generator type {type}")
    };
}
