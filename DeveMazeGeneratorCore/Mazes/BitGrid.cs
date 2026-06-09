using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid : IBitGrid, IStorable
{
    private readonly IStore store;
    private readonly bool leaveOpen;
    private BitArray array;
    private int width;
    private int height;
    private bool disposed;

    public BitGrid(IStore store, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        array = null!;
    }

    public BitGrid(IStore store, int width, int height, bool leaveOpen = false)
    {
        this.store = store;
        this.leaveOpen = leaveOpen;
        this.width = width;
        this.height = height;
        array = new(width * height);
    }

    public IStore Store => store;
    public bool IsLong => Extent > int.MaxValue;
    public long Extent => CollectionsMarshal.AsBytes(array).Length + (sizeof(int) * 2);
    public int Width => width;
    public int Height => height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * height)] = value;
    }

    public void Read()
    {
        store.Position = 0;

        width = store.ReadInt32();
        height = store.ReadInt32();

        array = new BitArray(width * height);
        store.ReadExactly(CollectionsMarshal.AsBytes(array));
    }

    public async Task ReadAsync()
    {
        Read();
        //store.Position = 0;

        //width = store.ReadInt32();
        //height = store.ReadInt32();

        //array = new BitArray(width * height);
        //await store.ReadExactlyAsync(array.GetArray());
    }

    public void Write()
    {
        store.Position = 0;

        store.Write(width);
        store.Write(height);

        store.Write(CollectionsMarshal.AsBytes(array));
        //store.WriteArray(array.GetArray());
    }

    public async Task WriteAsync()
    {
        Write();
        //store.Position = 0;

        //store.Write(width);
        //store.Write(height);

        //await store.WriteAsync(array.GetArray());
        ////await store.WriteArrayAsync(array.GetArray());
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

    public IBitGrid Clone() => Clone(IStore.Create(IsLong));

    public IBitGrid Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new BitGrid(destination, leaveOpen);
        result.Read();
        return result;
    }

    public async Task<IBitGrid> CloneAsync() => Clone(IStore.Create(IsLong));

    public async Task<IBitGrid> CloneAsync(IStore destination, bool leaveOpen = false)
    {
        await WriteAsync();
        await store.CopyToAsync(destination);
        var result = new BitGrid(destination, leaveOpen);
        await result.ReadAsync();
        return result;
    }
}
