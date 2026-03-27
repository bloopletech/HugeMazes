#pragma warning disable CA1822 // Mark members as static

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using DeveMazeGeneratorCore.Algorithms;
using DeveMazeGeneratorCore.Mazes;
using Microsoft.VSDiagnostics;

namespace DeveMazeGeneratorCore.Benchmark;

[CPUUsageDiagnoser]
[MemoryDiagnoser]
//[InliningDiagnoser]
//[TailCallDiagnoser]
//[EtwProfiler]
//[ConcurrencyVisualizerProfiler]
//[NativeMemoryProfiler]
//[ThreadingDiagnoser]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
//[
//    //DeveJob(RuntimeMoniker.Net60, launchCount: 1, warmupCount: 4, targetCount: 50, invocationCount: 1),
//    //DeveJob(RuntimeMoniker.Net70, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
//    //DeveJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
//    //DeveJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
//    DeveJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
//]
//[AsciiDocExporter]
//[HtmlExporter]
//[MarkdownExporterAttribute.GitHub]
[MinColumn, MaxColumn]
[Config(typeof(Config))]
public class MazeBenchmarkJob
{
    [Benchmark]
    public void GenerateBaseline()
    {
        DeveMazeGeneratorCore.BenchmarkBaseline();
    }

    //[Benchmark]
    public void GenerateFast()
    {
        DeveMazeGeneratorCore.BenchmarkFast();
    }

    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);
        }
    }
}
