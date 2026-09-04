using System.Runtime.InteropServices;
using HugeMazes.IO;

namespace HugeMazes.Mazes;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MazeHeader(long MagicHuman, long MagicBinary, ushort Version, MazeType MazeType)
{
    public static readonly int SizeOf = IStore.SizeOf<MazeHeader>();
}
