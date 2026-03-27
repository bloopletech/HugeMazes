using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class ContiguousArrayMaze : Maze
{
    public const short TypeId = 1;

    private readonly BitList store;

    public ContiguousArrayMaze(int width, int height) : base(width, height)
    {
        store = new(width * height);
    }

    public ContiguousArrayMaze(ContiguousArrayMaze source) : base(source.Width, source.Height)
    {
        store = new(source.store);
    }

    public ContiguousArrayMaze(BinaryReader reader) : this(reader.ReadInt32(), reader.ReadInt32())
    {
    }

    public override Maze Clone() => new ContiguousArrayMaze(this);

    public override bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => store[x + (y * Height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => store[x + (y * Height)] = value;
    }

    protected override async Task Read(BinaryReader reader)
    {
        await store.Read(reader.BaseStream);
    }

    protected override async Task Write(BinaryWriter writer)
    {
        writer.Write(TypeId);
        writer.Write(Width);
        writer.Write(Height);
        await store.Write(writer.BaseStream);
    }
}
