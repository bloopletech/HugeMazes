using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGrid : IBitGrid, IBinarySerializable, IDisposable, IAsyncDisposable
{
    private readonly IBinarySerializer serializer;
    private readonly long offset;
    public int width;
    public int height;
    private BigBitArray array;
    private bool disposed;

    public BigBitGrid(IBinarySerializer serializer, long offset)
    {
        this.serializer = serializer;
        this.offset = offset;
        array = new BigBitArray();
    }

    public BigBitGrid(IBinarySerializer serializer, long offset, int width, int height)
    {
        this.serializer = serializer;
        this.offset = offset;

        this.width = width;
        this.height = height;

        array = new BigBitArray(serializer, offset + (sizeof(int) * 2), (long)width * height);
    }

    public IBinarySerializer Serializer => serializer;
    public long Offset => offset;
    public int Width => width;
    public int Height => height;

    public IBitGrid Clone() => Clone(serializer.Create(), 0);

    public IBitGrid Clone(IBinarySerializer destination, long offset)
    {
        Write();
        serializer.CopyTo(destination);
        var result = new BigBitGrid(destination, offset);
        result.Read();
        return result;
    }

    public async Task<IBitGrid> CloneAsync() => await CloneAsync(serializer.Create(), 0);

    public async Task<IBitGrid> CloneAsync(IBinarySerializer destination, long offset)
    {
        await WriteAsync();
        await serializer.CopyToAsync(destination);
        var result = new BigBitGrid(destination, offset);
        await result.ReadAsync();
        return result;
    }

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + ((long)y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + ((long)y * height)] = value;
    }

    public void Read()
    {
        serializer.Position = offset;

        width = serializer.ReadInt32();
        height = serializer.ReadInt32();

        array = new BigBitArray(serializer, serializer.Position);
        //array.Read();
    }

    public async Task ReadAsync()
    {
        Read();
    }

    public void Write()
    {
        serializer.Position = offset;

        serializer.Write(width);
        serializer.Write(height);

        array.Write();
    }

    public async Task WriteAsync()
    {
        serializer.Position = offset;

        serializer.Write(width);
        serializer.Write(height);

        await array.WriteAsync();
    }

    public void Dispose()
    {
        if(disposed) return;
        disposed = true;

        Write();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if(disposed) return;
        disposed = true;

        await WriteAsync();
        GC.SuppressFinalize(this);
    }
}
