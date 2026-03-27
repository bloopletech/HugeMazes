using System.Collections;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class BitList
{
    private const int ChunkSize = 4096;

    private readonly BitArray array;

    public BitList(int bitLength)
    {
        array = new(bitLength);
    }

    public BitList(BitList source)
    {
        array = new(source.array);
    }

    public bool this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[i];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[i] = value;
    }

    public async Task Read(Stream stream)
    {
        var rawArray = GetRawArray();
        for(var offset = 0; offset < rawArray.Length;)
        {
            var length = Math.Min(ChunkSize, rawArray.Length - 1 - offset);
            if(length == 0) break;
            var bytesRead = await stream.ReadAsync(rawArray.AsMemory(offset, length));
            //if(bytesRead == 0) throw new InvalidOperationException("Stream ended too early");
            offset += bytesRead;
        }
    }

    public async Task Write(Stream stream)
    {
        var rawArray = GetRawArray();
        for(var offset = 0; offset < rawArray.Length; offset += ChunkSize)
        {
            var length = Math.Min(ChunkSize, rawArray.Length - 1 - offset);
            await stream.WriteAsync(rawArray.AsMemory(offset, length));
        }
    }

    private ref byte[] GetRawArray() => ref GetArrayField(array);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);
}
