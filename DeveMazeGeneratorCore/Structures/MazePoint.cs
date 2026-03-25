using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.Structures;

/// <summary>
/// Contains a position.
/// Note: Struct really is faster then class
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MazePoint(int X, int Y);
