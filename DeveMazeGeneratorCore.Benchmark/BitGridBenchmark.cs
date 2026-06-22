#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Benchmark;

[Config(typeof(Config))]
public class BitGridBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        var maze = DeveMazeGeneratorCore.Generate(
            MazeType.BitGridMaze,
            AlgorithmType.Backtrack,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSeed);
        Verifier.IsPerfectMaze(maze);
    }

    [Benchmark]
    public void GenerateLong()
    {
        var maze = DeveMazeGeneratorCore.Generate(
            MazeType.LongBitGridMaze,
            AlgorithmType.Backtrack,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSeed);
        Verifier.IsPerfectMaze(maze);
    }
}
