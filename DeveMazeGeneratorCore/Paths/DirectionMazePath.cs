using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class DirectionMazePath : Storable, IMazePath
{
    private LongList<MazeDirection> directions;
    private Adapter adapter;

    public DirectionMazePath(IStore store, int delta = 1, bool leaveOpen = false) : base(store, leaveOpen)
    {
        directions = new(store.Offset<Header>(true));
        adapter = new Adapter(directions, MazePoint.Empty, MazePoint.Empty, delta);
    }

    public override long Extent => directions.Extent + Header.SizeOf;

    public long Count => adapter.Count;
    public MazePoint this[long index] => adapter.Get(index);
    public void Add(MazePoint point) => adapter.Add(point);
    public void Clear() => adapter.Clear();
    public bool Contains(MazePoint point) => adapter.Contains(point);
    public IEnumerator<MazePoint> GetEnumerator() => adapter.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public long IndexOf(MazePoint point) => adapter.IndexOf(point);
    public MazePoint Pop() => adapter.Pop();
    public void Push(MazePoint point) => Add(point);
    public MazePoint Peek() => adapter.Peek();

    public override void Read()
    {
        directions = new(store.Offset<Header>(true));
        directions.Read();

        var (start, end, delta) = store.Read<Header>(0);
        adapter = new Adapter(directions, start, end, delta);
    }

    public override void Write()
    {
        store.Write(0, new Header(adapter.Start, adapter.End, adapter.Delta));
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

    private class Adapter(LongList<MazeDirection> directions, MazePoint start, MazePoint end, int delta)
    {
        private bool HasStart => start != MazePoint.Empty;

        public long Count => HasStart ? directions.Count + 1 : 0; // Fencepost
        public MazePoint Start => start;
        public MazePoint End => end;
        public int Delta => delta;

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
                for(var i = directions.Count - 1; i >= index; i--) value = value.PrevDirection(directions[i], delta);
                return value;
            }

            value = start;
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
            if(!HasStart) ExceptionExtensions.ThrowOutOfRangeException(0);
            var prevEnd = end;
            if(directions.Count > 0) end = end.PrevDirection(directions.Pop(), delta);
            else end = start = MazePoint.Empty;
            return prevEnd;
        }

        public MazePoint Peek()
        {
            if(!HasStart) ExceptionExtensions.ThrowOutOfRangeException(0);
            return end;
        }
    }
}
