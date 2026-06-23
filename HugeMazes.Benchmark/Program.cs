using System.Diagnostics;
using BenchmarkDotNet.Running;
using HugeMazes.Benchmark;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, NoExportersDefaultConfig.Instance);

var resultsPath = Path.Join(AppContext.BaseDirectory, "BenchmarkDotNet.Artifacts", "results");

Process.Start(new ProcessStartInfo
{
    FileName = resultsPath,
    UseShellExecute = true,
});