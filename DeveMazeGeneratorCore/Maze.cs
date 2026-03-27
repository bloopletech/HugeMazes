using System.Text;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public abstract class Maze(int width, int height) : IMaze
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    public int Width => width;
    public int Height => height;

    //abstract must be overidden
    public abstract bool this[int x, int y] { get; set; }

    /// <summary>
    /// Clones the map into either an instance of BitArreintjeFastInnerMap or 
    /// an exact copy of itself (if this is implemented by the child class)
    /// </summary>
    /// <returns>Cloned inner map</returns>
    public virtual Maze Clone()
    {
        var innerMapTarget = new BitArreintjeFastInnerMap(Width, Height);
        CloneInto(innerMapTarget);
        return innerMapTarget;
    }

    /// <summary>
    /// This method makes a copy of the maze.
    /// </summary>
    /// <param name="mapTarget">The map to clone into</param>
    /// <returns>The cloned maze</returns>
    public void CloneInto(Maze mapTarget)
    {
        if(Width != mapTarget.Width) throw new ArgumentException($"Width of the target ({mapTarget.Width}) is not equal to that of the source ({Width}).");
        if(Height != mapTarget.Height) throw new ArgumentException($"Height of the target ({mapTarget.Height}) is not equal to that of the source ({Height}).");

        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                mapTarget[x, y] = this[x, y];
            }
        }
    }

    //virtual can be overidden
    public virtual string GenerateMapAsString()
    {
        var stringBuilder = new StringBuilder();
        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                bool b = this[x, y];
                if(b)
                {
                    stringBuilder.Append(' ');
                }
                else
                {
                    stringBuilder.Append('0');
                }
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }

    public static async Task<Maze> Read(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != 1) throw new InvalidDataException($"Maze version is {version} but we only understand version 1");

        var type = reader.ReadInt16();
        if(type == ContiguousArrayMaze.TypeId)
        {
            var maze = new ContiguousArrayMaze(reader);
            await maze.Read(reader);
            return maze;
        }
        else
        {
            throw new InvalidDataException($"Unknown maze type {type}");
        }
    }

    protected abstract Task Read(BinaryReader reader);

    public static async Task<Maze> Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await Read(fs);
    }

    public virtual async Task Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(MagicHeader);
        writer.Write(Version);
        await Write(writer);
    }

    protected abstract Task Write(BinaryWriter writer);

    public virtual async Task Save(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Write(fs);
    }
}
