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
        int bytesRead;
        var buffer = ArrayPool<byte>.Shared.Rent(IStore.BufferSize);
        try
        {
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

    public void Flush() => store.Flush(); // Also BinaryWriter

    public long Seek(long offset, SeekOrigin origin) => store.Seek(offset, origin); // TODO: Need to subtract the offset or something

    public void ReadExactly(long position, byte[] buffer, int offset, int count) =>
        store.ReadExactly(offset + position, buffer, offset, count);

    public int Read(long position, byte[] buffer, int offset, int count) =>
        store.Read(offset + position, buffer, offset, count);

    public int Read(long position, Span<byte> buffer) => store.Read(offset + position, buffer);

    public int ReadByteInt(long position) => store.ReadByteInt(offset + position);

    public void ReadExactly(long position, Span<byte> buffer) => store.ReadExactly(offset + position, buffer);

    public void Write(long position, byte[] buffer, int offset, int count) =>
        store.Write(offset + position, buffer, offset, count);

    public void Write(long position, ReadOnlySpan<byte> buffer) => store.Write(offset + position, buffer);

    public void WriteByte(long position, byte value) => store.WriteByte(offset + position, value);

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
