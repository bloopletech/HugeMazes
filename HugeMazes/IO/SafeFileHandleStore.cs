using Microsoft.Win32.SafeHandles;
using System.Buffers;
using System.Runtime.InteropServices;

namespace HugeMazes.IO;

public sealed class SafeFileHandleStore : IStore
{
    private readonly SafeFileHandle handle;
    private readonly string name;

    //public SafeFileHandleStore(SafeFileHandle handle, string name)
    //{
    //    this.handle = handle;
    //    this.name = name;
    //}

    public SafeFileHandleStore(string name, FileMode mode)
    {
        name = Path.GetFullPath(name);
        this.handle = File.OpenHandle(name, mode, FileAccess.ReadWrite);
        this.name = name;
    }

    public SafeFileHandle Handle => handle;
    public void Close() => handle.Close();
    public void Dispose() => handle.Dispose();

    public long Length
    {
        get => RandomAccess.GetLength(handle);
        set => RandomAccess.SetLength(handle, value);
    }

    public void EnsureLength() => EnsureLength(Length);
    public void EnsureLength(long length)
    {
        var drive = new DriveInfo(name);
        var freeSpace = drive.AvailableFreeSpace;

        if(freeSpace >= length) return;

        throw new InsufficientDiskSpaceException(name, length, drive.Name, freeSpace);
    }

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

    public void Flush() => RandomAccess.FlushToDisk(handle);

    public int Read(long position, Span<byte> buffer)
    {
        return RandomAccess.Read(handle, buffer, position);
    }

    public void ReadExactly(long position, Span<byte> buffer)
    {
        int read;
        var totalRead = 0;
        while((read = Read(position + totalRead, buffer[totalRead..])) != 0) totalRead += read;
        if(totalRead != buffer.Length) throw new EndOfStreamException("Unable to read beyond the end of the file.");
    }

    public void Write(long position, ReadOnlySpan<byte> buffer)
    {
        RandomAccess.Write(handle, buffer, position);
    }

    public T Read<T>(long position) where T : struct
    {
        Span<T> buffer = new T[1];
        Read(position, buffer);
        return buffer[0];
    }

    public void Read<T>(long position, Span<T> buffer) where T : struct
    {
        Read(position, MemoryMarshal.AsBytes(buffer));
    }

    public T[] ReadArray<T>(long position) where T : struct
    {
        var length = Read<int>(position);
        var buffer = new T[length];
        Read(position + sizeof(int), buffer);
        return buffer;
    }

    public void Write<T>(long position, T value) where T : struct
    {
        Span<T> buffer = [value];
        Write(position, buffer);
    }

    public void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct
    {
        Write(position, MemoryMarshal.AsBytes(buffer));
    }

    public void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct
    {
        Write(position, buffer.Length);
        Write(position + sizeof(int), buffer);
    }

    public IStore Clone() => Clone(IStore.Create());

    public IStore Clone(IStore destination)
    {
        CopyTo(destination);
        return destination;
    }

    public IStore Offset(long offset, bool leaveOpen = false) => new StoreOffset(this, offset, leaveOpen);
    public IStore Offset<T>(bool leaveOpen = false) where T : struct => Offset<T>(0, leaveOpen);
    public IStore Offset<T>(long offset, bool leaveOpen = false) where T : struct =>
        Offset(IStore.SizeOf<T>() + offset, leaveOpen);

    public void Move(long sourceStart, int sourceCount, long destinationStart)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(sourceCount);
        try
        {
            ReadExactly(sourceStart, buffer.AsSpan(0, sourceCount));
            Write(destinationStart, buffer.AsSpan(0, sourceCount));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}