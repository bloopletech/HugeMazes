using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.VSDiagnostics;

namespace DeveMazeGeneratorCore.Benchmark;

public class Config : ManualConfig
{
    public Config()
    {
        AddDiagnoser(new CPUUsageDiagnoser());
        AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()));
        //[InliningDiagnoser]
        //[TailCallDiagnoser]
        //[EtwProfiler]
        //[ConcurrencyVisualizerProfiler]
        //[NativeMemoryProfiler]
        //[ThreadingDiagnoser]
        AddJob(DeveJobAttribute.CreateJob(null, runtimeMoniker: RuntimeMoniker.Net11_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1));
        AddJob(DeveJobAttribute.CreateJob(null, runtimeMoniker: RuntimeMoniker.NativeAot11_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1));
        //[AsciiDocExporter]
        //[HtmlExporter]
        //[MarkdownExporterAttribute.GitHub]
        AddColumn(StatisticColumn.Min);
        AddColumn(StatisticColumn.Max);

        SummaryStyle = BenchmarkDotNet.Reports.SummaryStyle.Default.WithMaxParameterColumnWidth(200);
    }
}