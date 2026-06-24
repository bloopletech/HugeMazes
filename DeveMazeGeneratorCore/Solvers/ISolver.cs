using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;

namespace DeveMazeGeneratorCore.Solvers;

public interface ISolver
{
    void Solve();

    public static ISolver Create(SolverType type, IMaze maze, IMazePath path) => type switch
    {
        SolverType.DepthFirstSmart => new DepthFirstSmartSolver(maze, path),
        _ => throw new InvalidDataException($"Unknown solver type {type}")
    };
}