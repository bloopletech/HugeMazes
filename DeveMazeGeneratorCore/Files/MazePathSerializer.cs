using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Files;

public static class MazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    public static void Serialize(Stream stream, MazePath path)
    {
        using var writer = new BinaryWriter(stream);
        SerializeHeader(writer);
        path.Write(writer);
    }

    public static async Task SerializeAsync(Stream stream, MazePath path)
    {
        using var writer = new BinaryWriter(stream);
        SerializeHeader(writer);
        await path.WriteAsync(writer);
    }

    public static MazePath Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);

        if(type == MazePath.TypeId)
        {
            var path = MazePath.Read(reader);
            reader.BaseStream.EnsureCompleted();
            return path;
        }
        else
        {
            throw new InvalidDataException($"Unknown path type {type}");
        }
    }

    public static async Task<MazePath> DeserializeAsync(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);

        if(type == MazePath.TypeId)
        {
            var path = await MazePath.ReadAsync(reader);
            reader.BaseStream.EnsureCompleted();
            return path;
        }
        else
        {
            throw new InvalidDataException($"Unknown path type {type}");
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
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return reader.ReadInt16();
    }
}
