using System.Text;
using HugeMazes.IO;

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

    public static IMaze Create(IStore store, MazeType type, Guid id, MazeSize size)
    {
        var maze = InitForWrite(type, store.Offset<MazeHeader>(), id, size);
        maze.EnsureDiskSpace();
        WriteHeader(store, type);
        return maze;
    }

    private static IMaze InitForRead(MazeType type, IStore store) => type switch
    {
        MazeType.Maze => new Maze(store),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMaze InitForWrite(MazeType type, IStore store, Guid id, MazeSize size) => type switch
    {
        MazeType.Maze => new Maze(store, id, size),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
