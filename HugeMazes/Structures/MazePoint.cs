using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.IO;

namespace HugeMazes.Structures;

/// <summary>
/// Contains a position.
/// Note: Struct really is faster then class
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct MazePoint(int X, int Y)
{
    public static readonly int SizeOf = IStore.SizeOf<MazePoint>();
    public static readonly MazePoint Empty = new(-1, -1);
    public static readonly MazePoint Start = new(1, 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int x, int y)
    {
        X = x;
        Y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly MazePoint NextDirection(MazeDirection direction, int delta) => direction switch
    {
        MazeDirection.North => new(X, Y - delta),
        MazeDirection.East => new(X + delta, Y),
        MazeDirection.South => new(X, Y + delta),
        MazeDirection.West => new(X - delta, Y),
        _ => throw new InvalidOperationException("Impossible case reached")
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly MazePoint PrevDirection(MazeDirection direction, int delta) => direction switch
    {
        MazeDirection.North => new(X, Y + delta),
        MazeDirection.East => new(X - delta, Y),
        MazeDirection.South => new(X, Y - delta),
        MazeDirection.West => new(X + delta, Y),
        _ => throw new InvalidOperationException("Impossible case reached")
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MazeDirection CalcDirection(MazePoint start, MazePoint end)
    {
        if(start.X == end.X && start.Y > end.Y) return MazeDirection.North;
        if(start.X < end.X && start.Y == end.Y) return MazeDirection.East;
        if(start.X == end.X && start.Y < end.Y) return MazeDirection.South;
        if(start.X > end.X && start.Y == end.Y) return MazeDirection.West;
        throw new InvalidOperationException("Impossible case reached");
    }
}
