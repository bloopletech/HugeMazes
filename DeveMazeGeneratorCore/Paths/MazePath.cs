using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public sealed class MazePath(LongList<MazePoint> points) : IPointsMazePath
{
    public MazePath(IStore store, bool leaveOpen = false) : this(new LongList<MazePoint>(store, 0, leaveOpen))
    {
    }

    public IStore Store => points.Store;
    public bool IsLong => points.IsLong;
    public long Extent => points.Extent;
    public ILongList<MazePoint> Points => points;

    public static MazePath Read(
        IStore store,
        bool leaveOpen = false) => new(LongList<MazePoint>.Read(store, leaveOpen));

    public void Write() => points.Write();

    public void Dispose() => points.Dispose();

    public IPointsMazePath Clone() => Clone(IStore.Create(IsLong));

    public IPointsMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        return new MazePath((LongList<MazePoint>)points.Clone(destination, leaveOpen));
    }
}
