using SixLabors.ImageSharp;

namespace DeveMazeGeneratorCore.Structures;

public readonly record struct RenderColors(Color Background, Color Wall, Color Path, Color? Start = null, Color? End = null)
{
    public static readonly RenderColors Default = new(Color.White, Color.Black, Color.Lime, Color.Blue, Color.Red);
}
