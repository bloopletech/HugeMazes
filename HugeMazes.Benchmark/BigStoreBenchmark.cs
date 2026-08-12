using BenchmarkDotNet.Attributes;
using HugeMazes.IO;

namespace HugeMazes.Benchmark;

[SimpleJob]
public class BigStoreBenchmark : BaseBenchmark
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
