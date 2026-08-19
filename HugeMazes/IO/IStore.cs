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
    void Flush();

    int Read(long position, Span<byte> buffer);
    void ReadExactly(long position, Span<byte> buffer);
    void Write(long position, ReadOnlySpan<byte> buffer);

    T Read<T>(long position) where T : struct;
    void Read<T>(long position, Span<T> buffer) where T : struct;
    T[] ReadArray<T>(long position) where T : struct;
    void Write<T>(long position, T value) where T : struct;
    void Write<T>(long position, ReadOnlySpan<T> buffer) where T : struct;
    void WriteArray<T>(long position, ReadOnlySpan<T> buffer) where T : struct;

    IStore Clone();

    IStore Offset(long offset, bool leaveOpen = false);
    IStore Offset<T>(bool leaveOpen = false) where T : struct;
    IStore Offset<T>(long offset, bool leaveOpen = false) where T : struct;

    void Move(long sourceStart, int sourceCount, long destinationStart);

    public static int SizeOf<T>() where T : struct => Unsafe.SizeOf<T>();

    public const int BufferSize = 81920;

    public static IStore Create() => new StreamStore(new TemporaryFileStream());
    public static IStore Create(string fileName) => new StreamStore(File.Open(fileName, FileMode.CreateNew));
    //public static IStore Create(string fileName) => new SafeFileHandleStore(fileName, FileMode.CreateNew);
    public static IStore Open(string fileName) => new StreamStore(File.Open(fileName, FileMode.Open));
    //public static IStore Open(string fileName) => new SafeFileHandleStore(fileName, FileMode.Open);
}