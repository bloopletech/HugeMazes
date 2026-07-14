using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

public interface IImage : IStorable
{
    MazeSize Size { get; }
    int Height { get; }
    int Width { get; }
}

public interface IImage<T> : IImage where T : struct
{
    T this[int x, int y] { get; set; }
}