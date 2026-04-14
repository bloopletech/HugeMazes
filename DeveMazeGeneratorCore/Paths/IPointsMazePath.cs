using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public interface IPointsMazePath : IMazePath
{
    IBigList<MazePoint> Points { get; }

    IPointsMazePath Clone();
}
