using BenchmarkDotNet.Running;

namespace DeveMazeGeneratorCore.Benchmark;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Running the Benchmark job");

        //var config = DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(200));
        BenchmarkRunner.Run<MazeBenchmarkJob>();
    }
}