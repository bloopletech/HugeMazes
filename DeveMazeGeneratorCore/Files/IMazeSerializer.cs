using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Files;

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

    public static IMaze Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);

        if(type == BitGridMaze.TypeId)
        {
            var maze = BitGridMaze.Read(reader);
            reader.BaseStream.EnsureCompleted();
            return maze;
        }
        else
        {
            throw new InvalidDataException($"Unknown maze type {type}");
        }
    }

    public static async Task<IMaze> DeserializeAsync(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);

        if(type == BitGridMaze.TypeId)
        {
            var maze = await BitGridMaze.ReadAsync(reader);
            reader.BaseStream.EnsureCompleted();
            return maze;
        }
        else
        {
            throw new InvalidDataException($"Unknown maze type {type}");
        }
    }

    private static void SerializeHeader(BinaryWriter writer)
    {
        writer.Write(MagicHeader);
        writer.Write(Version);
    }

    private static short DeserializeHeader(BinaryReader reader)
    {
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return reader.ReadInt16();
    }
}
