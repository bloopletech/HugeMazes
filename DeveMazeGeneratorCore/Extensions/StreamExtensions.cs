namespace DeveMazeGeneratorCore.Extensions;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        //public static bool IsCompleted(this Stream stream) => stream.Length == 0 || (stream.Position + 1) == stream.Length;
        public bool IsCompleted() => stream.Position == stream.Length;
        public bool HasMore() => !stream.IsCompleted();

        public void EnsureCompleted()
        {
            if(stream.HasMore()) throw new InvalidDataException("Stream contains more data than expected");
        }
    }
}
