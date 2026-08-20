using System.Buffers;
using System.Runtime.InteropServices;

namespace HugeMazes.IO;

public sealed class StreamStore(Stream stream) : IStore
{
    public Stream Stream => stream;
    public void Close() => stream.Close();
    public void Dispose() => stream.Dispose();

    public long Length
    {
        get => stream.Length;
        set => stream.SetLength(value);
    }

    public void EnsureLength() => EnsureLength(Length);
    public void EnsureLength(long length)
    {
        if(stream is FileStream fileStream)
        {
            var fileName = fileStream.Name;
            var drive = new DriveInfo(fileName);
            var freeSpace = drive.AvailableFreeSpace;

            if(freeSpace >= length) return;

            throw new InsufficientDiskSpaceException(fileName, length, drive.Name, freeSpace);
        }
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

    public void Flush() => stream.Flush();

    public int Read(long position, Span<byte> buffer)
    {
        stream.Position = position;
        return stream.Read(buffer);
    }

    public void ReadExactly(long position, Span<byte> buffer)
    {
        stream.Position = position;
        stream.ReadExactly(buffer);
    }

    public void Write(long position, ReadOnlySpan<byte> buffer)
    {
        stream.Position = position;
        stream.Write(buffer);
    }

    public T Read<T>(long position) where T : struct
    {
        Span<T> buffer = new T[1];
        Read(position, buffer);
        return buffer[0];
    }

    public void Read<T>(long position, Span<T> buffer) where T : struct
    {
        ReadExactly(position, MemoryMarshal.AsBytes(buffer));
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
