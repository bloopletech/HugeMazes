using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid : IBitGrid, IBinarySerializable
{
    private readonly IBinarySerializer serializer;
    private readonly long offset;
    private int width;
    private int height;
    private BitArray array;

    public BitGrid(IBinarySerializer serializer, long offset)
    {
        this.serializer = serializer;
        this.offset = offset;
        array = new BitArray(0);
    }

    public BitGrid(IBinarySerializer serializer, long offset, int width, int height)
    {
        this.serializer = serializer;
        this.offset = offset;

        this.width = width;
        this.height = height;
        array = new BitArray(width * height);
    }

    private BitGrid(IBinarySerializer serializer, long offset, int width, int height, BitArray array)
    {
        this.serializer = serializer;
        this.offset = offset;

        this.width = width;
        this.height = height;
        this.array = array;
    }

    public IBinarySerializer Serializer => serializer;
    public long Offset => offset;
    public int Width => width;
    public int Height => height;

    public IBitGrid Clone() => Clone(serializer.Create(), 0);

    public IBitGrid Clone(IBinarySerializer serializer, long offset) =>
        new BitGrid(serializer, offset, width, height, new(array));

    public async Task<IBitGrid> CloneAsync() => Clone();

    public async Task<IBitGrid> CloneAsync(IBinarySerializer serializer, long offset) => Clone(serializer, offset);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * height)] = value;
    }

    public void Read()
    {
        serializer.Position = offset;

        width = serializer.ReadInt32();
        height = serializer.ReadInt32();

        array = new BitArray(serializer.ReadInt32());
        serializer.ReadExactly(array.GetArray());
    }

    public async Task ReadAsync()
    {
        serializer.Position = offset;

        width = serializer.ReadInt32();
        height = serializer.ReadInt32();

        array = new BitArray(serializer.ReadInt32());
        await serializer.ReadExactlyAsync(array.GetArray());
    }

    public void Write()
    {
        serializer.Position = offset;

        serializer.Write(width);
        serializer.Write(height);

        serializer.Write(array.Length);
        serializer.Write(array.GetArray());
        //serializer.WriteArray(array.GetArray());
    }

    public async Task WriteAsync()
    {
        serializer.Position = offset;

        serializer.Write(width);
        serializer.Write(height);

        serializer.Write(array.Length);
        await serializer.WriteAsync(array.GetArray());
        //await serializer.WriteArrayAsync(array.GetArray());
    }
}
