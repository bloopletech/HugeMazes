#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using HugeMazes.Generators;
using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Benchmark;

//[Config(typeof(DebugBuildConfig))]
[Use<DryJobs>]
public class BitGridBenchmark : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        HugeMazes.Generate(
            IStore.Create(false),
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSeed,
            MazeType.BitGridMaze,
            GeneratorType.Backtrack);
    }

    [Benchmark]
    public void GenerateLong()
    {
        HugeMazes.Generate(
            IStore.Create(false),
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSeed,
            MazeType.LongBitGridMaze,
            GeneratorType.Backtrack);
    }

    [Benchmark]
    public void GenerateJagged()
    {
        HugeMazes.Generate(
            IStore.Create(false),
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSeed,
            MazeType.JaggedBitGridMaze,
            GeneratorType.Backtrack);
    }
}
