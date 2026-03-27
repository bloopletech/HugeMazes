namespace DeveMazeGeneratorCore.Algorithms;

public abstract class Algorithm(Maze maze, Random random) : IAlgorithm
{
    public Maze Maze => maze;
    public Random Random => random;

    public abstract void Generate();
}
