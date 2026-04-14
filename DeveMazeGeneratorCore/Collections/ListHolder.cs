using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Collections;

public class ListHolder<T>(BigList<T> owner, long offset, long start = 0, int count = 0) where T : struct
{
    public static readonly int ItemSize = IBinarySerializer.SizeOf<T>();

    private List<T>? list;

    public List<T> List
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            LastUsedAt = Environment.TickCount64;
            list ??= Load();
            return list;
        }
    }

    public long LastUsedAt { get; set; } = long.MinValue;

    public bool IsPresent => list != null;

    public long EndOffset => offset + (Count * ItemSize);

    public long Start => start;
    public int Count => list?.Count ?? count;
    public long End => Start + Count;

    private List<T> Load()
    {
        owner.EvictOldest();
        var list = new List<T>(count);
        owner.Serializer.Read(offset, list.GetArray());
        return list;
    }

    public void Evict()
    {
        if(list == null) return;

        owner.Serializer.Write(offset, list.GetCurrentArray());
        count = list.Count;
        list = null;
        LastUsedAt = long.MinValue;
    }

    public ListHolder<T> Next(int count = 0) => new(owner, EndOffset, End, count);
}
