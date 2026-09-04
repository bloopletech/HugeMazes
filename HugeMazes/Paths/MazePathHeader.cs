using System.Runtime.InteropServices;
using HugeMazes.IO;

namespace HugeMazes.Paths;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MazePathHeader(
    long MagicHuman,
    long MagicBinary,
    ushort Version,
    MazePathType MazePathType)
{
    public static readonly int SizeOf = IStore.SizeOf<MazePathHeader>();
}
