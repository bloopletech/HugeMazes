using System.Runtime.CompilerServices;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;

namespace HugeMazes.Benchmark;

public class NoExportersDefaultConfig : ManualConfig
{
    public static readonly IConfig Instance = new NoExportersDefaultConfig();

    public NoExportersDefaultConfig()
    {
        Add(DefaultConfig.Default);
        GetExportersField(this).Clear();
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "exporters")]
    private extern static ref List<IExporter> GetExportersField(ManualConfig @this);
}

public class AllowDebuggingConfig : ManualConfig
{
    public AllowDebuggingConfig()
    {
        //.WithCustomBuildConfiguration("Debug")
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}