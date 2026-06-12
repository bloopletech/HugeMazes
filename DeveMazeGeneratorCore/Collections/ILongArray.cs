using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public interface ILongArray<T> : IEnumerable<T> where T : struct
{
    T this[long index] { get; set; }

    long Length { get; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }

    bool Contains(T item);
    long IndexOf(T item);
    T Peek();
    T[] ToArray();
    IList<T> ToList();
    ILongArray<T> Clone();
    ILongArray<T> Clone(IStore destination, bool leaveOpen = false);
}