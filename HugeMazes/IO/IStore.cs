using System.Runtime.CompilerServices;

namespace HugeMazes.IO;

public interface IStore : IDisposable
{
    // Common
    void Close();

    // BinaryReader
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
    long Length { get; set; }

    void EnsureLength();
    void EnsureLength(long size);

    void CopyTo(IStore destination);
    void Flush(); // Also BinaryWriter
    long Seek(long offset, SeekOrigin origin);

    void ReadExactly(long position, byte[] buffer, int offset, int count);
    int Read(long position, byte[] buffer, int offset, int count); // Also BinaryReader
    int Read(long position, Span<byte> buffer); // Also BinaryReader
    int ReadByteInt(long position); // ReadByte in Stream
    void ReadExactly(long position, Span<byte> buffer); // Also BinaryReader
    void Write(long position, byte[] buffer, int offset, int count); // Also BinaryWriter
    void Write(long position, ReadOnlySpan<byte> buffer); // Also BinaryWriter
    void WriteByte(long position, byte value);

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

    IStore Offset(long offset, bool leaveOpen = false);
    IStore Offset<T>(bool leaveOpen = false) where T : struct;
    IStore Offset<T>(long offset, bool leaveOpen = false) where T : struct;

    void Move(long sourceStart, int sourceCount, long destinationStart);

    public static int SizeOf<T>() where T : struct => Unsafe.SizeOf<T>();

    public const int BufferSize = 81920;

    public static StreamStore CreateFile() => new(new TemporaryFileStream());

    public static StreamStore CreateMemory() => new(new MemoryStream());

    public static IStore Create(bool isLong) => (LongOverride ?? isLong) ? CreateFile() : CreateMemory();

    public static IStore Create(long extent) => Create(extent > int.MaxValue);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211")]
    public static bool? LongOverride;

}