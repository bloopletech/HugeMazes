using BenchmarkDotNet.Running;
using DeveMazeGeneratorCore.Benchmark;

Console.WriteLine("Running the Benchmark job");

//var config = DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(200));
//BenchmarkRunner.Run<MazeBenchmarkJob>();
//BenchmarkRunner.Run<BigStoreBenchmark>();
BenchmarkRunner.Run<BitGridBenchmark>();