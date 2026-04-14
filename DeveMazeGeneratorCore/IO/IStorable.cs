namespace DeveMazeGeneratorCore.IO;

public interface IStorable : IDisposable
{
    IStore Store { get; }
    bool IsBig { get; }
    long Extent { get; }

    void Read();
    void Write();

    Task ReadAsync();
    Task WriteAsync();
}
