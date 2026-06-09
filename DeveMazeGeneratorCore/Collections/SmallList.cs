using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Collections;

public class SmallList<T> : ILongList<T> where T : struct
{
    private readonly List<T> list = [];

    public T this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => list[(int)index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => list[(int)index] = value;
    }

    public long Count => list.Count;
    public bool IsFixedSize => false;
    public bool IsReadOnly => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item) => list.Add(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => list.Clear();

    public bool Contains(T item) => list.Contains(item);

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long IndexOf(T item) => list.IndexOf(item);

    public void Insert(long index, T item) => list.Insert((int)index, item);

    public bool Remove(T item) => list.Remove(item);

    public void RemoveAt(long index) => list.RemoveAt((int)index);

    public T Pop() => list.Pop();

    public void Push(T item) => list.Push(item);

    public T Shift() => list.Shift();

    public void Unshift(T item) => list.Unshift(item);

    public T Peek() => list[^1];

    public T[] ToArray() => [..list];

    public IList<T> ToList() => [..list];

    public ILongList<T> Clone()
    {
        var result = new SmallList<T>();
        result.list.AddRange(list);
        return result;
    }

    public async Task<ILongList<T>> CloneAsync()
    {
        return Clone();
    }
}
