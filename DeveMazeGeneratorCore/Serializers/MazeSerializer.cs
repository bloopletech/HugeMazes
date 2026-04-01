using System.Text;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Serializers;

public class MazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    public static async Task Write(Stream stream, IMaze maze)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(MagicHeader);
        writer.Write(Version);
        await maze.Write(writer);
    }

    public static async Task Save(string fileName, IMaze maze)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Write(fs, maze);
    }

    public static async Task<IMaze> Read(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != 1) throw new InvalidDataException($"Maze version is {version} but we only understand version 1");

        var type = reader.ReadInt16();
        if(type == ContiguousArrayMaze.TypeId)
        {
            var maze = await ContiguousArrayMaze.Read(reader);
            reader.BaseStream.EnsureCompleted();
            return maze;
        }
        else
        {
            throw new InvalidDataException($"Unknown maze type {type}");
        }
    }

    public static async Task<IMaze> Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await Read(fs);
    }

    public static string GenerateMapAsString(IMaze maze)
    {
        var stringBuilder = new StringBuilder();
        for (int y = 0; y < maze.Height; y++)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                bool b = maze[x, y];
                if (b)
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
}
