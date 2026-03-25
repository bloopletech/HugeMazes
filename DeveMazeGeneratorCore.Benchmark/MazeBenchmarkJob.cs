using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using DeveMazeGeneratorCore.InnerMaps;

namespace DeveMazeGeneratorCore.Benchmark;

[MemoryDiagnoser]
//[InliningDiagnoser]
//[TailCallDiagnoser]
//[EtwProfiler]
//[ConcurrencyVisualizerProfiler]
//[NativeMemoryProfiler]
//[ThreadingDiagnoser]
[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
[
    //DeveJob(RuntimeMoniker.Net60, launchCount: 1, warmupCount: 4, targetCount: 50, invocationCount: 1),
    //DeveJob(RuntimeMoniker.Net70, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
    //DeveJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
    //DeveJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
    DeveJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
]
[AsciiDocExporter]
[HtmlExporter]
[MarkdownExporterAttribute.GitHub]
[MinColumn, MaxColumn]
[Config(typeof(Config))]
public class MazeBenchmarkJob
{
    private const int SIZE = 4096 * 2 * 2;
    private const int SEED = 1337;

    [Benchmark]
    public void Simple()
    {
        var map = new BitArreintjeFastInnerMap(SIZE, SIZE);
        var random = new Random(SEED);
        var algorithm = new AlgorithmBacktrack2Deluxe2_AsByte(map, random);
        algorithm.Generate();
    }

    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);
        }
    }
}
