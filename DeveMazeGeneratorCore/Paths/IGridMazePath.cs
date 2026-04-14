namespace DeveMazeGeneratorCore.Paths;

public interface IGridMazePath : IMazePath
{
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IGridMazePath Clone();
}