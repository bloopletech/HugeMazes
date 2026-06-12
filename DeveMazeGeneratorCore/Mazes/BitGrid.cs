using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid(IStore store, Size size, bool leaveOpen = false) : IBitGrid, IStorable
{
    private readonly BitArray array = new((int)size.Area);
    private bool disposed;

    public IStore Store => store;
    public bool IsLong => Extent > int.MaxValue;
    public long Extent => CollectionsMarshal.AsBytes(array).Length + Size.SizeOf;
    public Size Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * size.Height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * size.Height)] = value;
    }

    public static BitGrid Read(IStore store, bool leaveOpen = false)
    {
        var size = store.Read<Size>(0);
        var result = new BitGrid(store, size, leaveOpen);
        store.ReadExactly(Size.SizeOf, CollectionsMarshal.AsBytes(result.array));
        return result;
    }

    public void Write()
    {
        store.Write(0, size);
        store.Write(Size.SizeOf, CollectionsMarshal.AsBytes(array));
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
