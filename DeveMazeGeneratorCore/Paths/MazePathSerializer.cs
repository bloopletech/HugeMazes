using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Paths;

public static class MazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    private static MazePathType ReadHeader(IBinarySerializer serializer)
    {
        var magic = serializer.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = serializer.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return (MazePathType)serializer.ReadUInt16();
    }

    private static void WriteHeader(IBinarySerializer serializer, MazePathType type)
    {
        serializer.Write(MagicHeader);
        serializer.Write(Version);
        serializer.Write((ushort)type);
    }

    public static IMazePath Read(IBinarySerializer serializer)
    {
        var type = ReadHeader(serializer);
        var path = InitForRead(type, serializer);
        path.Read();
        return path;
    }

    public static async Task<IMazePath> ReadAsync(IBinarySerializer serializer)
    {
        var type = ReadHeader(serializer);
        var path = InitForRead(type, serializer);
        await path.ReadAsync();
        return path;
    }

    private static IMazePath InitForRead(MazePathType type, IBinarySerializer serializer) => type switch
    {
        MazePathType.MazePath => new MazePath(serializer, serializer.Position),
        MazePathType.BitGridMazePath => new BitGridMazePath(serializer, serializer.Position),
        MazePathType.BigBitGridMazePath => new BigBitGridMazePath(serializer, serializer.Position),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static IMazePath Create(MazePathType type, IBinarySerializer serializer, int width, int height)
    {
        WriteHeader(serializer, type);
        return InitForWrite(type, serializer, width, height);
    }

    private static IMazePath InitForWrite(
        MazePathType type,
        IBinarySerializer serializer,
        int width,
        int height) => type switch
    {
        MazePathType.MazePath => new MazePath(serializer, serializer.Position),
        MazePathType.BitGridMazePath => new BitGridMazePath(serializer, serializer.Position, width, height),
        MazePathType.BigBitGridMazePath => new BigBitGridMazePath(serializer, serializer.Position, width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
