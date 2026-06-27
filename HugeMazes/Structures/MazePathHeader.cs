using System.Runtime.InteropServices;
using HugeMazes.IO;
using HugeMazes.Paths;

namespace HugeMazes.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct MazePathHeader(long Magic, ushort Version, MazePathType MazePathType)
{
    public static readonly int SizeOf = IStore.SizeOf<MazePathHeader>();
}
