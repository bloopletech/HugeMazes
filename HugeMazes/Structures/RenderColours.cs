using System.Drawing;

namespace HugeMazes.Structures;

public readonly record struct RenderColours(
    Colour Background,
    Colour Wall,
    Colour Path,
    Colour? Start = null,
    Colour? End = null)
{
    public static readonly RenderColours Default = new(Color.White, Color.Black, Color.Lime, Color.Red, Color.Blue);
}
