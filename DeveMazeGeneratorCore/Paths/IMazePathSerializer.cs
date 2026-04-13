using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Paths;

public static class IMazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    public static void Serialize(Stream stream, IMazePath path)
    {
        using var writer = new BinaryWriter(stream);
        SerializeHeader(writer);
        path.Write(writer);
    }

    public static async Task SerializeAsync(Stream stream, IMazePath path)
    {
        using var writer = new BinaryWriter(stream);
        SerializeHeader(writer);
        await path.WriteAsync(writer);
    }

    public static IMazePath Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);
        var path = IMazePath.Read(reader, type);
        reader.BaseStream.EnsureCompleted();
        return path;
    }

    public static async Task<IMazePath> DeserializeAsync(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var type = DeserializeHeader(reader);
        var path = await IMazePath.ReadAsync(reader, type);
        reader.BaseStream.EnsureCompleted();
        return path;
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
