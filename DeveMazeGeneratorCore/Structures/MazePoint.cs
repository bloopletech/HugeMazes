namespace DeveMazeGeneratorCore.Structures;

/// <summary>
/// Contains a position.
/// Note: Struct really is faster then class
/// </summary>
public struct MazePoint(int X, int Y)
{
    public int X = X, Y = Y;

    public override string ToString()
    {
        return $"MazePoint(X: {X} Y: {Y})";
    }
}
