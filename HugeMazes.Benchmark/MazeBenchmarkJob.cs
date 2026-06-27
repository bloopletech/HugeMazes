#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;

namespace HugeMazes.Benchmark;

[Config(typeof(Config))]
public class MazeBenchmarkJob
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        HugeMazes.BenchmarkBaseline();
    }

    [Benchmark]
    public void GenerateFast()
    {
        HugeMazes.BenchmarkFast();
    }
}
