using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class LongBitGrid : IBitGrid, IStorable, IAsyncDisposable
{
    private readonly IStore store;
    private readonly bool leaveOpen;
    private LongBitArray array;
    private int width;
    private int height;
    private bool disposed;

    public LongBitGrid(IStore store, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        array = null!;
    }

    public LongBitGrid(
        IStore store,
        int width,
        int height,
        bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        this.width = width;
        this.height = height;
        array = new(new StoreOffset(store, sizeof(int) * 2, true), (long)width * height);
    }

    public IStore Store => store;
    public bool IsLong => Extent > int.MaxValue;
    public long Extent => array.Extent + (sizeof(int) * 2);
    public int Width => width;
    public int Height => height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + ((long)y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + ((long)y * height)] = value;
    }

    public void Read()
    {
        store.Position = 0;

        width = store.ReadInt32();
        height = store.ReadInt32();

        array = new(store.WithPosition(true));
        array.Read();
    }

    public async Task ReadAsync()
    {
        Read();
    }

    public void Write()
    {
        store.Position = 0;

        store.Write(width);
        store.Write(height);

        array.Write();
    }

    public async Task WriteAsync()
    {
        store.Position = 0;

        store.Write(width);
        store.Write(height);

        await array.WriteAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!disposed)
        {
            if(disposing)
            {
                Write();
                if(!leaveOpen) store.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if(!disposed)
        {
            await WriteAsync();
            if(!leaveOpen) store.Dispose();
            disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    public IBitGrid Clone() => Clone(IStore.Create(IsLong));

    public IBitGrid Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitGrid(destination, leaveOpen);
        result.Read();
        return result;
    }

    public async Task<IBitGrid> CloneAsync() => await CloneAsync(IStore.Create(IsLong));

    public async Task<IBitGrid> CloneAsync(IStore destination, bool leaveOpen = false)
    {
        await WriteAsync();
        await store.CopyToAsync(destination);
        var result = new LongBitGrid(destination, leaveOpen);
        await result.ReadAsync();
        return result;
    }
}
