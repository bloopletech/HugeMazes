#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;

namespace HugeMazes.Benchmark;

[Config(typeof(Config))]
public class MazePathBenchmark
{
    private IMaze maze = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        maze = HugeMazes.BenchmarkBaseline();
    }

    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        HugeMazes.Solve(IStore.Create(false), maze, MazePathType.MazePath);
    }

    [Benchmark]
    public void GenerateDirection()
    {
        HugeMazes.Solve(IStore.Create(false), maze, MazePathType.DirectionMazePath);
    }
}
