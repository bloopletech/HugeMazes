using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
//using Microsoft.VSDiagnostics;

namespace DeveMazeGeneratorCore.Benchmark;

//[CPUUsageDiagnoser]
[MemoryDiagnoser]
[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[ConcurrencyVisualizerProfiler]
//[NativeMemoryProfiler]
//[ThreadingDiagnoser]
[DisassemblyDiagnoser(printSource: true)]
[ExceptionDiagnoser]

//[AsciiDocExporter]
//[HtmlExporter]
//[MarkdownExporterAttribute.GitHub]
[MinColumn, MaxColumn]
[Config(typeof(Config))]
public abstract class BaseBenchmark
{
    public class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);
        }
    }
}

[
    DeveJob(RuntimeMoniker.Net11_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
    DeveJob(RuntimeMoniker.NativeAot11_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1)
]
public abstract class FullJobSource
{
}

[SimpleJob]
public abstract class SimpleJobSource
{
}