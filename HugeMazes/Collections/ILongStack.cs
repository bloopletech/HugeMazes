using HugeMazes.IO;

namespace HugeMazes.Collections;

public interface ILongStack<T> : IEnumerable<T> where T : struct
{
    long Count { get; }
    T this[long index] { get; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }
    void Add(T point);
    void Clear();
    bool Contains(T item);
    long IndexOf(T item);
    T Pop();
    void Push(T point);
    T Peek();
    ILongStack<T> Clone();
    ILongStack<T> Clone(IStore destination, bool leaveOpen = false);
}