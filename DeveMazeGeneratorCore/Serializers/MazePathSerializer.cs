using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Serializers;

public class MazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    public static async Task Serialize(Stream stream, MazePath path)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(MagicHeader);
        writer.Write(Version);
        await path.Write(writer);
    }

    public static async Task Save(string fileName, MazePath path)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Serialize(fs, path);
    }

    public static async Task<MazePath> Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != 1) throw new InvalidDataException($"Path version is {version} but we only understand version 1");

        var type = reader.ReadInt16();
        if(type == MazePath.TypeId)
        {
            var path = await MazePath.Read(reader);
            reader.BaseStream.EnsureCompleted();
            return path;
        }
        else
        {
            throw new InvalidDataException($"Unknown path type {type}");
        }
    }

    public static async Task<MazePath> Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await Deserialize(fs);
    }
}
