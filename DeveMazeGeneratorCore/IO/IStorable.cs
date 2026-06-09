namespace DeveMazeGeneratorCore.IO;

public interface IStorable : IDisposable
{
    IStore Store { get; }
    bool IsLong { get; }
    long Extent { get; }

    void Read();
    void Write();

    Task ReadAsync();
    Task WriteAsync();
}
