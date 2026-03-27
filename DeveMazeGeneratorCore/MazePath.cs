using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public class MazePath(MazePoint[] points)
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;
    public const short TypeId = 1;

    public MazePoint[] Points => points;

    public void Highlight()
    {
        //var points = new MazePointPos[stack.Count];

        //foreach(var item in stack)
        //{
        //    byte formulathing = (byte)(points.Count / (double)stack.Count * 255.0);
        //    points.Add(new MazePointPos(item.X, item.Y, formulathing));
        //}
    }

    public async Task Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(MagicHeader);
        writer.Write(Version);
        writer.Write(TypeId);

        writer.Write(points.Length);
        foreach(var point in points)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }
    }

    public async Task Save(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Write(fs);
    }

    public static MazePath Read(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != 1) throw new InvalidDataException($"Path version is {version} but we only understand version 1");

        var type = reader.ReadInt16();
        if(type != TypeId) throw new InvalidDataException($"Unknown path type {type}");

        var points = new MazePoint[reader.ReadInt32()];
        for(var i = 0; i < points.Length; i++) points[i].Set(reader.ReadInt32(), reader.ReadInt32());

        return new MazePath(points);
    }

    public static async Task<MazePath> Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return Read(fs);
    }
}
