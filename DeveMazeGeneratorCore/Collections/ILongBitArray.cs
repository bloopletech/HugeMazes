using System.Collections;

namespace DeveMazeGeneratorCore.Collections;

public interface ILongBitArray : IEnumerable
{
    bool this[long index] { get; set; }

    long Length { get; }
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }

    void Clear();
    bool Peek();
    bool[] ToArray();
    IList<bool> ToList();
    ILongBitArray Clone();
    Task<ILongBitArray> CloneAsync();
}