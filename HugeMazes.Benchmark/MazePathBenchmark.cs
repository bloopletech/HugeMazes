using BenchmarkDotNet.Attributes;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;

namespace HugeMazes.Benchmark;

[SimpleJob]
public class MazePathBenchmark : BaseBenchmark
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
        HugeMazes.Solve(IStore.Create(), maze, MazePathType.MazePath);
    }

    [Benchmark]
    public void GenerateDirection()
    {
        HugeMazes.Solve(IStore.Create(), maze, MazePathType.DirectionMazePath);
    }
}
