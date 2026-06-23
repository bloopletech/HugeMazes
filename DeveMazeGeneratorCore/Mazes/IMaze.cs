using System.Text;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Mazes;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public interface IMaze : IStorable
{
    MazeSize Size { get; }
    int Height { get; }
    int Width { get; }
    bool this[int x, int y] { get; set; }

    IMaze Clone();
    IMaze Clone(IStore destination, bool leaveOpen = false);

    public void EnsureMinimumSize()
    {
        if(Width < 3) throw new ArgumentOutOfRangeException("maze.Width", Width, "Value must >= 3");
        if(Height < 3) throw new ArgumentOutOfRangeException("maze.Height", Height, "Value must >= 3");
    }

    public void EnsureOddSize()
    {
        if(int.IsEvenInteger(Width)) throw new ArgumentException("Value must be an odd number", "maze.Width");
        if(int.IsEvenInteger(Height)) throw new ArgumentException("Value must be an odd number", "maze.Height");
    }

    public string GenerateMapAsString()
    {
        var stringBuilder = new StringBuilder();
        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                bool b = this[x, y];
                if(b)
                {
                    stringBuilder.Append(' ');
                }
                else
                {
                    stringBuilder.Append('0');
                }
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }

    public static long MaxExtent(int width, int height) => ((long)width * height) + 256;
}