namespace DeveMazeGeneratorCore.Paths;

public interface IMazePath
{
    bool this[int x, int y] { get; set; }

    IMazePath Clone();
    void Highlight();
    void Write(BinaryWriter writer);
    Task WriteAsync(BinaryWriter writer);

    public static IMazePath Read(BinaryReader reader, short type) => type switch
    {
        MazePath.TypeId => MazePath.Read(reader),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static async Task<IMazePath> ReadAsync(BinaryReader reader, short type) => type switch
    {
        MazePath.TypeId => await MazePath.ReadAsync(reader),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };
}