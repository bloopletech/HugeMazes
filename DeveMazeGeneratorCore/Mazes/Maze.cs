using DeveMazeGeneratorCore.InnerMaps;

namespace DeveMazeGeneratorCore.Mazes;

public class Maze(InnerMap innerMap)
{
    public InnerMap InnerMap { get; } = innerMap;
}
