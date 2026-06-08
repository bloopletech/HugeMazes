#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Benchmark;

[Config(typeof(Config))]
public class BigStoreBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        IStore.BigOverride = false;
        DeveMazeGeneratorCore.BenchmarkBaseline();
    }

    [Benchmark]
    public void GenerateFast()
    {
        IStore.BigOverride = false;
        DeveMazeGeneratorCore.BenchmarkBaseline();
    }
}
