namespace DeveMazeGeneratorCore.Mazes;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public interface IMaze
{
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMaze Clone();
    void Write(BinaryWriter writer);
    Task WriteAsync(BinaryWriter writer);

    public static IMaze Read(MazeType type, BinaryReader reader) => type switch
    {
        MazeType.BitGridMaze => BitGridMaze.Read(reader),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static async Task<IMaze> ReadAsync(MazeType type, BinaryReader reader) => type switch
    {
        MazeType.BitGridMaze => await BitGridMaze.ReadAsync(reader),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static IMaze Create(MazeType type, int width, int height) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}