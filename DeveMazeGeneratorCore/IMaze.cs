namespace DeveMazeGeneratorCore;

public interface IMaze
{
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    Maze Clone();
    void CloneInto(Maze mapTarget);
    string GenerateMapAsString();
    Task Write(Stream stream);
}