namespace HugeMazes.Extensions;

public static class ExceptionExtensions
{
    public static void ThrowOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");
}
