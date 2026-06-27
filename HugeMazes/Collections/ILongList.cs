using HugeMazes.IO;

namespace HugeMazes.Collections;

public interface ILongList<T> : IEnumerable<T> where T : struct
{
    long Count { get; }
    T this[long index] { get; set; }
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
    ILongList<T> Clone();
    ILongList<T> Clone(IStore destination, bool leaveOpen = false);
}