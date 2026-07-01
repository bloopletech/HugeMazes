using System.Runtime.InteropServices;
using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct MazeHeader(long MagicHuman, long MagicBinary, ushort Version, MazeType MazeType)
{
    public static readonly int SizeOf = IStore.SizeOf<MazeHeader>();
}
