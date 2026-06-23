using BenchmarkDotNet.Attributes;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;

namespace DeveMazeGeneratorCore.Benchmark;

[Use<BaseBenchmark>]
[Use<SimpleJobSource>]
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
        DeveMazeGeneratorCore.Solve(maze, null, MazePathType.MazePath);
    }

    [Benchmark]
    public void GenerateDirection()
    {
        DeveMazeGeneratorCore.Solve(maze, null, MazePathType.DirectionMazePath);
    }
}
