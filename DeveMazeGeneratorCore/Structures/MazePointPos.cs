using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.Structures;

/// <summary>
/// Contains a position with a byte that describes how far in the maze this point is (to determine the color).
/// Note: Struct really is faster then class
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)] //This is required so this struct uses 9 bytes instead of 12
public readonly record struct MazePointPos(int X, int Y, byte RelativePos = 0)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(BinaryWriter writer)
    {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(RelativePos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MazePointPos Read(BinaryReader reader) => new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadByte());
};