namespace DeveMazeGeneratorCore.Collections;

public interface IBigArray<T> : IEnumerable<T> where T : struct
{
    T this[long index] { get; set; }

    long Length { get; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }

    void Clear();
    bool Contains(T item);
    long IndexOf(T item);
    T Peek();
    T[] ToArray();
    IList<T> ToList();
    IBigArray<T> Clone();
    Task<IBigArray<T>> CloneAsync();
}