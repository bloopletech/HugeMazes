using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public interface IPointsMazePath : IMazePath
{
    ILongList<MazePoint> Points { get; }

    IPointsMazePath Clone();
    IPointsMazePath Clone(IStore destination, bool leaveOpen = false);
}
