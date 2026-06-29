namespace HugeMazes.CLI;

public record struct Job(string Name, object? Arguments, Action Action)
{
    public override readonly string ToString() => $"{Name} {Arguments?.ToString()}";
}
