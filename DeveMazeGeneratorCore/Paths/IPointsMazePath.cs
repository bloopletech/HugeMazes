using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public interface IPointsMazePath : IMazePath
{
    ILongList<MazePoint> Points { get; }

    IPointsMazePath Clone();
}
