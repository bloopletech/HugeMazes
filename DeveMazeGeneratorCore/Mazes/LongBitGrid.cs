using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class LongBitGrid(
    IStore store,
    Size size,
    bool leaveOpen = false) : IBitGrid, IStorable
{
    private readonly LongBitArray array = new(new StoreOffset(store, Size.SizeOf, true), size.Area);
    private bool disposed;

    public IStore Store => store;
    public bool IsLong => Extent > int.MaxValue;
    public long Extent => array.Extent + Size.SizeOf;
    public Size Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + ((long)y * size.Height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + ((long)y * size.Height)] = value;
    }

    public static LongBitGrid Read(IStore store, bool leaveOpen = false) => new(store, store.Read<Size>(0), leaveOpen);

    public void Write()
    {
        store.Write(0, size);
        array.Write();
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
        return Read(destination, leaveOpen);
    }
}
