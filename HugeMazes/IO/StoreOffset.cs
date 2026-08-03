using System.Buffers;

namespace HugeMazes.IO;

public class StoreOffset : IStore
{
    private readonly IStore store;
    private readonly bool leaveOpen;
    private readonly long offset;
    private bool disposed;

    public StoreOffset(IStore store, long offset, bool leaveOpen = false)
    {
        if(store is StoreOffset storeOffset)
        {
            this.store = storeOffset.store;
            this.offset = storeOffset.offset + offset;
        }
        else
        {
            this.store = store;
            this.offset = offset;
        }
        this.leaveOpen = leaveOpen;
    }

    public IStore Store => store;

    protected virtual void Dispose(bool disposing)
    {
        if(!disposed)
        {
            if(disposing && !leaveOpen)
            {
                store.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Close() => Dispose(true);
    //public void Dispose() => store.Dispose();

    public long Length
    {
        get => store.Length - offset;
        set => store.Length = value + offset;
    }

    public void EnsureLength() => store.EnsureLength();
    public void EnsureLength(long size) => store.EnsureLength(offset + size);

    // Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs#L51
    public void CopyTo(IStore destination)
    {
        var position = 0L;
        int read;
        var buffer = ArrayPool<byte>.Shared.Rent(IStore.BufferSize);
        try
        {
            while((read = Read(position, buffer)) != 0)
            {
                destination.Write(position, buffer.AsSpan(0, read));
                position += read;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public void Flush() => store.Flush();

    public int Read(long position, Span<byte> buffer) => store.Read(offset + position, buffer);

    public void ReadExactly(long position, Span<byte> buffer) => store.ReadExactly(offset + position, buffer);

    public void Write(long position, ReadOnlySpan<byte> buffer) => store.Write(offset + position, buffer);

    public T Read<T>(long position) where T : struct => store.Read<T>(offset + position);

    public void Read<T>(long position, Span<T> buffer) where T : struct => store.Read(offset + position, buffer);

    public T[] ReadArray<T>(long position) where T : struct => store.ReadArray<T>(offset + position);

    public void Write<T>(long position, T value) where T : struct => store.Write<T>(offset + position, value);

    public void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct =>
        store.Write(offset + position, buffer);

    public void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct =>
        store.WriteArray<T>(offset + position, buffer);

    public IStore Clone() => Clone(IStore.Create(Length));

    public IStore Clone(IStore destination)
    {
        CopyTo(destination);
        return destination;
    }

    public IStore Offset(long offset, bool leaveOpen = false) => new StoreOffset(this, offset, leaveOpen);
    public IStore Offset<T>(bool leaveOpen = false) where T : struct => Offset<T>(0, leaveOpen);
    public IStore Offset<T>(long offset, bool leaveOpen = false) where T : struct =>
        Offset(IStore.SizeOf<T>() + offset, leaveOpen);

    public void Move(long sourceStart, int sourceCount, long destinationStart) =>
        store.Move(offset + sourceStart, sourceCount, offset + destinationStart);
}
