using System.Buffers;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.IO;

public class StoreOffset(IStore store, long offset, bool leaveOpen = false) : IStore
{
    private bool disposed;

    public StoreOffset(
        StoreOffset store,
        long offset,
        bool leaveOpen = false) : this(store.Store, store.Offset + offset, leaveOpen)
    {
    }

    public IStore Store => store;
    public long Offset => offset;

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
    //public ValueTask DisposeAsync() => store.DisposeAsync();

    public int PeekChar() => store.PeekChar();
    public int Read() => store.Read();
    public byte ReadByte() => store.ReadByte();
    public int Read(char[] buffer, int index, int count) => store.Read(buffer, index, count);
    public int Read(Span<char> buffer) => store.Read(buffer);
    public int Read7BitEncodedInt() => store.Read7BitEncodedInt();
    public long Read7BitEncodedInt64() => store.Read7BitEncodedInt64();
    public bool ReadBoolean() => store.ReadBoolean();
    public byte[] ReadBytes(int count) => store.ReadBytes(count);
    public char ReadChar() => store.ReadChar();
    public char[] ReadChars(int count) => store.ReadChars(count);
    public decimal ReadDecimal() => store.ReadDecimal();
    public double ReadDouble() => store.ReadDouble();
    public Half ReadHalf() => store.ReadHalf();
    public short ReadInt16() => store.ReadInt16();
    public int ReadInt32() => store.ReadInt32();
    public long ReadInt64() => store.ReadInt64();
    public sbyte ReadSByte() => store.ReadSByte();
    public float ReadSingle() => store.ReadSingle();
    public string ReadString() => store.ReadString();
    public ushort ReadUInt16() => store.ReadUInt16();
    public uint ReadUInt32() => store.ReadUInt32();
    public ulong ReadUInt64() => store.ReadUInt64();

    public int PeekChar(long position) => store.PeekChar(offset + position);
    public int Read(long position) => store.Read(offset + position);
    public byte ReadByte(long position) => store.ReadByte(offset + position);
    public int Read(long position, char[] buffer, int index, int count) =>
        store.Read(offset + position, buffer, index, count);
    public int Read(long position, Span<char> buffer) => store.Read(offset + position, buffer);
    public int Read7BitEncodedInt(long position) => store.Read7BitEncodedInt(offset + position);
    public long Read7BitEncodedInt64(long position) => store.Read7BitEncodedInt64(offset + position);
    public bool ReadBoolean(long position) => store.ReadBoolean(offset + position);
    public byte[] ReadBytes(long position, int count) => store.ReadBytes(offset + position, count);
    public char ReadChar(long position) => store.ReadChar(offset + position);
    public char[] ReadChars(long position, int count) => store.ReadChars(offset + position, count);
    public decimal ReadDecimal(long position) => store.ReadDecimal(offset + position);
    public double ReadDouble(long position) => store.ReadDouble(offset + position);
    public Half ReadHalf(long position) => store.ReadHalf(offset + position);
    public short ReadInt16(long position) => store.ReadInt16(offset + position);
    public int ReadInt32(long position) => store.ReadInt32(offset + position);
    public long ReadInt64(long position) => store.ReadInt64(offset + position);
    public sbyte ReadSByte(long position) => store.ReadSByte(offset + position);
    public float ReadSingle(long position) => store.ReadSingle(offset + position);
    public string ReadString(long position) => store.ReadString(offset + position);
    public ushort ReadUInt16(long position) => store.ReadUInt16(offset + position);
    public uint ReadUInt32(long position) => store.ReadUInt32(offset + position);
    public ulong ReadUInt64(long position) => store.ReadUInt64(offset + position);

    public void Write(bool value) => store.Write(value);
    public void Write(byte value) => store.Write(value);
    public void Write(byte[] buffer) => store.Write(buffer);
    public void Write(char ch) => store.Write(ch);
    public void Write(char[] chars) => store.Write(chars);
    public void Write(char[] chars, int index, int count) => store.Write(chars, index, count);
    public void Write(decimal value) => store.Write(value);
    public void Write(double value) => store.Write(value);
    public void Write(float value) => store.Write(value);
    public void Write(Half value) => store.Write(value);
    public void Write(int value) => store.Write(value);
    public void Write(long value) => store.Write(value);
    public void Write(ReadOnlySpan<char> chars) => store.Write(chars);
    public void Write(sbyte value) => store.Write(value);
    public void Write(short value) => store.Write(value);
    public void Write(string value) => store.Write(value);
    public void Write(uint value) => store.Write(value);
    public void Write(ulong value) => store.Write(value);
    public void Write(ushort value) => store.Write(value);
    public void Write7BitEncodedInt(int value) => store.Write(value);
    public void Write7BitEncodedInt64(long value) => store.Write(value);

    public void Write(long position, bool value) => store.Write(offset + position, value);
    public void Write(long position, byte value) => store.Write(offset + position, value);
    public void Write(long position, byte[] buffer) => store.Write(offset + position, buffer);
    public void Write(long position, char ch) => store.Write(offset + position, ch);
    public void Write(long position, char[] chars) => store.Write(offset + position, chars);
    public void Write(long position, char[] chars, int index, int count) =>
        store.Write(offset + position, chars, index, count);
    public void Write(long position, decimal value) => store.Write(offset + position, value);
    public void Write(long position, double value) => store.Write(offset + position, value);
    public void Write(long position, float value) => store.Write(offset + position, value);
    public void Write(long position, Half value) => store.Write(offset + position, value);
    public void Write(long position, int value) => store.Write(offset + position, value);
    public void Write(long position, long value) => store.Write(offset + position, value);
    public void Write(long position, ReadOnlySpan<char> chars) => store.Write(offset + position, chars);
    public void Write(long position, sbyte value) => store.Write(offset + position, value);
    public void Write(long position, short value) => store.Write(offset + position, value);
    public void Write(long position, string value) => store.Write(offset + position, value);
    public void Write(long position, uint value) => store.Write(offset + position, value);
    public void Write(long position, ulong value) => store.Write(offset + position, value);
    public void Write(long position, ushort value) => store.Write(offset + position, value);
    public void Write7BitEncodedInt(long position, int value) => store.Write7BitEncodedInt(offset + position, value);
    public void Write7BitEncodedInt64(long position, long value) =>
        store.Write7BitEncodedInt64(offset + position, value);

    public long Length => store.Length - offset;
    public long Position
    {
        get => store.Position - offset;
        set => store.Position = offset + value;
    }
    public bool IsCompleted => Position == Length;
    public bool HasMore => !IsCompleted;

    public void EnsureCompleted()
    {
        if(HasMore) throw new InvalidDataException("Stream contains more data than expected");
    }

    public void EnsureLength(long fileOffset, long size) => store.EnsureLength(offset + fileOffset, size);

    // Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs#L51
    public void CopyTo(IStore destination)
    {
        int bufferSize = 81920;

        Position = 0;
        destination.Position = 0;

        long position = 0;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while((bytesRead = Read(position, buffer)) != 0)
            {
                destination.Write(position, buffer.AsSpan(0, bytesRead));
                position += bytesRead;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        Position = 0;
        destination.Position = 0;
    }

    public Task CopyToAsync(IStore destination) => CopyToAsync(destination, CancellationToken.None);

    // Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs#L51
    public async Task CopyToAsync(IStore destination, CancellationToken cancellationToken)
    {
        int bufferSize = 81920;

        Position = 0;
        destination.Position = 0;

        long position = 0;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while((bytesRead = await ReadAsync(position, buffer, cancellationToken)) != 0)
            {
                await destination.WriteAsync(
                    position,
                    new ReadOnlyMemory<byte>(buffer, 0, bytesRead),
                    cancellationToken);
                position += bytesRead;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        Position = 0;
        destination.Position = 0;
    }

    public void Flush() => store.Flush(); // Also BinaryWriter

    public Task FlushAsync() => store.FlushAsync();

    public Task FlushAsync(CancellationToken cancellationToken) => store.FlushAsync(cancellationToken);

    public async Task<int> ReadAsync(byte[] buffer, int offset, int count) =>
        await store.ReadAsync(buffer, offset, count);

    public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        await store.ReadAsync(buffer, offset, count, cancellationToken);

    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        await store.ReadAsync(buffer, cancellationToken);

    public void ReadExactly(byte[] buffer, int offset, int count) => store.ReadExactly(buffer, offset, count);

    public async ValueTask ReadExactlyAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default)
    {
        await store.ReadExactlyAsync(buffer, offset, count, cancellationToken);
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        await store.ReadExactlyAsync(buffer, cancellationToken);

    public long Seek(long offset, SeekOrigin origin) => store.Seek(offset, origin); // TODO: Need to subtract the offset or something

    public void SetLength(long value) => store.SetLength(value + offset);

    public int Read(byte[] buffer, int offset, int count) => store.Read(buffer, offset, count); // Also BinaryReader

    public int Read(Span<byte> buffer) => store.Read(buffer); // Also BinaryReader

    public int ReadByteInt() => store.ReadByte(); // Also BinaryReader

    public void ReadExactly(Span<byte> buffer) => store.ReadExactly(buffer); // Also BinaryReader

    public async Task WriteAsync(byte[] buffer, int offset, int count) =>
        await store.WriteAsync(buffer, offset, count);

    public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        await store.WriteAsync(buffer, offset, count, cancellationToken);

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        await store.WriteAsync(buffer, cancellationToken);

    public void Write(byte[] buffer, int offset, int count) => store.Write(buffer, offset, count); // Also BinaryWriter

    public void Write(ReadOnlySpan<byte> buffer) => store.Write(buffer); // Also BinaryWriter

    public void WriteByte(byte value) => store.WriteByte(value);

    public Task<int> ReadAsync(long position, byte[] buffer, int offset, int count) =>
        store.ReadAsync(offset + position, buffer, offset, count);

    public Task<int> ReadAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken) =>
        store.ReadAsync(offset + position, buffer, offset, count, cancellationToken);

    public ValueTask<int> ReadAsync(
        long position,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default) =>
        store.ReadAsync(offset + position, buffer, cancellationToken);

    public void ReadExactly(long position, byte[] buffer, int offset, int count) =>
        store.ReadExactly(offset + position, buffer, offset, count);

    public ValueTask ReadExactlyAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default) =>
        store.ReadExactlyAsync(offset + position, buffer, offset, count, cancellationToken);

    public ValueTask ReadExactlyAsync(
        long position,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default) =>
        store.ReadExactlyAsync(offset + position, buffer, cancellationToken);

    public int Read(long position, byte[] buffer, int offset, int count) =>
        store.Read(offset + position, buffer, offset, count);

    public int Read(long position, Span<byte> buffer) => store.Read(offset + position, buffer);

    public int ReadByteInt(long position) => store.ReadByteInt(offset + position);

    public void ReadExactly(long position, Span<byte> buffer) => store.ReadExactly(offset + position, buffer);

    public Task WriteAsync(long position, byte[] buffer, int offset, int count) =>
        store.WriteAsync(offset + position, buffer, offset, count);

    public Task WriteAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken) =>
        store.WriteAsync(offset + position, buffer, offset, count, cancellationToken);

    public ValueTask WriteAsync(
        long position,
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default) =>
        store.WriteAsync(offset + position, buffer, cancellationToken);

    public void Write(long position, byte[] buffer, int offset, int count) =>
        store.Write(offset + position, buffer, offset, count);

    public void Write(long position, ReadOnlySpan<byte> buffer) => store.Write(offset + position, buffer);

    public void WriteByte(long position, byte value) => store.WriteByte(offset + position, value);

    public T Read<T>() where T : struct
    {
        Span<T> buffer = new T[1];
        Read(buffer);
        return buffer[0];
    }

    //public void Read<T>(T[] array) where T : struct => Read(array, 0, array.Length);

    public void Read<T>(T[] array, int index, int count) where T : struct
    {
        Read(new Span<T>(array, index, count));
    }

    public void Read<T>(Span<T> buffer) where T : struct
    {
        ReadExactly(MemoryMarshal.AsBytes(buffer));
    }

    public T[] ReadArray<T>() where T : struct
    {
        var length = ReadInt32();
        var buffer = new T[length];
        Read(buffer);
        return buffer;
    }

    public void Write<T>(T value) where T : struct
    {
        Span<T> buffer = [value];
        Write(buffer);
    }

    //public void Write<T>(T[] array) where T : struct => Write(array, 0, array.Length);

    public void Write<T>(T[] array, int index, int count) where T : struct
    {
        Write(new ReadOnlySpan<T>(array, index, count));
    }

    public void Write<T>(ReadOnlySpan<T> buffer) where T : struct
    {
        Write(MemoryMarshal.AsBytes(buffer));
    }

    public void WriteArray<T>(ReadOnlySpan<T> buffer) where T : struct
    {
        Write(buffer.Length);
        Write(buffer);
    }

    public T Read<T>(long position) where T : struct => store.Read<T>(offset + position);

    //public void Read<T>(long position, T[] array) where T : struct => WithPosition(position, () => Read<T>(array, 0, array.Length));

    public void Read<T>(long position, T[] array, int index, int count) where T : struct =>
        store.Read(offset + position, array, index, count);

    public void Read<T>(long position, Span<T> buffer) where T : struct => store.Read(offset + position, buffer);

    public T[] ReadArray<T>(long position) where T : struct => store.ReadArray<T>(offset + position);

    public void Write<T>(long position, T value) where T : struct => store.Write<T>(offset + position, value);

    //public void Write<T>(long position, T[] array) where T : struct => WithPosition(position, () => Write(array, 0, array.Length));

    public void Write<T>(long position, T[] array, int index, int count) where T : struct =>
        store.Write(offset + position, array, index, count);

    public void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct =>
        store.Write(offset + position, buffer);

    public void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct =>
        store.WriteArray<T>(offset + position, buffer);

    public IStore Clone() => Clone(IStore.Create(Length));
    public Task<IStore> CloneAsync() => CloneAsync(IStore.Create(Length));

    public IStore Clone(IStore destination)
    {
        CopyTo(destination);
        return destination;
    }

    public async Task<IStore> CloneAsync(IStore destination)
    {
        await CopyToAsync(destination);
        return destination;
    }

    public IStore WithPosition(bool leaveOpen = false)
    {
        if(store is StoreOffset storeOffset) return new StoreOffset(storeOffset, Position, leaveOpen);
        return new StoreOffset(store, Position, leaveOpen);
    }
}
