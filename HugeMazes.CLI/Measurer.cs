using System.Diagnostics;

namespace HugeMazes.CLI;

public class Measurer : IDisposable
{
    private readonly string name;
    private readonly long start = Stopwatch.GetTimestamp();
    private bool disposed;

    public Measurer(string name)
    {
        this.name = name;
        Console.WriteLine($"Starting {name}...");
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!disposed)
        {
            if(disposing)
            {
                var duration = Stopwatch.GetElapsedTime(start);
                Console.WriteLine($"Completed {name}, took {duration} ({(long)duration.TotalMilliseconds}ms)");
                Console.WriteLine();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public static Measurer Measure(string name) => new(name);

    public static void Measure(string name, Action callback)
    {
        using(Measure(name)) callback();
    }
}