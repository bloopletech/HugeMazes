namespace DeveMazeGeneratorCore.Structures;

/// <summary>
/// Contains a position.
/// Note: Struct really is faster then class
/// </summary>
public readonly record struct ImmutableMazePoint(int X, int Y);
