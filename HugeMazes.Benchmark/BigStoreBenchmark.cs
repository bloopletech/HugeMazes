#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using HugeMazes.IO;

namespace HugeMazes.Benchmark;

[Config(typeof(Config))]
public class BigStoreBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        IStore.LongOverride = false;
        HugeMazes.BenchmarkBaseline();
    }

    [Benchmark]
    public void GenerateFast()
    {
        IStore.LongOverride = false;
        HugeMazes.BenchmarkBaseline();
    }
}
