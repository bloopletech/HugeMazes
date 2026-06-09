using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct MazeHeader(long Magic, ushort Version, MazeType MazeType)
{
    public static readonly int SizeOf = IStore.SizeOf<MazeHeader>();
}
