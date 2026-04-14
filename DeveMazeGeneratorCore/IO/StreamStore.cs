using System.Buffers;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.IO;

public sealed class StreamStore(Stream stream) : IStore, IDisposable, IAsyncDisposable
{
    public StreamStore(string path) : this(File.Open(path, FileMode.OpenOrCreate))
    {
    }

    private readonly BinaryReader reader = new(stream);
    private readonly BinaryWriter writer = new(stream);

    public Stream Stream => stream;
    public void Close() => stream.Close();
    public void Dispose() => stream.Dispose();
    public ValueTask DisposeAsync() => stream.DisposeAsync();

    public int PeekChar() => reader.PeekChar();
    public int Read() => reader.Read();
    public byte ReadByte() => reader.ReadByte();
    public int Read(char[] buffer, int index, int count) => reader.Read(buffer, index, count);
    public int Read(Span<char> buffer) => reader.Read(buffer);
    public int Read7BitEncodedInt() => reader.Read7BitEncodedInt();
    public long Read7BitEncodedInt64() => reader.Read7BitEncodedInt64();
    public bool ReadBoolean() => reader.ReadBoolean();
    public byte[] ReadBytes(int count) => reader.ReadBytes(count);
    public char ReadChar() => reader.ReadChar();
    public char[] ReadChars(int count) => reader.ReadChars(count);
    public decimal ReadDecimal() => reader.ReadDecimal();
    public double ReadDouble() => reader.ReadDouble();
    public Half ReadHalf() => reader.ReadHalf();
    public short ReadInt16() => reader.ReadInt16();
    public int ReadInt32() => reader.ReadInt32();
    public long ReadInt64() => reader.ReadInt64();
    public sbyte ReadSByte() => reader.ReadSByte();
    public float ReadSingle() => reader.ReadSingle();
    public string ReadString() => reader.ReadString();
    public ushort ReadUInt16() => reader.ReadUInt16();
    public uint ReadUInt32() => reader.ReadUInt32();
    public ulong ReadUInt64() => reader.ReadUInt64();

    public int PeekChar(long position)
    {
        Position = position;
        return PeekChar();
    }

    public int Read(long position)
    {
        Position = position;
        return Read();
    }

    public byte ReadByte(long position)
    {
        Position = position;
        return ReadByte();
    }

    public int Read(long position, char[] buffer, int index, int count)
    {
        Position = position;
        return Read(buffer, index, count);
    }

    public int Read(long position, Span<char> buffer)
    {
        Position = position;
        return Read(buffer);
    }

    public int Read7BitEncodedInt(long position)
    {
        Position = position;
        return Read7BitEncodedInt();
    }

    public long Read7BitEncodedInt64(long position)
    {
        Position = position;
        return Read7BitEncodedInt64();
    }

    public bool ReadBoolean(long position)
    {
        Position = position;
        return ReadBoolean();
    }

    public byte[] ReadBytes(long position, int count)
    {
        Position = position;
        return ReadBytes(count);
    }

    public char ReadChar(long position)
    {
        Position = position;
        return ReadChar();
    }

    public char[] ReadChars(long position, int count)
    {
        Position = position;
        return ReadChars(count);
    }

    public decimal ReadDecimal(long position)
    {
        Position = position;
        return ReadDecimal();
    }

    public double ReadDouble(long position)
    {
        Position = position;
        return ReadDouble();
    }

    public Half ReadHalf(long position)
    {
        Position = position;
        return ReadHalf();
    }

    public short ReadInt16(long position)
    {
        Position = position;
        return ReadInt16();
    }

    public int ReadInt32(long position)
    {
        Position = position;
        return ReadInt32();
    }

    public long ReadInt64(long position)
    {
        Position = position;
        return ReadInt64();
    }

    public sbyte ReadSByte(long position)
    {
        Position = position;
        return ReadSByte();
    }

    public float ReadSingle(long position)
    {
        Position = position;
        return ReadSingle();
    }

    public string ReadString(long position)
    {
        Position = position;
        return ReadString();
    }

    public ushort ReadUInt16(long position)
    {
        Position = position;
        return ReadUInt16();
    }

    public uint ReadUInt32(long position)
    {
        Position = position;
        return ReadUInt32();
    }

    public ulong ReadUInt64(long position)
    {
        Position = position;
        return ReadUInt64();
    }

    public void Write(bool value) => writer.Write(value);
    public void Write(byte value) => writer.Write(value);
    public void Write(byte[] buffer) => writer.Write(buffer);
    public void Write(char ch) => writer.Write(ch);
    public void Write(char[] chars) => writer.Write(chars);
    public void Write(char[] chars, int index, int count) => writer.Write(chars, index, count);
    public void Write(decimal value) => writer.Write(value);
    public void Write(double value) => writer.Write(value);
    public void Write(float value) => writer.Write(value);
    public void Write(Half value) => writer.Write(value);
    public void Write(int value) => writer.Write(value);
    public void Write(long value) => writer.Write(value);
    public void Write(ReadOnlySpan<char> chars) => writer.Write(chars);
    public void Write(sbyte value) => writer.Write(value);
    public void Write(short value) => writer.Write(value);
    public void Write(string value) => writer.Write(value);
    public void Write(uint value) => writer.Write(value);
    public void Write(ulong value) => writer.Write(value);
    public void Write(ushort value) => writer.Write(value);
    public void Write7BitEncodedInt(int value) => writer.Write(value);
    public void Write7BitEncodedInt64(long value) => writer.Write(value);

    public void Write(long position, bool value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, byte value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, byte[] buffer)
    {
        Position = position;
        Write(buffer);
    }

    public void Write(long position, char ch)
    {
        Position = position;
        Write(ch);
    }

    public void Write(long position, char[] chars)
    {
        Position = position;
        Write(chars);
    }

    public void Write(long position, char[] chars, int index, int count)
    {
        Position = position;
        Write(position, chars, index, count);
    }

    public void Write(long position, decimal value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, double value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, float value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, Half value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, int value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, long value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, ReadOnlySpan<char> chars)
    {
        Position = position;
        Write(chars);
    }

    public void Write(long position, sbyte value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, short value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, string value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, uint value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, ulong value)
    {
        Position = position;
        Write(value);
    }

    public void Write(long position, ushort value)
    {
        Position = position;
        Write(value);
    }

    public void Write7BitEncodedInt(long position, int value)
    {
        Position = position;
        Write(value);
    }

    public void Write7BitEncodedInt64(long position, long value)
    {
        Position = position;
        Write(value);
    }

    public long Length => stream.Length;
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }
    public bool IsCompleted => stream.Position == stream.Length;
    public bool HasMore => !IsCompleted;

    public void EnsureCompleted()
    {
        if(HasMore) throw new InvalidDataException("Stream contains more data than expected");
    }

    public void EnsureLength(long fileOffset, long size)
    {
        var requiredLength = fileOffset + size;
        if(Length < requiredLength) SetLength(requiredLength);
    }

    // Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs#L51
    public void CopyTo(IStore destination)
    {
        int bufferSize = 81920;

        Position = 0;
        destination.Position = 0;

        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while((bytesRead = Read(buffer)) != 0)
            {
                destination.Write(buffer.AsSpan(0, bytesRead));
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

        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while((bytesRead = await ReadAsync(buffer, cancellationToken)) != 0)
            {
                await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        Position = 0;
        destination.Position = 0;
    }



    public void Flush() => stream.Flush(); // Also BinaryWriter

    public Task FlushAsync() => stream.FlushAsync();

    public Task FlushAsync(CancellationToken cancellationToken) => stream.FlushAsync(cancellationToken);

    public Task<int> ReadAsync(byte[] buffer, int offset, int count) => stream.ReadAsync(buffer, offset, count);

    public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        stream.ReadAsync(buffer, offset, count, cancellationToken);

    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        stream.ReadAsync(buffer, cancellationToken);

    public void ReadExactly(byte[] buffer, int offset, int count) => stream.ReadExactly(buffer, offset, count);

    public ValueTask ReadExactlyAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default) =>
        stream.ReadExactlyAsync(buffer, offset, count, cancellationToken);

    public ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        stream.ReadExactlyAsync(buffer, cancellationToken);

    public long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin); // Also BinaryWriter

    public void SetLength(long value) => stream.SetLength(value);

    public int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count); // Also BinaryReader

    public int Read(Span<byte> buffer) => stream.Read(buffer); // Also BinaryReader

    public int ReadByteInt() => stream.ReadByte(); // Also BinaryReader

    public void ReadExactly(Span<byte> buffer) => stream.ReadExactly(buffer); // Also BinaryReader

    public Task WriteAsync(byte[] buffer, int offset, int count) =>
        stream.WriteAsync(buffer, offset, count);

    public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        stream.WriteAsync(buffer, offset, count, cancellationToken);

    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        stream.WriteAsync(buffer, cancellationToken);

    public void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count); // Also BinaryWriter

    public void Write(ReadOnlySpan<byte> buffer) => stream.Write(buffer); // Also BinaryWriter

    public void WriteByte(byte value) => stream.WriteByte(value);

    public Task<int> ReadAsync(long position, byte[] buffer, int offset, int count)
    {
        Position = position;
        return ReadAsync(buffer, offset, count);
    }

    public Task<int> ReadAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
    {
        Position = position;
        return ReadAsync(buffer, offset, count, cancellationToken);
    }

    public ValueTask<int> ReadAsync(
        long position,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        Position = position;
        return ReadAsync(buffer, cancellationToken);
    }

    public void ReadExactly(long position, byte[] buffer, int offset, int count)
    {
        Position = position;
        ReadExactly(buffer, offset, count);
    }

    public ValueTask ReadExactlyAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default)
    {
        Position = position;
        return ReadExactlyAsync(buffer, offset, count, cancellationToken);
    }

    public ValueTask ReadExactlyAsync(
        long position,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        Position = position;
        return ReadExactlyAsync(buffer, cancellationToken);
    }

    public int Read(long position, byte[] buffer, int offset, int count) // Also BinaryReader
    {
        Position = position;
        return Read(buffer, offset, count);
    }

    public int Read(long position, Span<byte> buffer) // Also BinaryReader
    {
        Position = position;
        return Read(buffer);
    }

    public int ReadByteInt(long position) // Also BinaryReader
    {
        Position = position;
        return ReadByteInt();
    }

    public void ReadExactly(long position, Span<byte> buffer) // Also BinaryReader
    {
        Position = position;
        ReadExactly(buffer);
    }

    public Task WriteAsync(long position, byte[] buffer, int offset, int count)
    {
        Position = position;
        return WriteAsync(buffer, offset, count);
    }

    public Task WriteAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
    {
        Position = position;
        return WriteAsync(buffer, offset, count, cancellationToken);
    }

    public ValueTask WriteAsync(
        long position,
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        Position = position;
        return WriteAsync(buffer, cancellationToken);
    }

    public void Write(long position, byte[] buffer, int offset, int count) // Also BinaryWriter
    {
        Position = position;
        Write(buffer, offset, count);
    }

    public void Write(long position, ReadOnlySpan<byte> buffer) // Also BinaryWriter
    {
        Position = position;
        Write(buffer);
    }

    public void WriteByte(long position, byte value)
    {
        Position = position;
        WriteByte(value);
    }

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

    public T Read<T>(long position) where T : struct
    {
        Position = position;
        return Read<T>();
    }

    //public void Read<T>(long position, T[] array) where T : struct
    //{
    //    Position = position;
    //    Read<T>(array, 0, array.Length);
    //}

    public void Read<T>(long position, T[] array, int index, int count) where T : struct
    {
        Position = position;
        Read<T>(array, index, count);
    }

    public void Read<T>(long position, Span<T> buffer) where T : struct
    {
        Position = position;
        Read(buffer);
    }

    public T[] ReadArray<T>(long position) where T : struct
    {
        Position = position;
        return ReadArray<T>();
    }

    public void Write<T>(long position, T value) where T : struct
    {
        Position = position;
        Write(value);
    }

    //public void Write<T>(long position, T[] array) where T : struct
    //{
    //    Position = position;
    //    Write(array, 0, array.Length);
    //}

    public void Write<T>(long position, T[] array, int index, int count) where T : struct
    {
        Position = position;
        Write(array, index, count);
    }

    public void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct
    {
        Position = position;
        Write(buffer);
    }

    public void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct
    {
        Position = position;
        WriteArray(buffer);
    }

    public IStore Clone() => Clone(IStore.Create(Length));

    public IStore Clone(IStore destination)
    {
        CopyTo(destination);
        return destination;
    }

    public Task<IStore> CloneAsync() => CloneAsync(IStore.Create(Length));

    public async Task<IStore> CloneAsync(IStore destination)
    {
        await CopyToAsync(destination);
        return destination;
    }

    public IStore WithPosition(bool leaveOpen = false) => new StoreOffset(this, Position, leaveOpen);
}