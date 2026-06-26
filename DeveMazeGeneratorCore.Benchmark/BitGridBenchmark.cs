#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Benchmark;

[Config(typeof(Config))]
public class BitGridBenchmark
{
    [Benchmark(Baseline = true)]
    public void GenerateBaseline()
    {
        var maze = DeveMazeGeneratorCore.Generate(
            IStore.Create(false),
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSeed,
            MazeType.BitGridMaze,
            GeneratorType.Backtrack);
        Verifier.IsPerfectMaze(maze);
    }

    [Benchmark]
    public void GenerateLong()
    {
        var maze = DeveMazeGeneratorCore.Generate(
            IStore.Create(true),
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSize,
            DeveMazeGeneratorCore.BenchmarkSeed,
            MazeType.LongBitGridMaze,
            GeneratorType.Backtrack);
        Verifier.IsPerfectMaze(maze);
    }
}
