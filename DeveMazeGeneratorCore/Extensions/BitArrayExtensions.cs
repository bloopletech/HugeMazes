using System.Collections;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Extensions;

public static class BitArrayExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);

    extension(BitArray array)
    {
        private ref byte[] GetArray() => ref GetArrayField(array);

        public void Write(BinaryWriter writer)
        {
            writer.Write(array.Length);
            writer.BaseStream.Write(array.GetArray());
        }

        public async Task WriteAsync(BinaryWriter writer)
        {
            writer.Write(array.Length);
            await writer.BaseStream.WriteAsync(array.GetArray());
        }

        public static BitArray Read(BinaryReader reader)
        {
            var result = new BitArray(reader.ReadInt32());
            reader.BaseStream.ReadExactly(result.GetArray());
            return result;
        }

        public static async Task<BitArray> ReadAsync(BinaryReader reader)
        {
            var result = new BitArray(reader.ReadInt32());
            await reader.BaseStream.ReadExactlyAsync(result.GetArray());
            return result;
        }
    }
}
