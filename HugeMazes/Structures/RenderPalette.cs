using System.Drawing;

namespace HugeMazes.Structures;

public readonly record struct RenderPalette(
    MazeColor Background,
    MazeColor Wall,
    MazeColor Path,
    MazeColor? Start = null,
    MazeColor? End = null)
{
    public static readonly RenderPalette Default = new(Color.White, Color.Black, Color.Lime, Color.Red, Color.Blue);

    public MazeColor?[] ToArray() => [Background, Wall, Path, Start, End];

    public Index Indexed => new(this);

    public readonly struct Index(RenderPalette owner)
    {
        public readonly MazeColor[] Palette => [..owner.ToArray().OfType<MazeColor>()];

        public const byte Background = 0;
        public const byte Wall = 1;
        public const byte Path = 2;
        public const byte Start = 3;
        public const byte End = 4;
    }
}
