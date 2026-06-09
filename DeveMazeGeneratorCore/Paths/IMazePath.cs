using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public interface IMazePath : IEnumerable<MazePoint>, IStorable
{
    long Count { get; }
    MazePoint this[long index] { get; set; }
    void Add(MazePoint point);
    void Clear();
    bool Contains(MazePoint point);
    long IndexOf(MazePoint point);
    void Insert(long index, MazePoint point);
    bool Remove(MazePoint point);
    void RemoveAt(long index);
    MazePoint Pop();
    void Push(MazePoint point);
    MazePoint Shift();
    void Unshift(MazePoint point);
    MazePoint Peek();

    int Height { get; }
    int Width { get; }

    //ILongList<MazePoint> Points { get; }

    IMazePath Clone();
    IMazePath Clone(IStore destination, bool leaveOpen = false);
}