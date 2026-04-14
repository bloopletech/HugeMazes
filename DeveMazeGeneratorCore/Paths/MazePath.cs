using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class MazePath(BigList<MazePoint> points) : IPointsMazePath
{
    public MazePath(IBinarySerializer serializer, long offset) : this(new(serializer, offset))
    {
    }

    public IBinarySerializer Serializer => points.Serializer;
    public long Offset => points.Offset;
    public IBigList<MazePoint> Points => points;

    public IPointsMazePath Clone() => new MazePath((BigList<MazePoint>)points.Clone());

    public void Read()
    {
        points.Read();
    }

    public async Task ReadAsync()
    {
        await points.ReadAsync();
    }

    public void Write()
    {
        points.Write();
    }

    public async Task WriteAsync()
    {
        await points.WriteAsync();
    }
}
