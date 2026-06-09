using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public interface ILongArray<T> : IEnumerable<T> where T : struct
{
    long Length { get; }
    T this[long index] { get; set; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }
    bool Contains(T item);
    long IndexOf(T item);
    T Peek();
    ILongArray<T> Clone();
    ILongArray<T> Clone(IStore destination, bool leaveOpen = false);
}