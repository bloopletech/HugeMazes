using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes;

public interface IImage : IStorable
{
    MazeSize Size { get; }
    int Height { get; }
    int Width { get; }
    Colour this[int x, int y] { get; set; }
    IEnumerable<(int, int)> ByPixel();
}