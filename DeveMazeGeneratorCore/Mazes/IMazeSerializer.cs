using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public static class IMazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    public static void Serialize(Stream stream, IMaze maze)
    {
        SerializeHeader(stream);
        maze.Write(stream);
    }

    public static async Task SerializeAsync(Stream stream, IMaze maze)
    {
        SerializeHeader(stream);
        await maze.WriteAsync(stream);
    }

    private static void SerializeHeader(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write(MagicHeader);
        writer.Write(Version);
    }

    public static IMaze Deserialize(Stream stream)
    {
        var type = DeserializeHeader(stream);
        var maze = IMaze.Read(type, stream);
        stream.EnsureCompleted();
        return maze;
    }

    public static async Task<IMaze> DeserializeAsync(Stream stream)
    {
        var type = DeserializeHeader(stream);
        var maze = await IMaze.ReadAsync(type, stream);
        stream.EnsureCompleted();
        return maze;
    }

    private static MazeType DeserializeHeader(Stream stream)
    {
        using var reader = stream.Reader();
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return (MazeType)reader.ReadUInt16();
    }
}
