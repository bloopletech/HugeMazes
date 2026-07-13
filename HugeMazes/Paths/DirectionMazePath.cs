using System.Collections;
using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Paths;

public class DirectionMazePath(
    IStore store,
    int delta = 1,
    bool leaveOpen = false) : Storable(store, leaveOpen), IMazePath
{
    private LongList<MazeDirection> directions = new(store.Offset<Header>(true));
    private MazePoint start = MazePoint.Empty;
    private MazePoint end = MazePoint.Empty;
    private bool HasStart => start != MazePoint.Empty;

    public override long Extent => directions.Extent + Header.SizeOf;
    public long Count => HasStart ? directions.Count + 1 : 0; // Fencepost

    public MazePoint this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if((ulong)index >= (ulong)Count) ExceptionExtensions.ThrowOutOfRangeException(index);
            return index > (Count >> 2) ? GetFromEnd(index) : GetFromStart(index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MazePoint GetFromEnd(long index)
    {
        var value = end;
        for(var i = directions.Count - 1; i >= index; i--) value = value.PrevDirection(directions[i], delta);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MazePoint GetFromStart(long index)
    {
        var value = start;
        for(var i = 0; i < index; i++) value = value.NextDirection(directions[i], delta);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(MazePoint point)
    {
        if(!HasStart) start = point;
        else directions.Add(MazePoint.CalcDirection(end, point));
        end = point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        directions.Clear();
        end = start = MazePoint.Empty;
    }

    public bool Contains(MazePoint point)
    {
        foreach(var item in this)
        {
            if(item == point) return true;
        }

        return false;
    }

    public IEnumerator<MazePoint> GetEnumerator()
    {
        var point = start;
        if(HasStart) yield return point;
        foreach(var direction in directions)
        {
            point = point.NextDirection(direction, delta);
            yield return point;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long IndexOf(MazePoint point)
    {
        for(var i = 0; i < Count; i++)
        {
            if(this[i] == point) return i;
        }

        return -1;
    }

    public MazePoint Pop()
    {
        if(!HasStart) ExceptionExtensions.ThrowOutOfRangeException(0);
        var prevEnd = end;
        if(directions.Count > 0) end = end.PrevDirection(directions.Pop(), delta);
        else end = start = MazePoint.Empty;
        return prevEnd;
    }

    public void Push(MazePoint point) => Add(point);

    public MazePoint Peek()
    {
        if(!HasStart) ExceptionExtensions.ThrowOutOfRangeException(0);
        return end;
    }

    public override void Read()
    {
        (start, end, delta) = store.Read<Header>(0);
        directions = new(store.Offset<Header>(true));
        directions.Read();
    }

    public override void Write()
    {
        store.Write(0, new Header(start, end, delta));
        directions.Write();
    }

    public IMazePath Clone() => Clone(IStore.Create(IsLong));

    public IMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new MazePath(destination, leaveOpen);
        result.Read();
        return result;
    }

    private record struct Header(MazePoint Start, MazePoint End, int Delta)
    {
        public static readonly int SizeOf = IStore.SizeOf<Header>();
    }
}
