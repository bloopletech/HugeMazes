namespace HugeMazes.Extensions;

public static class ListExtensions
{
    extension<T>(List<T> list)
    {
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
