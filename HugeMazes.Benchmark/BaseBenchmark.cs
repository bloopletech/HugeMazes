using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
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
//[DisassemblyDiagnoser(10, printSource: true, exportHtml: true, exportCombinedDisassemblyReport: true)]
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
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core11_0).WithEnvironmentVariables([
                new("DOTNET_TieredCompilation", "0"),
                new("DOTNET_TC_QuickJit", "0"),
                new("DOTNET_TC_QuickJitForLoops", "0")
            ]));

            AddJob(Job.Default.WithRuntime(NativeAotRuntime.Net11_0));

            SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);
        }
    }
}