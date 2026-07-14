using System.Drawing;
using System.Runtime.InteropServices;

namespace HugeMazes.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MazeColor(byte R, byte G, byte B)
{
    public static implicit operator MazeColor(Color color) => new(color.R, color.G, color.B);
}
