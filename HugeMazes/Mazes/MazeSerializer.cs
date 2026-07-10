using System.Text;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

public class MazeSerializer
{
    public static readonly long MagicHuman = BitConverter.ToInt64(Encoding.ASCII.GetBytes("HGMZMAZE"));
    public const long MagicBinary = 8722170413477670132; // Just a randomly generated number
    public const ushort Version = 1;

    private static MazeType ReadHeader(IStore store)
    {
        var header = store.Read<MazeHeader>(0);
        var (magicHuman, magicBinary, version, type) = header;

        if(magicHuman != MagicHuman || magicBinary != MagicBinary)
        {
            throw new InvalidDataException("Invalid magic header present");
        }
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return type;
    }

    private static void WriteHeader(IStore store, MazeType type)
    {
        store.Write(0, new MazeHeader(MagicHuman, MagicBinary, Version, type));
    }

    public static IMaze Read(IStore store)
    {
        var type = ReadHeader(store);
        var result = InitForRead(type, store.Offset<MazeHeader>());
        result.Read();
        return result;
    }

    public static IMaze Create(IStore store, MazeType type, MazeSize size)
    {
        var maze = InitForWrite(type, store.Offset<MazeHeader>(), size);
        maze.EnsureDiskSpace();
        WriteHeader(store, type);
        return maze;
    }

    private static IMaze InitForRead(MazeType type, IStore store) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(store),
        MazeType.LongBitGridMaze => new LongBitGridMaze(store),
        MazeType.JaggedBitGridMaze => new JaggedBitGridMaze(store),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMaze InitForWrite(MazeType type, IStore store, MazeSize size) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(store, size),
        MazeType.LongBitGridMaze => new LongBitGridMaze(store, size),
        MazeType.JaggedBitGridMaze => new JaggedBitGridMaze(store, size),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static MazeType DetermineMazeType(MazeSize size)
    {
        return size.Area > int.MaxValue ? MazeType.LongBitGridMaze : MazeType.BitGridMaze;
    }
}
