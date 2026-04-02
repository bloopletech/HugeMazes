namespace DeveMazeGeneratorCore.Extensions;

public static class StreamExtensions
{
    //public static bool IsCompleted(this Stream stream) => stream.Length == 0 || (stream.Position + 1) == stream.Length;
    public static bool IsCompleted(this Stream stream) => stream.Position == stream.Length;
    public static bool HasMore(this Stream stream) => !stream.IsCompleted();

    public static void EnsureCompleted(this Stream stream)
    {
        if(stream.HasMore()) throw new InvalidDataException("Stream contains more data than expected");
    }
}
