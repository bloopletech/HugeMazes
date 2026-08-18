using BenchmarkDotNet.Attributes;
using HugeMazes.Benchmark.Support;

namespace HugeMazes.Benchmark;

[Use<SimpleJobs>]
public class GenerateBenchmark : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public void Generate()
    {
        HugeMazes.BenchmarkBaseline();
    }

    //[Benchmark]
    //public void GenerateFast()
    //{
    //    HugeMazes.BenchmarkFast();
    //}
}