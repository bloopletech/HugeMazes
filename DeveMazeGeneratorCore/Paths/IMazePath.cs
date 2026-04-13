namespace DeveMazeGeneratorCore.Paths;

public interface IMazePath
{
    bool this[int x, int y] { get; set; }

    IMazePath Clone();
    void Highlight();
    void Write(BinaryWriter writer);
    Task WriteAsync(BinaryWriter writer);

    public static IMazePath Read(MazePathType type, BinaryReader reader) => type switch
    {
        MazePathType.MazePath => MazePath.Read(reader),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static async Task<IMazePath> ReadAsync(MazePathType type, BinaryReader reader) => type switch
    {
        MazePathType.MazePath => await MazePath.ReadAsync(reader),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static IMazePath Create(MazePathType type, int width, int height) => type switch
    {
        MazePathType.MazePath => new MazePath(width, height),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };
}