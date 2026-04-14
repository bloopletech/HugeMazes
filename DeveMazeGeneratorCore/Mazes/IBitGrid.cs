namespace DeveMazeGeneratorCore.Mazes;

public interface IBitGrid
{
    bool this[int x, int y] { get; set; }

    int Height { get; }
    int Width { get; }

    IBitGrid Clone();
    Task<IBitGrid> CloneAsync();
}