using System.Text;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class MazeSerializer
{
    public static readonly long Magic = BitConverter.ToInt64(Encoding.ASCII.GetBytes("DEVEMAZE"));
    public const ushort Version = 1;

    private static MazeType ReadHeader(IStore store)
    {
        var header = store.Read<MazeHeader>(0);
        var (magic, version, type) = header;

        if(magic != Magic) throw new InvalidDataException("Magic header not present");
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return type;
    }

    private static void WriteHeader(IStore store, MazeType type)
    {
        store.Write(0, new MazeHeader(Magic, Version, type));
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
        WriteHeader(store, type);
        return InitForWrite(type, store.Offset<MazeHeader>(), size);
    }

    private static IMaze InitForRead(MazeType type, IStore store) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(store),
        MazeType.LongBitGridMaze => new LongBitGridMaze(store),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMaze InitForWrite(MazeType type, IStore store, MazeSize size) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(store, size),
        MazeType.LongBitGridMaze => new LongBitGridMaze(store, size),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static MazeType DetermineMazeType(MazeSize size)
    {
        return size.Area > int.MaxValue ? MazeType.LongBitGridMaze : MazeType.BitGridMaze;
    }
}
