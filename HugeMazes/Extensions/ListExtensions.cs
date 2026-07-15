using System.Runtime.CompilerServices;

namespace HugeMazes.Extensions;

public static class ListExtensions
{
    extension<T>(List<T> list)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            var item = list[^1];
            list.PopIgnore();
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopIgnore() => list.RemoveAt(list.Count - 1);

        public void Push(T item) => list.Add(item);

        public T Shift()
        {
            var index = 0;
            var item = list[index];
            list.RemoveAt(index);
            return item;
        }

        public void Unshift(T item) => list.Insert(0, item);
    }
}
