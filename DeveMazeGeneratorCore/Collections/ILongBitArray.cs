using System.Collections;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public interface ILongBitArray : IEnumerable
{
    long Length { get; }
    bool this[long index] { get; set; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }
    void Clear();
    bool Peek();
    ILongBitArray Clone();
    ILongBitArray Clone(IStore destination, bool leaveOpen = false);
}