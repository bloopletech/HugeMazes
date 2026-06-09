using System.Buffers;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.IO;

public sealed class StreamStore(Stream stream) : IStore
{
    public StreamStore(string path) : this(File.Open(path, FileMode.OpenOrCreate))
    {
    }

    private readonly BinaryReader reader = new(stream);
    private readonly BinaryWriter writer = new(stream);

    public Stream Stream => stream;
    public void Close() => stream.Close();
    public void Dispose() => stream.Dispose();

    public int PeekChar(long position)
    {
        stream.Position = position;
        return reader.PeekChar();
    }

    public int Read(long position)
    {
        stream.Position = position;
        return reader.Read();
    }

    public byte ReadByte(long position)
    {
        stream.Position = position;
        return reader.ReadByte();
    }

    public int Read(long position, char[] buffer, int index, int count)
    {
        stream.Position = position;
        return reader.Read(buffer, index, count);
    }

    public int Read(long position, Span<char> buffer)
    {
        stream.Position = position;
        return reader.Read(buffer);
    }

    public int Read7BitEncodedInt(long position)
    {
        stream.Position = position;
        return reader.Read7BitEncodedInt();
    }

    public long Read7BitEncodedInt64(long position)
    {
        stream.Position = position;
        return reader.Read7BitEncodedInt64();
    }

    public bool ReadBoolean(long position)
    {
        stream.Position = position;
        return reader.ReadBoolean();
    }

    public byte[] ReadBytes(long position, int count)
    {
        stream.Position = position;
        return reader.ReadBytes(count);
    }

    public char ReadChar(long position)
    {
        stream.Position = position;
        return reader.ReadChar();
    }

    public char[] ReadChars(long position, int count)
    {
        stream.Position = position;
        return reader.ReadChars(count);
    }

    public decimal ReadDecimal(long position)
    {
        stream.Position = position;
        return reader.ReadDecimal();
    }

    public double ReadDouble(long position)
    {
        stream.Position = position;
        return reader.ReadDouble();
    }

    public Half ReadHalf(long position)
    {
        stream.Position = position;
        return reader.ReadHalf();
    }

    public short ReadInt16(long position)
    {
        stream.Position = position;
        return reader.ReadInt16();
    }

    public int ReadInt32(long position)
    {
        stream.Position = position;
        return reader.ReadInt32();
    }

    public long ReadInt64(long position)
    {
        stream.Position = position;
        return reader.ReadInt64();
    }

    public sbyte ReadSByte(long position)
    {
        stream.Position = position;
        return reader.ReadSByte();
    }

    public float ReadSingle(long position)
    {
        stream.Position = position;
        return reader.ReadSingle();
    }

    public string ReadString(long position)
    {
        stream.Position = position;
        return reader.ReadString();
    }

    public ushort ReadUInt16(long position)
    {
        stream.Position = position;
        return reader.ReadUInt16();
    }

    public uint ReadUInt32(long position)
    {
        stream.Position = position;
        return reader.ReadUInt32();
    }

    public ulong ReadUInt64(long position)
    {
        stream.Position = position;
        return reader.ReadUInt64();
    }

    public void Write(long position, bool value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, byte value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, byte[] buffer)
    {
        stream.Position = position;
        writer.Write(buffer);
    }

    public void Write(long position, char ch)
    {
        stream.Position = position;
        writer.Write(ch);
    }

    public void Write(long position, char[] chars)
    {
        stream.Position = position;
        writer.Write(chars);
    }

    public void Write(long position, char[] chars, int index, int count)
    {
        stream.Position = position;
        writer.Write(chars, index, count);
    }

    public void Write(long position, decimal value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, double value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, float value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, Half value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, int value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, long value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, ReadOnlySpan<char> chars)
    {
        stream.Position = position;
        writer.Write(chars);
    }

    public void Write(long position, sbyte value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, short value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, string value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, uint value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, ulong value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write(long position, ushort value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write7BitEncodedInt(long position, int value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public void Write7BitEncodedInt64(long position, long value)
    {
        stream.Position = position;
        writer.Write(value);
    }

    public long Length => stream.Length;

    public void EnsureLength(long fileOffset, long size)
    {
        var requiredLength = fileOffset + size;
        if(Length < requiredLength) SetLength(requiredLength);
    }

    // Based on https://github.com/dotnet/runtime/blob/b82454cad0aaaae3db2cf18fbf2cccc36e201ccc/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs#L51
    public void CopyTo(IStore destination)
    {
        int bufferSize = 81920;

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
    }

    public void Flush() => stream.Flush(); // Also BinaryWriter

    public long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin); // Also BinaryWriter

    public void SetLength(long value) => stream.SetLength(value);

    public void ReadExactly(long position, byte[] buffer, int offset, int count)
    {
        stream.Position = position;
        stream.ReadExactly(buffer, offset, count);
    }

    public int Read(long position, byte[] buffer, int offset, int count) // Also BinaryReader
    {
        stream.Position = position;
        return stream.Read(buffer, offset, count);
    }

    public int Read(long position, Span<byte> buffer) // Also BinaryReader
    {
        stream.Position = position;
        return stream.Read(buffer);
    }

    public int ReadByteInt(long position) // Also BinaryReader
    {
        stream.Position = position;
        return stream.ReadByte();
    }

    public void ReadExactly(long position, Span<byte> buffer) // Also BinaryReader
    {
        stream.Position = position;
        stream.ReadExactly(buffer);
    }

    public void Write(long position, byte[] buffer, int offset, int count) // Also BinaryWriter
    {
        stream.Position = position;
        stream.Write(buffer, offset, count);
    }

    public void Write(long position, ReadOnlySpan<byte> buffer) // Also BinaryWriter
    {
        stream.Position = position;
        stream.Write(buffer);
    }

    public void WriteByte(long position, byte value)
    {
        stream.Position = position;
        stream.WriteByte(value);
    }

    public T Read<T>(long position) where T : struct
    {
        Span<T> buffer = new T[1];
        Read(position, buffer);
        return buffer[0];
    }

    //public void Read<T>(long position, T[] array) where T : struct
    //{
    //    Read<T>(position, array, 0, array.Length);
    //}

    public void Read<T>(long position, T[] array, int index, int count) where T : struct
    {
        Read(position, new Span<T>(array, index, count));
    }

    public void Read<T>(long position, Span<T> buffer) where T : struct
    {
        Read(position, MemoryMarshal.AsBytes(buffer));
    }

    public T[] ReadArray<T>(long position) where T : struct
    {
        var length = ReadInt32(position);
        var buffer = new T[length];
        Read(position + sizeof(int), buffer);
        return buffer;
    }

    public void Write<T>(long position, T value) where T : struct
    {
        Span<T> buffer = [value];
        Write(position, buffer);
    }

    //public void Write<T>(long position, T[] array) where T : struct
    //{
    //    Write(position, array, 0, array.Length);
    //}

    public void Write<T>(long position, T[] array, int index, int count) where T : struct
    {
        Write(position, new ReadOnlySpan<T>(array, index, count));
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
}