using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public static class IMazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    public static void Serialize(Stream stream, IMaze maze)
    {
        using var writer = new BinaryWriter(stream);
        SerializeHeader(writer);
        maze.Write(writer);
    }

    public static async Task SerializeAsync(Stream stream, IMaze maze)
    {
        using var writer = new BinaryWriter(stream);
        SerializeHeader(writer);
        await maze.WriteAsync(writer);
    }

    private static void SerializeHeader(BinaryWriter writer)
    {
        writer.Write(MagicHeader);
        writer.Write(Version);
    }

    public static IMaze Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);
        var maze = IMaze.Read(type, reader);
        reader.BaseStream.EnsureCompleted();
        return maze;
    }

    public static async Task<IMaze> DeserializeAsync(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);
        var maze = await IMaze.ReadAsync(type, reader);
        reader.BaseStream.EnsureCompleted();
        return maze;
    }

    private static MazeType DeserializeHeader(BinaryReader reader)
    {
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return (MazeType)reader.ReadUInt16();
    }
}
