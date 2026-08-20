namespace HugeMazes.IO;

public abstract class Storable(IStore store, bool leaveOpen = false) : IStorable
{
    protected IStore store = store;
    private bool disposed;

    public IStore Store => store;

    public abstract long Extent { get; }

    //public static abstract T Read(IStore store, bool leaveOpen = false);
    public abstract void Read();
    public abstract void Write();

    protected void Dispose(bool disposing)
    {
        if(disposed) return;

        if(disposing)
        {
            Write();
            if(!leaveOpen) store.Dispose();
        }

        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
