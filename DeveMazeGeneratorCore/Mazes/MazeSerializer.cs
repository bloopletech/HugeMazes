using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

public class MazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    private static MazeType ReadHeader(IStore store)
    {
        var magic = store.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = store.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return (MazeType)store.ReadUInt16();
    }

    private static void WriteHeader(IStore store, MazeType type)
    {
        store.Write(MagicHeader);
        store.Write(Version);
        store.Write((ushort)type);
    }

    public static IMaze Read(IStore store)
    {
        var type = ReadHeader(store);
        return InitForRead(type, store);
    }

    public static IMaze Create(MazeType type, IStore store, Size size)
    {
        WriteHeader(store, type);
        return InitForWrite(type, store, size);
    }

    private static IMaze InitForRead(MazeType type, IStore store) => type switch
    {
        MazeType.BitGridMaze => BitGridMaze.Read(store.WithPosition()),
        MazeType.LongBitGridMaze => LongBitGridMaze.Read(store.WithPosition()),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMaze InitForWrite(MazeType type, IStore store, Size size) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(store.WithPosition(), size),
        MazeType.LongBitGridMaze => new LongBitGridMaze(store.WithPosition(), size),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static MazeType DetermineMazeType(Size size)
    {
        return size.Area > int.MaxValue ? MazeType.LongBitGridMaze : MazeType.BitGridMaze;
    }
}
