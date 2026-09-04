using System.Runtime.InteropServices;
using HugeMazes.Extensions;
using HugeMazes.IO;

namespace HugeMazes.Mazes;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct MazeSize(int Width, int Height)
{
    public long Area => (long)Width * Height;
    public int WidthStride => Width.RoundUpEven();
    public static readonly int SizeOf = IStore.SizeOf<MazeSize>();
}
