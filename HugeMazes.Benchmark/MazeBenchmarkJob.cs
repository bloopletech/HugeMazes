#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;

namespace HugeMazes.Benchmark;

[SimpleJob]
public class MazeBenchmarkJob : BaseBenchmark
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
