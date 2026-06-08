using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Paths;

public static class MazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    private static MazePathType ReadHeader(IStore store)
    {
        var magic = store.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = store.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return (MazePathType)store.ReadUInt16();
    }

    private static void WriteHeader(IStore store, MazePathType type)
    {
        store.Write(MagicHeader);
        store.Write(Version);
        store.Write((ushort)type);
    }

    public static IMazePath Read(IStore store)
    {
        var type = ReadHeader(store);
        var path = InitForRead(type, store);
        path.Read();
        return path;
    }

    public static async Task<IMazePath> ReadAsync(IStore store)
    {
        var type = ReadHeader(store);
        var path = InitForRead(type, store);
        await path.ReadAsync();
        return path;
    }

    public static IMazePath Create(MazePathType type, IStore store, int width, int height)
    {
        WriteHeader(store, type);
        return InitForWrite(type, store, width, height);
    }

    private static IMazePath InitForRead(MazePathType type, IStore store) => type switch
    {
        MazePathType.MazePath => new MazePath(store.WithPosition()),
        MazePathType.BitGridMazePath => new BitGridMazePath(store.WithPosition()),
        MazePathType.BigBitGridMazePath => new BigBitGridMazePath(store.WithPosition()),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    private static IMazePath InitForWrite(
        MazePathType type,
        IStore store,
        int width,
        int height) => type switch
    {
        MazePathType.MazePath => new MazePath(store.WithPosition()),
        MazePathType.BitGridMazePath => new BitGridMazePath(store.WithPosition(), width, height),
        MazePathType.BigBitGridMazePath => new BigBitGridMazePath(store.WithPosition(), width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
