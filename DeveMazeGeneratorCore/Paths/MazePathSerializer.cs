using System.Text;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public static class MazePathSerializer
{
    public static readonly long Magic = BitConverter.ToInt64(Encoding.ASCII.GetBytes("DEVEPATH"));
    public const ushort Version = 1;

    private static MazePathType ReadHeader(IStore store)
    {
        var header = store.Read<MazePathHeader>(0);
        var (magic, version, type) = header;

        if(magic != Magic) throw new InvalidDataException("Magic header not present");
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return type;
    }

    private static void WriteHeader(IStore store, MazePathType type)
    {
        store.Write(0, new MazePathHeader(Magic, Version, type));
    }

    public static IMazePath Read(IStore store)
    {
        var type = ReadHeader(store);
        var result = InitForRead(type, store.Offset<MazePathHeader>());
        result.Read();
        return result;
    }

    public static IMazePath Create(MazePathType type, IStore store, Size size)
    {
        WriteHeader(store, type);
        return InitForWrite(type, store.Offset<MazePathHeader>(), size);
    }

    private static IMazePath InitForRead(MazePathType type, IStore store) => type switch
    {
        MazePathType.MazePath => new MazePath(store),
        MazePathType.DirectionMazePath => new DirectionMazePath(store),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMazePath InitForWrite(MazePathType type, IStore store, Size size) => type switch
    {
        MazePathType.MazePath => new MazePath(store, size),
        MazePathType.DirectionMazePath => new DirectionMazePath(store, size),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
