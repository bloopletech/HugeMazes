using System.Reflection;
using BenchmarkDotNet.Configs;

namespace DeveMazeGeneratorCore.Benchmark;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UseAttribute<T> : Attribute, IConfigSource
{
    public IConfig Config { get; }

    public UseAttribute()
    {
        Config = GetFullTypeConfig(typeof(T), ManualConfig.CreateEmpty());
    }

    // Based on https://github.com/dotnet/BenchmarkDotNet/blob/50f9429f3295dc9362cb0aa5cd89e87f93194670/src/BenchmarkDotNet/Running/BenchmarkConverter.cs#L88
    public static IConfig GetFullTypeConfig(Type type, IConfig? config)
    {
        config ??= DefaultConfig.Instance;

        var typeAttributes = type.GetCustomAttributes(true).OfType<IConfigSource>();
        var assemblyAttributes = type.Assembly.GetCustomAttributes().OfType<IConfigSource>();

        foreach(var configFromAttribute in assemblyAttributes.Concat(typeAttributes))
            config = ManualConfig.Union(config, configFromAttribute.Config);

        //return ImmutableConfigBuilder.Create(config);
        return config;
    }
}
