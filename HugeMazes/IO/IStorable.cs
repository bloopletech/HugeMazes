namespace HugeMazes.IO;

//public interface IStorable<T> : IDisposable where T : IStorable<T>
public interface IStorable : IDisposable
{
    IStore Store { get; }
	bool IsLong { get; }
    long Extent { get; }

    //static abstract T Read(IStore store, bool leaveOpen = false);
    void Read();
    void Write();

    public void EnsureDiskSpace() => Store.EnsureLength(Extent);
}
