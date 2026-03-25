using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public class AlgorithmBacktrack(Maze map, Random random)
{
    public void Generate()
    {
        int width = map.Width - 1;
        int height = map.Height - 1;
        int x = 1;
        int y = 1;

        var stackje = new Stack<MazePoint>();
        stackje.Push(new MazePoint(x, y));
        map[x, y] = true;

        var targets = new MazePoint[4];

        while (stackje.Count != 0)
        {
            var cur = stackje.Peek();
            x = cur.X;
            y = cur.Y;

            int targetCount = 0;
            if (x - 2 > 0 && !map[x - 2, y])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = x - 2;
                curTarget.Y = y;
                targetCount++;
            }
            if (x + 2 < width && !map[x + 2, y])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = x + 2;
                curTarget.Y = y;
                targetCount++;
            }
            if (y - 2 > 0 && !map[x, y - 2])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = x;
                curTarget.Y = y - 2;
                targetCount++;
            }
            if (y + 2 < height && !map[x, y + 2])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = x;
                curTarget.Y = y + 2;
                targetCount++;
            }

            if (targetCount > 0)
            {
                var target = targets[random.Next(targetCount)];
                stackje.Push(target);
                map[target.X, target.Y] = true;

                if (target.X < x)
                {
                    map[x - 1, y] = true;
                }
                else if (target.X > x)
                {
                    map[x + 1, y] = true;
                }
                else if (target.Y < y)
                {
                    map[x, y - 1] = true;
                }
                else if (target.Y > y)
                {
                    map[x, y + 1] = true;
                }
            }
            else
            {
                stackje.Pop();
            }
        }
    }
}
