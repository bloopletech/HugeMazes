using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Paths;

public interface IMazePath : IEnumerable<MazePoint>, IStorable
{
    Guid MazeId { get; }
    long Count { get; }
    MazePoint this[long index] { get; }
    void Add(MazePoint point);
    void Clear();
    bool Contains(MazePoint point);
    long IndexOf(MazePoint point);
    MazePoint Pop();
    void PopIgnore();
    void Push(MazePoint point);
    MazePoint Peek();

    IMazePath Clone();
    IMazePath Clone(IStore destination, bool leaveOpen = false);

    public static void EnsureRelated(IMaze maze, IMazePath path)
    {
        if(maze.Id != path.MazeId) throw new ArgumentException("path.MazeId must match maze.Id");
    }
}