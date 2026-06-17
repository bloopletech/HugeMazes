using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class DirectionMazePath : Storable, IMazePath
{
    private Header header;
    private LongList<MazeDirection> directions;

    public DirectionMazePath(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        directions = null!;
    }

    public DirectionMazePath(IStore store, Size size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        header = new Header(size, MazePoint.Empty, MazePoint.Empty);
        directions = new(store.Offset<Header>(true));
    }

    public override long Extent => directions.Extent + Header.SizeOf;
    public Size Size => header.Size;
    public int Width => Size.Width;
    public int Height => Size.Height;

    public long Count => directions.Count + 1; // Fencepost

    public MazePoint this[long index] => Get(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MazePoint Get(long index)
    {
        if(header.Start == MazePoint.Empty) ExceptionExtensions.ThrowOutOfRangeException(index);
        if(index == 0) return header.Start;
        if(index == Count - 1) return header.End;

        MazePoint value;

        if(index > (Count / 2))
        {
            value = header.End;
            for(var i = directions.Count - 1; i >= index; i--) value = value.PrevDirection(directions[i]);
            return value;
        }

        value = header.Start;
        for(var i = 0; i < index; i++) value = value.NextDirection(directions[i]);
        return value;
    }

    public void Add(MazePoint point)
    {
        if(header.Start == MazePoint.Empty)
        {
            header.Start = point;
            header.End = point;
            return;
        }

        directions.Add(MazePoint.CalcDirection(header.End, point));
        header.End = point;
    }

    public void Clear()
    {
        directions.Clear();
        header.Start = MazePoint.Empty;
        header.End = MazePoint.Empty;
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
        if(header.Start == MazePoint.Empty) yield break;

        var point = header.Start;
        foreach(var direction in directions)
        {
            yield return point;
            point = point.NextDirection(direction);
        }
        if(directions.Count > 0) yield return point;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long IndexOf(MazePoint point)
    {
        var index = 0;
        foreach(var item in this)
        {
            if(item == point) return index;
            index++;
        }

        return -1;
    }

    public MazePoint Pop()
    {
        if(header.Start == MazePoint.Empty) ExceptionExtensions.ThrowOutOfRangeException(0);
        var end = header.End;

        if(directions.Count > 0)
        {
            header.End = header.End.PrevDirection(directions.Pop());
            return end;
        }

        header.Start = MazePoint.Empty;
        header.End = MazePoint.Empty;
        return end;
    }

    public void Push(MazePoint point) => Add(point);

    public MazePoint Peek() => header.End;

    public override void Read()
    {
        header = store.Read<Header>(0);
        directions = new(store.Offset<Header>(true));
        directions.Read();
    }

    public override void Write()
    {
        store.Write(0, header);
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

    private record struct Header(Size Size, MazePoint Start, MazePoint End)
    {
        public static readonly int SizeOf = IStore.SizeOf<Header>();
    }
}
