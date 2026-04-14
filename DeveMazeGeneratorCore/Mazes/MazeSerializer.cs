using DeveMazeGeneratorCore.IO;

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
        var maze = Init(type, store, 0, 0);
        maze.Read();
        return maze;
    }

    public static async Task<IMaze> ReadAsync(IStore store)
    {
        var type = ReadHeader(store);
        var maze = Init(type, store, 0, 0);
        await maze.ReadAsync();
        return maze;
    }

    public static IMaze Create(MazeType type, IStore store, int width, int height)
    {
        WriteHeader(store, type);
        return Init(type, store, width, height);
    }

    private static IMaze Init(MazeType type, IStore store, int width, int height) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(store.WithPosition(), width, height),
        MazeType.BigBitGridMaze => new BigBitGridMaze(store.WithPosition(), width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static MazeType DetermineMazeType(int width, int height)
    {
        return ((long)width * height) > int.MaxValue ? MazeType.BigBitGridMaze : MazeType.BitGridMaze;
    }
}
