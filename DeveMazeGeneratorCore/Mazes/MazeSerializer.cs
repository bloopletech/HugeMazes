using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class MazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    private static MazeType ReadHeader(IBinarySerializer serializer)
    {
        var magic = serializer.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = serializer.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return (MazeType)serializer.ReadUInt16();
    }

    private static void WriteHeader(IBinarySerializer serializer, MazeType type)
    {
        serializer.Write(MagicHeader);
        serializer.Write(Version);
        serializer.Write((ushort)type);
    }

    public static IMaze Read(IBinarySerializer serializer)
    {
        var type = ReadHeader(serializer);
        var maze = InitForRead(type, serializer);
        maze.Read();
        return maze;
    }

    public static async Task<IMaze> ReadAsync(IBinarySerializer serializer)
    {
        var type = ReadHeader(serializer);
        var maze = InitForRead(type, serializer);
        await maze.ReadAsync();
        return maze;
    }

    private static IMaze InitForRead(MazeType type, IBinarySerializer serializer) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(serializer, serializer.Position),
        MazeType.BigBitGridMaze => new BigBitGridMaze(serializer, serializer.Position),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static IMaze Create(MazeType type, IBinarySerializer serializer, int width, int height)
    {
        WriteHeader(serializer, type);
        return InitForWrite(type, serializer, width, height);
    }

    private static IMaze InitForWrite(
        MazeType type,
        IBinarySerializer serializer,
        int width,
        int height) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(serializer, serializer.Position, width, height),
        MazeType.BigBitGridMaze => new BigBitGridMaze(serializer, serializer.Position, width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
