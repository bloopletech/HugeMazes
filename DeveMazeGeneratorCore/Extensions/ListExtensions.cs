using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Extensions;

public static class ListExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
    private extern static ref T[] GetItemsField<T>(List<T> @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
    private extern static ref int GetSizeField<T>(List<T> @this);

    extension<T>(List<T> list)
    {
        public ref T[] GetArray() => ref GetItemsField(list);
        public Span<T> GetCurrentArray() => GetItemsField(list).AsSpan(0, GetSizeField(list));

        public T Pop()
        {
            var index = list.Count - 1;
            var item = list[index];
            list.RemoveAt(index);
            return item;
        }

        public void Push(T item)
        {
            list.Add(item);
        }

        public T Shift()
        {
            var index = 0;
            var item = list[index];
            list.RemoveAt(index);
            return item;
        }

        public void Unshift(T item)
        {
            list.Insert(0, item);
        }
    }
}
