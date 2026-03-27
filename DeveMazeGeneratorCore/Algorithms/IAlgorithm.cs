namespace DeveMazeGeneratorCore.Algorithms;

public interface IAlgorithm
{
    Maze Maze { get; }
    Random Random { get; }

    void Generate();
}
