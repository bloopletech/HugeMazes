using System.Collections;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Extensions;

public static class BitArrayExtensions
{
    //private const int ChunkSize = 4096;

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);

    private static ref byte[] GetArray(this BitArray array) => ref GetArrayField(array);

    public static async Task Write(this BitArray array, BinaryWriter writer)
    {
        writer.Write(array.Length);
        await writer.BaseStream.WriteAsync(array.GetArray());

        //var rawArray = GetRawArray();
        //await writer.BaseStream.WriteAsync(rawArray);
        //for (var offset = 0; offset < rawArray.Length; offset += ChunkSize)
        //{
        //    var length = Math.Min(ChunkSize, rawArray.Length - 1 - offset);
        //    await writer.BaseStream.WriteAsync(rawArray.AsMemory(offset, length));
        //}
    }

    public static async Task<BitArray> Read(BinaryReader reader)
    {
        var array = new BitArray(reader.ReadInt32());
        await reader.BaseStream.ReadExactlyAsync(array.GetArray());
        return array;
        //await reader.BaseStream.ReadExactlyAsync(array.GetArray().AsMemory());


        //for (var offset = 0; offset < rawArray.Length;)
        //{
        //    var length = Math.Min(ChunkSize, rawArray.Length - 1 - offset);
        //    if (length == 0) break;

        //    var bytesRead = await reader.BaseStream.ReadAsync(rawArray.AsMemory(offset, length));
        //    //if(bytesRead == 0) throw new InvalidOperationException("Stream ended too early");
        //    offset += bytesRead;
        //}
    }
}
