using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Paths;

public interface IMazePath : IEnumerable<MazePoint>, IStorable
{
    long Count { get; }
    MazePoint this[long index] { get; }
    void Add(MazePoint point);
    void Clear();
    bool Contains(MazePoint point);
    long IndexOf(MazePoint point);
    MazePoint Pop();
    void Push(MazePoint point);
    MazePoint Peek();

    IMazePath Clone();
    IMazePath Clone(IStore destination, bool leaveOpen = false);
}