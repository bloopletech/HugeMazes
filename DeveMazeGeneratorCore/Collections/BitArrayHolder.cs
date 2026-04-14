using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public class BitArrayHolder(IBinarySerializer serializer, long offset, long size)
{
    private BitArray? array;

    public long LastUsedAt { get; set; } = long.MaxValue;

    public bool this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            LastUsedAt = Environment.TickCount64;
            return array![index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            LastUsedAt = Environment.TickCount64;
            array![index] = value;
        }
    }

    public bool IsEmpty => array == null;
    public bool IsPresent => array != null;
    public long Start => offset;
    public long End => offset + size;

    public void Load()
    {
        if(array != null) return;
        array = new BitArray((int)size * 8);
        serializer.ReadExactly(offset, array.GetArray());
        //LastUsedAt = Environment.TickCount;
    }

    public void Evict()
    {
        if(array == null) return;
        serializer.Write(offset, array.GetArray());
        array = null;
        //LastUsedAt = long.MaxValue;
    }
}
