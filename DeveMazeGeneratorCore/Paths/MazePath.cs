using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Collections;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class MazePath : Storable, IMazePath
{
    private LongList<MazePoint> points;
    private Size size;

    public MazePath(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        points = null!;
    }

    public MazePath(IStore store, Size size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        points = new(store.Offset<Size>(true));
    }

    public override long Extent => points.Extent + Size.SizeOf;
    public Size Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

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
    public void Push(MazePoint point) => points.Push(point);
    public MazePoint Shift() => points.Shift();
    public void Unshift(MazePoint point) => points.Unshift(point);
    public MazePoint Peek() => points.Peek();

    public override void Read()
    {
        size = store.Read<Size>(0);
        points = new(store.Offset<Size>(true));
        points.Read();
    }

    public override void Write()
    {
        store.Write(0, size);
        points.Write();
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
}
