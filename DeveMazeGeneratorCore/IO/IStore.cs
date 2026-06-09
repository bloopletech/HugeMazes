using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.IO;

public interface IStore : IDisposable
{
    // Common
    void Close();

    // BinaryReader
    int PeekChar();
    int Read();
    byte ReadByte();
    int Read(char[] buffer, int index, int count);
    int Read(Span<char> buffer);
    int Read7BitEncodedInt();
    long Read7BitEncodedInt64();
    bool ReadBoolean();
    byte[] ReadBytes(int count);
    char ReadChar();
    char[] ReadChars(int count);
    decimal ReadDecimal();
    double ReadDouble();
    Half ReadHalf();
    short ReadInt16();
    int ReadInt32();
    long ReadInt64();
    sbyte ReadSByte();
    float ReadSingle();
    string ReadString();
    ushort ReadUInt16();
    uint ReadUInt32();
    ulong ReadUInt64();

    int PeekChar(long position);
    int Read(long position);
    byte ReadByte(long position);
    int Read(long position, char[] buffer, int index, int count);
    int Read(long position, Span<char> buffer);
    int Read7BitEncodedInt(long position);
    long Read7BitEncodedInt64(long position);
    bool ReadBoolean(long position);
    byte[] ReadBytes(long position, int count);
    char ReadChar(long position);
    char[] ReadChars(long position, int count);
    decimal ReadDecimal(long position);
    double ReadDouble(long position);
    Half ReadHalf(long position);
    short ReadInt16(long position);
    int ReadInt32(long position);
    long ReadInt64(long position);
    sbyte ReadSByte(long position);
    float ReadSingle(long position);
    string ReadString(long position);
    ushort ReadUInt16(long position);
    uint ReadUInt32(long position);
    ulong ReadUInt64(long position);

    // BinaryWriter
    void Write(bool value);
    void Write(byte value);
    void Write(byte[] buffer);
    void Write(char ch);
    void Write(char[] chars);
    void Write(char[] chars, int index, int count);
    void Write(decimal value);
    void Write(double value);
    void Write(float value);
    void Write(Half value);
    void Write(int value);
    void Write(long value);
    void Write(ReadOnlySpan<char> chars);
    void Write(sbyte value);
    void Write(short value);
    void Write(string value);
    void Write(uint value);
    void Write(ulong value);
    void Write(ushort value);
    void Write7BitEncodedInt(int value);
    void Write7BitEncodedInt64(long value);

    void Write(long position, bool value);
    void Write(long position, byte value);
    void Write(long position, byte[] buffer);
    void Write(long position, char ch);
    void Write(long position, char[] chars);
    void Write(long position, char[] chars, int index, int count);
    void Write(long position, decimal value);
    void Write(long position, double value);
    void Write(long position, float value);
    void Write(long position, Half value);
    void Write(long position, int value);
    void Write(long position, long value);
    void Write(long position, ReadOnlySpan<char> chars);
    void Write(long position, sbyte value);
    void Write(long position, short value);
    void Write(long position, string value);
    void Write(long position, uint value);
    void Write(long position, ulong value);
    void Write(long position, ushort value);
    void Write7BitEncodedInt(long position, int value);
    void Write7BitEncodedInt64(long offset, long value);

    // Stream
    //Stream Stream { get; }
    long Length { get; }
    long Position { get; set; }
    bool IsCompleted { get; }
    bool HasMore { get; }
    //long Offset { get; }

    void EnsureCompleted();
    void EnsureLength(long fileOffset, long size);

    void CopyTo(IStore destination);
    Task CopyToAsync(IStore destination);
    Task CopyToAsync(IStore destination, CancellationToken cancellationToken);
    void Flush(); // Also BinaryWriter
    Task FlushAsync();
    Task FlushAsync(CancellationToken cancellationToken);
    Task<int> ReadAsync(byte[] buffer, int offset, int count);
    Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    void ReadExactly(byte[] buffer, int offset, int count);
    ValueTask ReadExactlyAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
    ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    long Seek(long offset, SeekOrigin origin);
    void SetLength(long value);
    int Read(byte[] buffer, int offset, int count); // Also BinaryReader
    int Read(Span<byte> buffer); // Also BinaryReader
    int ReadByteInt(); // ReadByte in Stream
    void ReadExactly(Span<byte> buffer); // Also BinaryReader
    Task WriteAsync(byte[] buffer, int offset, int count);
    Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    void Write(byte[] buffer, int offset, int count); // Also BinaryWriter
    void Write(ReadOnlySpan<byte> buffer); // Also BinaryWriter
    void WriteByte(byte value);

    Task<int> ReadAsync(long position, byte[] buffer, int offset, int count);
    Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    ValueTask<int> ReadAsync(long position, Memory<byte> buffer, CancellationToken cancellationToken = default);
    void ReadExactly(long position, byte[] buffer, int offset, int count);
    ValueTask ReadExactlyAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
    ValueTask ReadExactlyAsync(long position, Memory<byte> buffer, CancellationToken cancellationToken = default);
    int Read(long position, byte[] buffer, int offset, int count); // Also BinaryReader
    int Read(long position, Span<byte> buffer); // Also BinaryReader
    int ReadByteInt(long position); // ReadByte in Stream
    void ReadExactly(long position, Span<byte> buffer); // Also BinaryReader
    Task WriteAsync(long position, byte[] buffer, int offset, int count);
    Task WriteAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    ValueTask WriteAsync(long position, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    void Write(long position, byte[] buffer, int offset, int count); // Also BinaryWriter
    void Write(long position, ReadOnlySpan<byte> buffer); // Also BinaryWriter
    void WriteByte(long position, byte value);

    T Read<T>() where T : struct;
    //void Read<T>(T[] array) where T : struct;
    void Read<T>(T[] array, int index, int count) where T : struct;
    void Read<T>(Span<T> buffer) where T : struct;
    T[] ReadArray<T>() where T : struct;
    void Write<T>(T value) where T : struct;
    //void Write<T>(T[] array) where T : struct;
    void Write<T>(T[] array, int index, int count) where T : struct;
    void Write<T>(ReadOnlySpan<T> buffer) where T : struct;
    void WriteArray<T>(ReadOnlySpan<T> buffer) where T : struct;

    T Read<T>(long position) where T : struct;
    //void Read<T>(long position, T[] array) where T : struct;
    void Read<T>(long position, T[] array, int index, int count) where T : struct;
    void Read<T>(long position, Span<T> buffer) where T : struct;
    T[] ReadArray<T>(long position) where T : struct;
    void Write<T>(long position, T value) where T : struct;
    //void Write<T>(long position, T[] array) where T : struct;
    void Write<T>(long position, T[] array, int index, int count) where T : struct;
    void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct;
    void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct;

    IStore Clone();
    Task<IStore> CloneAsync();

    IStore WithPosition(bool leaveOpen = false);

    public static int SizeOf<T>() where T : struct
    {
        Span<T> buffer = new T[1];
        return MemoryMarshal.AsBytes(buffer).Length;
    }

    public static StreamStore CreateFile() => new(new TemporaryFileStream());

    public static StreamStore CreateMemory() => new(new MemoryStream());

    public static IStore Create(bool isLong) => (LongOverride ?? isLong) ? CreateFile() : CreateMemory();

    public static IStore Create(long extent) => Create(extent > int.MaxValue);

    public static bool? LongOverride;
}