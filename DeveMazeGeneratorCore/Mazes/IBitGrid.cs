using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public interface IBitGrid
{
    bool this[int x, int y] { get; set; }

    int Height { get; }
    int Width { get; }

    IBitGrid Clone();
    IBitGrid Clone(IStore destination, bool leaveOpen = false);
}