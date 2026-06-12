using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public interface ILongList<T> : IEnumerable<T> where T : struct
{
    T this[long index] { get; set; }

    long Count { get; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }

    void Add(T item);
    void Clear();
    bool Contains(T item);
    long IndexOf(T item);
    void Insert(long index, T item);
    bool Remove(T item);
    void RemoveAt(long index);
    T Pop();
    void Push(T item);
    T Shift();
    void Unshift(T item);
    T Peek();
    T[] ToArray();
    IList<T> ToList();
    ILongList<T> Clone();
    ILongList<T> Clone(IStore destination, bool leaveOpen = false);
}