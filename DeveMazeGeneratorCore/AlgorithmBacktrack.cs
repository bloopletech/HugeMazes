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

        var stack = new Stack<MazePoint>();
        stack.Push(new(x, y));
        map[x, y] = true;

        var targets = new MazePoint[4];

        while (stack.Count != 0)
        {
            var cur = stack.Peek();
            x = cur.X;
            y = cur.Y;

            int targetCount = 0;
            if (x - 2 > 0 && !map[x - 2, y])
            {
                targets[targetCount] = new(x - 2, y);
                targetCount++;
            }
            if (x + 2 < width && !map[x + 2, y])
            {
                targets[targetCount] = new(x + 2, y);
                targetCount++;
            }
            if (y - 2 > 0 && !map[x, y - 2])
            {
                targets[targetCount] = new(x, y - 2);
                targetCount++;
            }
            if (y + 2 < height && !map[x, y + 2])
            {
                targets[targetCount] = new(x, y + 2);
                targetCount++;
            }

            if (targetCount > 0)
            {
                var target = targets[random.Next(targetCount)];
                stack.Push(target);
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
                stack.Pop();
            }
        }
    }
}
