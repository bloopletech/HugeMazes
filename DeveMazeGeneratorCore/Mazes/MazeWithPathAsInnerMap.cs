using DeveMazeGeneratorCore.InnerMaps;

namespace DeveMazeGeneratorCore.Mazes;

public class MazeWithPathAsInnerMap(InnerMap innerMap, InnerMap pathMap) : Maze(innerMap)
{
    public InnerMap PathMap { get; } = pathMap;
}
