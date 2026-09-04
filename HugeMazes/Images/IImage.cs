using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Images;

public interface IImage : IStorable
{
    Guid MazeId { get; }
    MazeSize Size { get; }
    int Height { get; }
    int Width { get; }
}

public interface IImage<T> : IImage where T : struct
{
    T this[int x, int y] { get; set; }
}