using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public sealed class MazePath(BigList<MazePoint> points) : IPointsMazePath
{
    public MazePath(IStore store, bool leaveOpen = false) : this(new BigList<MazePoint>(store, leaveOpen))
    {
    }

    public IStore Store => points.Store;
    public bool IsBig => points.IsBig;
    public long Extent => points.Extent;
    public IBigList<MazePoint> Points => points;

    public void Read() => points.Read();

    public async Task ReadAsync() => await points.ReadAsync();

    public void Write() => points.Write();

    public async Task WriteAsync() => await points.WriteAsync();

    public void Dispose() => points.Dispose();

    public IPointsMazePath Clone() => Clone(IStore.Create(IsBig));

    public IPointsMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        return new MazePath((BigList<MazePoint>)points.Clone(destination, leaveOpen));
    }
}
