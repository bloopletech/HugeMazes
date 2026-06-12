namespace DeveMazeGeneratorCore.IO;

//public interface IStorable<T> : IDisposable where T : IStorable<T>
public interface IStorable : IDisposable
{
    IStore Store { get; }
    long Extent { get; }

    //static abstract T Read(IStore store, bool leaveOpen = false);
    //static abstract Task<T> ReadAsync(IStore store, bool leaveOpen = false);
    void Write();
}
