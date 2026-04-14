using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.Structures;

/// <summary>
/// Contains a position.
/// Note: Struct really is faster then class
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct MazePoint(int X, int Y)
{
    public static readonly MazePoint Empty = new(-1, -1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int x, int y)
    {
        X = x;
        Y = y;
    }
}
