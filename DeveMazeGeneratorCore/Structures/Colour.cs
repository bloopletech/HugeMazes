using System.Drawing;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct Colour(byte R, byte G, byte B)
{
    public static implicit operator Colour(Color color) => new(color.R, color.G, color.B);
}
