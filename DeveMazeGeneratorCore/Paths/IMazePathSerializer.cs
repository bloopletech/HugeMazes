using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Paths;

public static class IMazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    public static void Serialize(Stream stream, IMazePath path)
    {
        SerializeHeader(stream);
        path.Write(stream);
    }

    public static async Task SerializeAsync(Stream stream, IMazePath path)
    {
        SerializeHeader(stream);
        await path.WriteAsync(stream);
    }

    private static void SerializeHeader(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write(MagicHeader);
        writer.Write(Version);
    }

    public static IMazePath Deserialize(Stream stream)
    {
        var type = DeserializeHeader(stream);
        var path = IMazePath.Read(type, stream);
        stream.EnsureCompleted();
        return path;
    }

    public static async Task<IMazePath> DeserializeAsync(Stream stream)
    {
        var type = DeserializeHeader(stream);
        var path = await IMazePath.ReadAsync(type, stream);
        stream.EnsureCompleted();
        return path;
    }

    private static MazePathType DeserializeHeader(Stream stream)
    {
        using var reader = stream.Reader();
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return (MazePathType)reader.ReadUInt16();
    }
}
