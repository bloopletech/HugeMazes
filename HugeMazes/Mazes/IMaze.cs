using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Mazes;

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
}