namespace HugeMazes.Extensions;

public static class ArrayExtensions
{
    public static T[] Extend<T>(this T[] array, int length) => [
        ..array[0..Math.Min(array.Length, length)],
        ..new T[Math.Max(0, length - array.Length)]
    ];
}
