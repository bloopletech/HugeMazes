using System.Collections;
using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Mazes;

namespace HugeMazes.Paths;

public class MazePath(IStore store, Guid mazeId, bool leaveOpen = false) : Storable(store, leaveOpen), IMazePath
{
    private LongList<MazePoint> points = new(store.Offset<Guid>(true), true);

    public MazePath(IStore store, bool leaveOpen = false) : this(store, default, leaveOpen)
    {
    }

    public override long Extent => points.Extent + MazeSize.SizeOf;
    public Guid MazeId => mazeId;

    public long Count => points.Count;

    public MazePoint this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => points[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => points[index] = value;
    }

    public void Add(MazePoint point) => points.Add(point);
    public void Clear() => points.Clear();
    public bool Contains(MazePoint point) => points.Contains(point);
    public IEnumerator<MazePoint> GetEnumerator() => points.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public long IndexOf(MazePoint point) => points.IndexOf(point);
    public void Insert(long index, MazePoint point) => points.Insert(index, point);
    public bool Remove(MazePoint point) => points.Remove(point);
    public void RemoveAt(long index) => points.RemoveAt(index);
    public MazePoint Pop() => points.Pop();
    public void PopIgnore() => points.PopIgnore();
    public void Push(MazePoint point) => points.Push(point);
    public MazePoint Shift() => points.Shift();
    public void Unshift(MazePoint point) => points.Unshift(point);
    public MazePoint Peek() => points.Peek();

    public override void Read()
    {
        mazeId = store.Read<Guid>(0);
        points = new(store.Offset<Guid>(true), true);
        points.Read();
    }

    public override void Write()
    {
        store.Write(0, mazeId);
        points.Write();
    }

    public IMazePath Clone() => Clone(IStore.Create());

    public IMazePath Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new MazePath(destination, leaveOpen);
        result.Read();
        return result;
    }
}
