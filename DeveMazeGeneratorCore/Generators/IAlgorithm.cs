using DeveMazeGeneratorCore.Factories;
using DeveMazeGeneratorCore.InnerMaps;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Generators;

public interface IAlgorithm
{
    Maze GoGenerate<M>(int width, int height, int seed, IInnerMapFactory<M> mapFactory, IRandomFactory randomFactory) where M : InnerMap;
}
