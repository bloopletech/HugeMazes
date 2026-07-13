using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace HugeMazes.Benchmark;

[
    DeveJob(RuntimeMoniker.Net11_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1),
    DeveJob(RuntimeMoniker.NativeAot11_0, launchCount: 1, warmupCount: 4, targetCount: 10, invocationCount: 1)
]
public abstract class FullJobSource
{
}

[SimpleJob(iterationCount: 1)]
public abstract class FastJobSource
{
}


[DryJob(RuntimeMoniker.Net11_0)]
[DryJob(RuntimeMoniker.NativeAot11_0)]
public abstract class DryJobs
{
}