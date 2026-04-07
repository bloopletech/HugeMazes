using System.Collections;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Extensions;

public static class BitArrayExtensions
{
    //private const int ChunkSize = 4096;

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);

    extension(BitArray array)
    {
        public async Task Write(BinaryWriter writer)
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

        private ref byte[] GetArray() => ref GetArrayField(array);

        public static async Task<BitArray> Read(BinaryReader reader)
        {
            var result = new BitArray(reader.ReadInt32());
            await reader.BaseStream.ReadExactlyAsync(result.GetArray());
            return result;
            //await reader.BaseStream.ReadExactlyAsync(result.GetArray().AsMemory());


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

}
