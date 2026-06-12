using System.Collections;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public interface ILongBitArray : IEnumerable
{
    bool this[long index] { get; set; }

    long Length { get; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }

    bool Peek();
    bool[] ToArray();
    IList<bool> ToList();
    ILongBitArray Clone();
    ILongBitArray Clone(IStore destination, bool leaveOpen = false);
}