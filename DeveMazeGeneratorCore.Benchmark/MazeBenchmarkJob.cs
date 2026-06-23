#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;

namespace DeveMazeGeneratorCore.Benchmark;

[Use<BaseBenchmark>]
[Use<SimpleJobSource>]
public class MazeBenchmarkJob
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        DeveMazeGeneratorCore.BenchmarkBaseline();
    }

    [Benchmark]
    public void GenerateFast()
    {
        DeveMazeGeneratorCore.BenchmarkFast();
    }
}
