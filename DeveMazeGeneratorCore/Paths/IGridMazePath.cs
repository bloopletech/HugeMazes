using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public interface IGridMazePath : IMazePath
{
    Size Size { get; }
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IGridMazePath Clone();
    IGridMazePath Clone(IStore destination, bool leaveOpen = false);
}