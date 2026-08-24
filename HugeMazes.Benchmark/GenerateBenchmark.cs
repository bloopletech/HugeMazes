using BenchmarkDotNet.Attributes;
using HugeMazes.Generators;
using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Benchmark;

public class GenerateBenchmark : BaseBenchmark
{
    [ParamsAllValues]
    public GeneratorType GeneratorType { get; set; }// = GeneratorType.Backtrack;

    [Benchmark]
    public void Generate()
    {
        using var maze = HugeMazes.Generate(
            IStore.Create(),
            Guid.NewGuid(),
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSize,
            HugeMazes.BenchmarkSeed,
            MazeType.Maze,
            GeneratorType);
    }
}