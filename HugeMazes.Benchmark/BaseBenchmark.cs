using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Reports;
//using Microsoft.VSDiagnostics;

namespace HugeMazes.Benchmark;

//[CPUUsageDiagnoser]
[MemoryDiagnoser]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[ConcurrencyVisualizerProfiler]
//[NativeMemoryProfiler]
//[ThreadingDiagnoser]
[DisassemblyDiagnoser(10, printSource: true, exportHtml: true, exportCombinedDisassemblyReport: true)]
[ExceptionDiagnoser]
[HtmlExporter]
[MinColumn, MaxColumn]
[Config(typeof(Config))]
public abstract class BaseBenchmark
{
    public class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);
            WithOptions(ConfigOptions.JoinSummary);
        }
    }
}