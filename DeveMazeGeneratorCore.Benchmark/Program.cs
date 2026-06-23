using System.Diagnostics;
using BenchmarkDotNet.Running;

//BenchmarkSwitcher.FromTypes(BaseBenchmark.GetTypes()).Run(args);
var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
var summary = summaries.FirstOrDefault();
if(summary != null) Process.StartAndForget(summary.ResultsDirectoryPath);
