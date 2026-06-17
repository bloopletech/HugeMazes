#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;

namespace DeveMazeGeneratorCore.Benchmark;

[Config(typeof(Config))]
public class MazePathBenchmark
{
    private IMaze maze = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        maze = DeveMazeGeneratorCore.BenchmarkBaseline();
    }

    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        var path = MazePathSerializer.Create(MazePathType.MazePath, IStore.Create(maze.IsLong), maze.Size);
        Solver.Solve(maze, path);
    }

    [Benchmark]
    public void GenerateDirection()
    {
        var path = MazePathSerializer.Create(MazePathType.DirectionMazePath, IStore.Create(maze.IsLong), maze.Size);
        Solver.Solve(maze, path);
    }
}
