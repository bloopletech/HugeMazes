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
    private Range range;

    public DirectionMazePath(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        directions = null!;
        range = null!;
    }

    public DirectionMazePath(IStore store, Size size, int stride, bool leaveOpen = false) : base(store, leaveOpen)
    {
        header = new Header(size, MazePoint.Empty, MazePoint.Empty, stride);
        directions = new(store.Offset<Header>(true));
        range = new Range(directions, header.Start, header.End, header.Stride);
    }

    public override long Extent => directions.Extent + Header.SizeOf;
    public Size Size => header.Size;
    public int Width => Size.Width;
    public int Height => Size.Height;

    public long Count => range.Count;

    public MazePoint this[long index] => range.Get(index);

    public void Add(MazePoint point) => range.Add(point);

    public void Clear() => range.Clear();

    public bool Contains(MazePoint point)
    {
        foreach(var item in this)
        {
            if(item == point) return true;
        }

        return false;
    }

    public IEnumerator<MazePoint> GetEnumerator() => range.GetEnumerator();
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

    public MazePoint Pop() => range.Pop();

    public void Push(MazePoint point) => Add(point);

    public MazePoint Peek() => range.Peek();

    public override void Read()
    {
        header = store.Read<Header>(0);
        directions = new(store.Offset<Header>(true));
        directions.Read();
        range = new Range(directions, header.Start, header.End, header.Stride);
    }

    public override void Write()
    {
        header.Start = range.Start;
        header.End = range.End;
        header.Stride = range.Stride;
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

    private record struct Header(Size Size, MazePoint Start, MazePoint End, int Stride)
    {
        public static readonly int SizeOf = IStore.SizeOf<Header>();
    }

    private class Range(LongList<MazeDirection> directions, MazePoint start, MazePoint end, int stride)
    {
        private bool hasStart = start == MazePoint.Empty;

        public long Count => hasStart ? directions.Count + 1 : 0; // Fencepost
        public MazePoint Start => start;
        public MazePoint End => end;
        public int Stride => Stride;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MazePoint Get(long index)
        {
            if(index < 0 || index >= Count) ExceptionExtensions.ThrowOutOfRangeException(index);
            if(index == 0) return start;
            if(index == Count - 1) return end;

            MazePoint value;

            if(index > (Count / 2))
            {
                value = end;
                for(var i = directions.Count - 1; i >= index; i--) value = value.PrevDirection(directions[i], stride);
                return value;
            }

            value = start;
            for(var i = 0; i < index; i++) value = value.NextDirection(directions[i], stride);
            return value;
        }

        public void Add(MazePoint point)
        {
            if(!hasStart)
            {
                end = start = point;
                hasStart = true;
                return;
            }

            directions.Add(MazePoint.CalcDirection(end, point));
            end = point;
        }

        public void Clear()
        {
            directions.Clear();
            //end = start = MazePoint.Empty;
            hasStart = false;
        }

        public IEnumerator<MazePoint> GetEnumerator()
        {
            var point = start;
            if(hasStart) yield return point;
            foreach(var direction in directions)
            {
                point = point.NextDirection(direction, stride);
                yield return point;
            }
        }

        public MazePoint Pop()
        {
            if(!hasStart) ExceptionExtensions.ThrowOutOfRangeException(0);
            var oldEnd = end;

            if(directions.Count > 0)
            {
                end = end.PrevDirection(directions.Pop(), stride);
                return oldEnd;
            }

            //end = start = MazePoint.Empty;
            hasStart = false;
            return oldEnd;
        }

        public MazePoint Peek()
        {
            if(!hasStart) ExceptionExtensions.ThrowOutOfRangeException(0);
            return end;
        }
    }
}
