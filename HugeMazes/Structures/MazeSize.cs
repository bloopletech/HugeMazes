using HugeMazes.IO;

namespace HugeMazes.Structures;

public readonly record struct MazeSize(int Width, int Height)
{
    public long Area => (long)Width * Height;
    public static readonly int SizeOf = IStore.SizeOf<MazeSize>();
}
