using System.Text;
using HugeMazes.IO;

namespace HugeMazes.Paths;

public static class MazePathSerializer
{
    public static readonly long MagicHuman = BitConverter.ToInt64(Encoding.ASCII.GetBytes("HGMZPATH"));
    public const long MagicBinary = 6457225418376450318; // Just a randomly generated number
    public const ushort Version = 1;

    private static MazePathType ReadHeader(IStore store)
    {
        var header = store.Read<MazePathHeader>(0);
        var (magicHuman, magicBinary, version, type) = header;

        if(magicHuman != MagicHuman || magicBinary != MagicBinary)
        {
            throw new InvalidDataException("Invalid magic header present");
        }
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return type;
    }

    private static void WriteHeader(IStore store, MazePathType type)
    {
        store.Write(0, new MazePathHeader(MagicHuman, MagicBinary, Version, type));
    }

    public static IMazePath Read(IStore store)
    {
        var type = ReadHeader(store);
        var result = InitForRead(type, store.Offset<MazePathHeader>());
        result.Read();
        return result;
    }

    public static IMazePath Create(IStore store, MazePathType type, Guid mazeId)
    {
        var path = InitForWrite(type, store.Offset<MazePathHeader>(), mazeId);
        path.EnsureDiskSpace();
        WriteHeader(store, type);
        return path;
    }

    private static IMazePath InitForRead(MazePathType type, IStore store) => type switch
    {
        MazePathType.MazePath => new MazePath(store),
        MazePathType.DirectionMazePath => new DirectionMazePath(store),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMazePath InitForWrite(MazePathType type, IStore store, Guid mazeId) => type switch
    {
        MazePathType.MazePath => new MazePath(store, mazeId),
        MazePathType.DirectionMazePath => new DirectionMazePath(store, mazeId),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
