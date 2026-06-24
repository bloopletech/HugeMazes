#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Benchmark;

//[Use<BaseBenchmark>]
[Use<SimpleJobSource>]
public class BitGridBenchmark : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        var maze = DeveMazeGeneratorCore.Generate(
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSeed,
            null,
            MazeType.BitGridMaze,
            GeneratorType.Backtrack);
        Verifier.IsPerfectMaze(maze);
    }

    [Benchmark]
    public void GenerateLong()
    {
        var maze = DeveMazeGeneratorCore.Generate(
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSeed,
            null,
            MazeType.LongBitGridMaze,
            GeneratorType.Backtrack);
        Verifier.IsPerfectMaze(maze);
    }
}
