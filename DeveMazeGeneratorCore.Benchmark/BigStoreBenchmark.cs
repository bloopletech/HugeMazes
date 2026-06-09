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
        IStore.LongOverride = false;
        DeveMazeGeneratorCore.BenchmarkBaseline();
    }

    [Benchmark]
    public void GenerateFast()
    {
        IStore.LongOverride = false;
        DeveMazeGeneratorCore.BenchmarkBaseline();
    }
}
