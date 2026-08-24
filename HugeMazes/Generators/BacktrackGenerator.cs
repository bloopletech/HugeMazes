using System.Diagnostics;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Structures;

namespace HugeMazes.Generators;

public class BacktrackGenerator(IMaze maze, Random random) : IGenerator
{
    private BatchedSmallRandom batchedRandom = new(random);

    public void Generate()
    {
        maze.EnsureMinimumSize();
        maze.EnsureOddSize();

        var width = maze.Width - 1;
        var height = maze.Height - 1;

        using var stack = new DirectionMazePath(IStore.Create(), maze.Id, 2);
        stack.Push(new(1, 1));
        maze[1, 1] = true;

        Span<MazePoint> targets = stackalloc MazePoint[4];

        while(stack.Count != 0)
        {
            var (x, y) = stack.Peek();

            var targetCount = 0;
            if(x - 2 > 0 && !maze[x - 2, y]) targets[targetCount++].Set(x - 2, y);
            if(x + 2 < width && !maze[x + 2, y]) targets[targetCount++].Set(x + 2, y);
            if(y - 2 > 0 && !maze[x, y - 2]) targets[targetCount++].Set(x, y - 2);
            if(y + 2 < height && !maze[x, y + 2]) targets[targetCount++].Set(x, y + 2);

            if(targetCount == 0)
            {
                stack.PopIgnore();
            }
            else
            {
                var target = targets[batchedRandom.Next(targetCount)];
                stack.Push(target);

                var (tx, ty) = target;
                maze[tx, ty] = true;
                if(tx < x) maze[x - 1, y] = true;
                else if(tx > x) maze[x + 1, y] = true;
                else if(ty < y) maze[x, y - 1] = true;
                else if(ty > y) maze[x, y + 1] = true;
            }
        }
    }

    private struct BatchedSmallRandom(Random random)
    {
        private const int Length = 0x100000;
        private readonly byte[] buffer = new byte[Length];
        private int index = Length;

        public int Next(int maxValue)
        {
            Debug.Assert(maxValue <= 4);

            if(index == Length)
            {
                random.NextBytes(buffer);
                index = 0;
            }

            return (maxValue * buffer[index++]) >> 8;
        }
    }
}
