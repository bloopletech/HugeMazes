using HugeMazes.IO;

namespace HugeMazes.Mazes;

public interface IBitGrid
{
    int Height { get; }
    int Width { get; }
    bool this[int x, int y] { get; set; }

    IBitGrid Clone();
    IBitGrid Clone(IStore destination, bool leaveOpen = false);
}