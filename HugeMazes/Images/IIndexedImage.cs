using HugeMazes.Structures;

namespace HugeMazes.Images;

public interface IIndexedImage : IImage<byte>
{
    MazeColor[] Palette { get; }
}