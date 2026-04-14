using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.IO;

public class BinarySerializer(Stream stream) : IBinarySerializer, IDisposable, IAsyncDisposable
{
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

    public int PeekChar(long position) => WithPosition(position, PeekChar);
    public int Read(long position) => WithPosition(position, Read);
    public byte ReadByte(long position) => WithPosition(position, ReadByte);
    public int Read(long position, char[] buffer, int index, int count) =>
        WithPosition(position, () => Read(buffer, index, count));
    public int Read(long position, Span<char> buffer)
    {
        var previousPosition = Position;
        Position = position;
        var result = Read(buffer);
        Position = previousPosition;
        return result;
    }
    public int Read7BitEncodedInt(long position) => WithPosition(position, Read7BitEncodedInt);
    public long Read7BitEncodedInt64(long position) => WithPosition(position, Read7BitEncodedInt64);
    public bool ReadBoolean(long position) => WithPosition(position, ReadBoolean);
    public byte[] ReadBytes(long position, int count) => WithPosition(position, () => ReadBytes(count));
    public char ReadChar(long position) => WithPosition(position, ReadChar);
    public char[] ReadChars(long position, int count) => WithPosition(position, () => ReadChars(count));
    public decimal ReadDecimal(long position) => WithPosition(position, ReadDecimal);
    public double ReadDouble(long position) => WithPosition(position, ReadDouble);
    public Half ReadHalf(long position) => WithPosition(position, ReadHalf);
    public short ReadInt16(long position) => WithPosition(position, ReadInt16);
    public int ReadInt32(long position) => WithPosition(position, ReadInt32);
    public long ReadInt64(long position) => WithPosition(position, ReadInt64);
    public sbyte ReadSByte(long position) => WithPosition(position, ReadSByte);
    public float ReadSingle(long position) => WithPosition(position, ReadSingle);
    public string ReadString(long position) => WithPosition(position, ReadString);
    public ushort ReadUInt16(long position) => WithPosition(position, ReadUInt16);
    public uint ReadUInt32(long position) => WithPosition(position, ReadUInt32);
    public ulong ReadUInt64(long position) => WithPosition(position, ReadUInt64);

    public long Seek(int offset, SeekOrigin origin) => writer.Seek(offset, origin);
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

    public void Write(long position, bool value) => WithPosition(position, () => Write(value));
    public void Write(long position, byte value) => WithPosition(position, () => Write(value));
    public void Write(long position, byte[] buffer) => WithPosition(position, () => Write(buffer));
    public void Write(long position, char ch) => WithPosition(position, () => Write(ch));
    public void Write(long position, char[] chars) => WithPosition(position, () => Write(chars));
    public void Write(long position, char[] chars, int index, int count) =>
        WithPosition(position, () => Write(chars, index, count));
    public void Write(long position, decimal value) => WithPosition(position, () => Write(value));
    public void Write(long position, double value) => WithPosition(position, () => Write(value));
    public void Write(long position, float value) => WithPosition(position, () => Write(value));
    public void Write(long position, Half value) => WithPosition(position, () => Write(value));
    public void Write(long position, int value) => WithPosition(position, () => Write(value));
    public void Write(long position, long value) => WithPosition(position, () => Write(value));
    public void Write(long position, ReadOnlySpan<char> chars)
    {
        var previousPosition = Position;
        Position = position;
        Write(chars);
        Position = previousPosition;
    }
    public void Write(long position, sbyte value) => WithPosition(position, () => Write(value));
    public void Write(long position, short value) => WithPosition(position, () => Write(value));
    public void Write(long position, string value) => WithPosition(position, () => Write(value));
    public void Write(long position, uint value) => WithPosition(position, () => Write(value));
    public void Write(long position, ulong value) => WithPosition(position, () => Write(value));
    public void Write(long position, ushort value) => WithPosition(position, () => Write(value));
    public void Write7BitEncodedInt(long position, int value) =>
        WithPosition(position, () => Write7BitEncodedInt(value));
    public void Write7BitEncodedInt64(long position, long value) =>
        WithPosition(position, () => Write7BitEncodedInt64(value));

    public bool CanRead => stream.CanRead;
    public bool CanSeek => stream.CanSeek;
    public bool CanTimeout => stream.CanTimeout;
    public bool CanWrite => stream.CanWrite;
    public long Length => stream.Length;
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }
    public int ReadTimeout
    {
        get => stream.ReadTimeout;
        set => stream.ReadTimeout = value;
    }
    public int WriteTimeout
    {
        get => stream.WriteTimeout;
        set => stream.WriteTimeout = value;
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

    public IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
        stream.BeginRead(buffer, offset, count, callback, state);

    public IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
        stream.BeginWrite(buffer, offset, count, callback, state);

    public void CopyTo(IBinarySerializer destination)
    {
        var previousPosition = Position;
        WithPosition(0, () =>
        {
            destination.EnsureLength(0, Length);
            destination.Position = previousPosition;
            destination.WithPosition(0, () => stream.CopyTo(destination.Stream));
        });
    }

    public void CopyTo(IBinarySerializer destination, int bufferSize)
    {
        var previousPosition = Position;
        WithPosition(0, () =>
        {
            destination.EnsureLength(0, Length);
            destination.Position = previousPosition;
            destination.WithPosition(0, () => stream.CopyTo(destination.Stream, bufferSize));
        });
    }

    public async Task CopyToAsync(IBinarySerializer destination)
    {
        var previousPosition = Position;
        await WithPosition(0, async () =>
        {
            destination.EnsureLength(0, Length);
            destination.Position = previousPosition;
            await destination.WithPosition(0, async () => await stream.CopyToAsync(destination.Stream));
        });
    }

    public async Task CopyToAsync(IBinarySerializer destination, CancellationToken cancellationToken)
    {
        var previousPosition = Position;
        await WithPosition(0, async () =>
        {
            destination.EnsureLength(0, Length);
            destination.Position = previousPosition;
            await destination.WithPosition(0, async () =>
                await stream.CopyToAsync(destination.Stream, cancellationToken));
        });
    }

    public async Task CopyToAsync(IBinarySerializer destination, int bufferSize)
    {
        var previousPosition = Position;
        await WithPosition(0, async () =>
        {
            destination.EnsureLength(0, Length);
            destination.Position = previousPosition;
            await destination.WithPosition(0, async () => await stream.CopyToAsync(destination.Stream, bufferSize));
        });
    }

    public async Task CopyToAsync(
        IBinarySerializer destination,
        int bufferSize,
        CancellationToken cancellationToken)
    {
        var previousPosition = Position;
        await WithPosition(0, async () =>
        {
            destination.EnsureLength(0, Length);
            destination.Position = previousPosition;
            await destination.WithPosition(0, async () =>
                await stream.CopyToAsync(destination.Stream, bufferSize, cancellationToken));
        });
    }

    public int EndRead(IAsyncResult asyncResult) => stream.EndRead(asyncResult);

    public void EndWrite(IAsyncResult asyncResult) => stream.EndWrite(asyncResult);

    public void Flush() => stream.Flush(); // Also BinaryWriter

    public async Task FlushAsync() => await stream.FlushAsync();

    public async Task FlushAsync(CancellationToken cancellationToken) => await stream.FlushAsync(cancellationToken);

    public async Task<int> ReadAsync(byte[] buffer, int offset, int count) =>
        await stream.ReadAsync(buffer, offset, count);

    public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        await stream.ReadAsync(buffer, offset, count, cancellationToken);

    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        await stream.ReadAsync(buffer, cancellationToken);

    public int ReadAtLeast(Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true) =>
        stream.ReadAtLeast(buffer, minimumBytes, throwOnEndOfStream);

    public async ValueTask<int> ReadAtLeastAsync(
        Memory<byte> buffer,
        int minimumBytes,
        bool throwOnEndOfStream = true,
        CancellationToken cancellationToken = default)
    {
        return await stream.ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
    }

    public void ReadExactly(byte[] buffer, int offset, int count) => stream.ReadExactly(buffer, offset, count);

    public async ValueTask ReadExactlyAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default)
    {
        await stream.ReadExactlyAsync(buffer, offset, count, cancellationToken);
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        await stream.ReadExactlyAsync(buffer, cancellationToken);

    public long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);

    public void SetLength(long value) => stream.SetLength(value);

    public int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count); // Also BinaryReader

    public int Read(Span<byte> buffer) => stream.Read(buffer); // Also BinaryReader

    public int ReadByteInt() => stream.ReadByte(); // Also BinaryReader

    public void ReadExactly(Span<byte> buffer) => stream.ReadExactly(buffer); // Also BinaryReader

    public async Task WriteAsync(byte[] buffer, int offset, int count) =>
        await stream.WriteAsync(buffer, offset, count);

    public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        await stream.WriteAsync(buffer, offset, count, cancellationToken);

    public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        await stream.WriteAsync(buffer, cancellationToken);

    public void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count); // Also BinaryWriter

    public void Write(ReadOnlySpan<byte> buffer) => stream.Write(buffer); // Also BinaryWriter

    public void WriteByte(byte value) => stream.WriteByte(value);

    public async Task<int> ReadAsync(long position, byte[] buffer, int offset, int count) =>
        await WithPosition(position, async () => await ReadAsync(buffer, offset, count));

    public async Task<int> ReadAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
    {
        return await WithPosition(position, async () => await ReadAsync(buffer, offset, count, cancellationToken));
    }

    public async ValueTask<int> ReadAsync(
        long position,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        return await WithPosition(position, async () => await ReadAsync(buffer, cancellationToken));
    }

    public int ReadAtLeast(long position, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
    {
        var previousPosition = Position;
        Position = position;
        var result = ReadAtLeast(buffer, minimumBytes, throwOnEndOfStream);
        Position = previousPosition;
        return result;
    }

    public async ValueTask<int> ReadAtLeastAsync(
        long position,
        Memory<byte> buffer,
        int minimumBytes,
        bool throwOnEndOfStream = true,
        CancellationToken cancellationToken = default)
    {
        return await WithPosition(
            position,
            async () => await ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream, cancellationToken));
    }
        
    public void ReadExactly(long position, byte[] buffer, int offset, int count) =>
        WithPosition(position, () => ReadExactly(buffer, offset, count));

    public async ValueTask ReadExactlyAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default)
    {
        await WithPosition(position, async () => await ReadExactlyAsync(buffer, offset, count, cancellationToken));
    }

    public async ValueTask ReadExactlyAsync(
        long position,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        await WithPosition(position, async () => await ReadExactlyAsync(buffer, cancellationToken));
    }

    public int Read(long position, byte[] buffer, int offset, int count) => 
        WithPosition(position, () => Read(buffer, offset, count)); // Also BinaryReader

    public int Read(long position, Span<byte> buffer) // Also BinaryReader
    {
        var previousPosition = Position;
        Position = position;
        var result = Read(buffer);
        Position = previousPosition;
        return result;
    }

    public int ReadByteInt(long position) => WithPosition(position, ReadByteInt); // Also BinaryReader

    public void ReadExactly(long position, Span<byte> buffer) // Also BinaryReader
    {
        var previousPosition = Position;
        Position = position;
        ReadExactly(buffer);
        Position = previousPosition;
    }

    public async Task WriteAsync(long position, byte[] buffer, int offset, int count) =>
        await WithPosition(position, async () => await WriteAsync(buffer, offset, count));

    public async Task WriteAsync(
        long position,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
    {
        await WithPosition(position, async () => await WriteAsync(buffer, offset, count, cancellationToken));
    }

    public async ValueTask WriteAsync(
        long position,
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        await WithPosition(position, async () => await WriteAsync(buffer, cancellationToken));
    }

    public void Write(long position, byte[] buffer, int offset, int count) =>
        WithPosition(position, () => Write(buffer, offset, count)); // Also BinaryWriter

    public void Write(long position, ReadOnlySpan<byte> buffer) // Also BinaryWriter
    {
        var previousPosition = Position;
        Position = position;
        Write(buffer);
        Position = previousPosition;
    }

    public void WriteByte(long position, byte value) => WithPosition(position, () => WriteByte(value));

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

    public T Read<T>(long position) where T : struct => WithPosition(position, Read<T>);

    //public void Read<T>(long position, T[] array) where T : struct => WithPosition(position, () => Read<T>(array, 0, array.Length));

    public void Read<T>(long position, T[] array, int index, int count) where T : struct =>
        WithPosition(position, () => Read(array, index, count));

    public void Read<T>(long position, Span<T> buffer) where T : struct
    {
        var previousPosition = Position;
        Position = position;
        Read(buffer);
        Position = previousPosition;
    }

    public T[] ReadArray<T>(long position) where T : struct => WithPosition(position, ReadArray<T>);

    public void Write<T>(long position, T value) where T : struct => WithPosition(position, () => Write(value));

    //public void Write<T>(long position, T[] array) where T : struct => WithPosition(position, () => Write(array, 0, array.Length));

    public void Write<T>(long position, T[] array, int index, int count) where T : struct =>
        WithPosition(position, () => Write(array, index, count));

    public void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct
    {
        var previousPosition = Position;
        Position = position;
        Write(buffer);
        Position = previousPosition;
    }

    public void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct
    {
        var previousPosition = Position;
        Position = position;
        WriteArray(buffer);
        Position = previousPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WithPosition(long position, Action action)
    {
        var previousPosition = Position;
        Position = position;
        action();
        Position = previousPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T WithPosition<T>(long position, Func<T> action)
    {
        var previousPosition = Position;
        Position = position;
        var result = action();
        Position = previousPosition;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task WithPosition(long position, Func<Task> action)
    {
        var previousPosition = Position;
        Position = position;
        await action();
        Position = previousPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<T> WithPosition<T>(long position, Func<Task<T>> action)
    {
        var previousPosition = Position;
        Position = position;
        var result = await action();
        Position = previousPosition;
        return result;
    }

    public IBinarySerializer Clone()
    {
        var bs = Create();
        CopyTo(bs);
        return bs;
    }

    public async Task<IBinarySerializer> CloneAsync()
    {
        var bs = Create();
        await CopyToAsync(bs);
        return bs;
    }

    public static BinarySerializer CreateFile() => new(new TemporaryFileStream());

    public static BinarySerializer CreateMemory() => new(new MemoryStream());

    public IBinarySerializer Create() => Length > int.MaxValue ? CreateFile() : CreateMemory();
}