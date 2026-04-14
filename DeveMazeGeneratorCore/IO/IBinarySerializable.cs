namespace DeveMazeGeneratorCore.IO;

public interface IBinarySerializable : IDisposable
{
    IBinarySerializer Serializer { get; }
    long Offset { get; }

    void Read();
    void Write();

    Task ReadAsync();
    Task WriteAsync();
}
