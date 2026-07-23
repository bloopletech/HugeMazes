using System.Drawing;
using System.Runtime.InteropServices;
using HugeMazes.IO;

namespace HugeMazes.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MazeColor(byte R, byte G, byte B)
{
    public static implicit operator MazeColor(Color color) => new(color.R, color.G, color.B);
    public static readonly int SizeOf = IStore.SizeOf<MazeColor>();
}
