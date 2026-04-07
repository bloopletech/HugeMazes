using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid
{
    public readonly int width;
    public readonly int height;
    private readonly BitArray array;

    public BitGrid(int width, int height) : this(width, height, new(width * height))
    {
    }

    public BitGrid(BitGrid source) : this(source.width, source.height, new(source.array))
    {
    }

    private BitGrid(int width, int height, BitArray array)
    {
        var length = width * height;
        if (length != array.Length)
        {
            throw new ArgumentException($"(width {width} * height {height}) {length} != array length {array.Length}");
        }

        this.width = width;
        this.height = height;
        this.array = array;
    }

    public int Width => width;
    public int Height => height;

    public BitGrid Clone() => new(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * height)] = value;
    }

    public async Task Write(BinaryWriter writer)
    {
        writer.Write(width);
        writer.Write(height);
        await array.Write(writer);
    }

    public static async Task<BitGrid> Read(BinaryReader reader)
    {
        return new BitGrid(reader.ReadInt32(), reader.ReadInt32(), await BitArray.Read(reader));
    }
}
