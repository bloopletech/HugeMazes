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
        DeveMazeGeneratorCore.Solve(IStore.Create(false), maze, MazePathType.MazePath);
    }

    [Benchmark]
    public void GenerateDirection()
    {
        DeveMazeGeneratorCore.Solve(IStore.Create(false), maze, MazePathType.DirectionMazePath);
    }
}
