using HugeMazes.Mazes;
using HugeMazes.Paths;

namespace HugeMazes.Solvers;

public interface ISolver
{
    void Solve();

    public static ISolver Create(SolverType type, IMaze maze, IMazePath path) => type switch
    {
        SolverType.Backtrack => new BacktrackSolver(maze, path),
        _ => throw new InvalidDataException($"Unknown solver type {type}")
    };
}