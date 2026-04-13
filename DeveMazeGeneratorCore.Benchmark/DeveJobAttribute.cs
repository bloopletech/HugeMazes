using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace DeveMazeGeneratorCore.Benchmark;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
public class DeveJobAttribute : JobConfigBaseAttribute
{
    public DeveJobAttribute(
        int launchCount = -1,
        int warmupCount = -1,
        int targetCount = -1,
        int invocationCount = -1,
        string? id = null,
        bool baseline = false,
        Jit jit = Jit.Default,
        Platform platform = Platform.AnyCpu,
        bool disableTieredCompilation = true)
        : base(CreateJob(
            id,
            launchCount,
            warmupCount,
            targetCount,
            invocationCount,
            null,
            baseline,
            jit,
            platform,
            disableTieredCompilation))
    {
    }

    public DeveJobAttribute(
        RunStrategy runStrategy,
        int launchCount = -1,
        int warmupCount = -1,
        int targetCount = -1,
        int invocationCount = -1,
        string? id = null,
        bool baseline = false,
        Jit jit = Jit.Default,
        Platform platform = Platform.AnyCpu,
        bool disableTieredCompilation = true)
        : base(CreateJob(
            id,
            launchCount,
            warmupCount,
            targetCount,
            invocationCount,
            runStrategy,
            baseline,
            jit,
            platform,
            disableTieredCompilation))
    {
    }

    public DeveJobAttribute(
        RuntimeMoniker runtimeMoniker,
        int launchCount = -1,
        int warmupCount = -1,
        int targetCount = -1,
        int invocationCount = -1,
        string? id = null,
        bool baseline = false,
        Jit jit = Jit.Default,
        Platform platform = Platform.AnyCpu,
        bool disableTieredCompilation = true)
        : base(CreateJob(
            id,
            launchCount,
            warmupCount,
            targetCount,
            invocationCount,
            null,
            baseline,
            jit,
            platform,
            disableTieredCompilation,
            runtimeMoniker))
    {
    }

    public DeveJobAttribute(
        RunStrategy runStrategy,
        RuntimeMoniker runtimeMoniker,
        int launchCount = -1,
        int warmupCount = -1,
        int targetCount = -1,
        int invocationCount = -1,
        string? id = null,
        bool baseline = false,
        Jit jit = Jit.Default,
        Platform platform = Platform.AnyCpu,
        bool disableTieredCompilation = true)
        : base(CreateJob(
            id,
            launchCount,
            warmupCount,
            targetCount,
            invocationCount,
            runStrategy,
            baseline,
            jit,
            platform,
            disableTieredCompilation,
            runtimeMoniker))
    {
    }

    private static Job CreateJob(
        string? id,
        int launchCount,
        int warmupCount,
        int targetCount,
        int invocationCount,
        RunStrategy? runStrategy,
        bool baseline,
        Jit? jit,
        Platform? platform,
        bool disableTieredCompilation,
        RuntimeMoniker runtimeMoniker = RuntimeMoniker.HostProcess)
    {
        Job job = new(id);
        if(launchCount != -1)
        {
            job.Run.LaunchCount = launchCount;
        }

        if(warmupCount != -1)
        {
            job.Run.WarmupCount = warmupCount;
        }

        if(targetCount != -1)
        {
            job.Run.IterationCount = targetCount;
        }

        if(invocationCount != -1)
        {
            job.Run.InvocationCount = invocationCount;
            int num2 = job.Run.ResolveValue(RunMode.UnrollFactorCharacteristic, EnvironmentResolver.Instance);
            if(invocationCount % num2 != 0)
            {
                job.Run.UnrollFactor = 1;
            }
        }

        if(runStrategy.HasValue)
        {
            job.Run.RunStrategy = runStrategy.Value;
        }

        if(baseline)
        {
            job.Meta.Baseline = true;
        }

        if(disableTieredCompilation)
        {
            job.Environment.SetEnvironmentVariable(new EnvironmentVariable("DOTNET_TieredCompilation", "0"));
            job.Environment.SetEnvironmentVariable(new EnvironmentVariable("DOTNET_TC_QuickJit", "0"));
            job.Environment.SetEnvironmentVariable(new EnvironmentVariable("DOTNET_TC_QuickJitForLoops", "0"));
        }

        job = GetJob(job, runtimeMoniker, jit, platform);

        return job;
    }
}
