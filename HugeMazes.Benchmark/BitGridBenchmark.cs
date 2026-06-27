#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using HugeMazes.Generators;
using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Benchmark;

[Config(typeof(Config))]
public class BitGridBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        var maze = HugeMazes.Generate(
            IStore.Create(false),
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSeed,
            MazeType.BitGridMaze,
            GeneratorType.Backtrack);
        Verifier.IsPerfectMaze(maze);
    }

    [Benchmark]
    public void GenerateLong()
    {
        var maze = HugeMazes.Generate(
            IStore.Create(true),
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSeed,
            MazeType.LongBitGridMaze,
            GeneratorType.Backtrack);
        Verifier.IsPerfectMaze(maze);
    }
}
