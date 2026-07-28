using System.Runtime.CompilerServices;

namespace HugeMazes.IO;

public interface IStore : IDisposable
{
    // Common
    void Close();

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