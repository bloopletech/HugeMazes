namespace DeveMazeGeneratorCore.Paths;

public interface IMazePath
{
    bool this[int x, int y] { get; set; }

    IMazePath Clone();
    void Highlight();
    void Write(Stream stream);
    Task WriteAsync(Stream stream);

    public static IMazePath Read(MazePathType type, Stream stream) => type switch
    {
        MazePathType.MazePath => MazePath.Read(stream),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static async Task<IMazePath> ReadAsync(MazePathType type, Stream stream) => type switch
    {
        MazePathType.MazePath => await MazePath.ReadAsync(stream),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static IMazePath Create(MazePathType type, int width, int height) => type switch
    {
        MazePathType.MazePath => new MazePath(width, height),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };
}