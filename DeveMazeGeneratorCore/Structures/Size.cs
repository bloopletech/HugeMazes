using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Structures;

public readonly record struct Size(int Width, int Height)
{
    public long Area => (long)Width * Height;
    public static readonly int SizeOf = IStore.SizeOf<Size>();
}
