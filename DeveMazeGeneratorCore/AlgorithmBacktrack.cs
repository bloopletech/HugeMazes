using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public class AlgorithmBacktrack(Maze maze, Random random)
{
    public void Generate()
    {
        int width = maze.Width - 1;
        int height = maze.Height - 1;
        int x = 1;
        int y = 1;

        var stack = new Stack<MazePoint>();
        stack.Push(new(x, y));
        maze[x, y] = true;

        var targets = new MazePoint[4];

        while (stack.Count != 0)
        {
            var cur = stack.Peek();
            x = cur.X;
            y = cur.Y;

            int targetCount = 0;
            if (x - 2 > 0 && !maze[x - 2, y])
            {
                targets[targetCount] = new(x - 2, y);
                targetCount++;
            }
            if (x + 2 < width && !maze[x + 2, y])
            {
                targets[targetCount] = new(x + 2, y);
                targetCount++;
            }
            if (y - 2 > 0 && !maze[x, y - 2])
            {
                targets[targetCount] = new(x, y - 2);
                targetCount++;
            }
            if (y + 2 < height && !maze[x, y + 2])
            {
                targets[targetCount] = new(x, y + 2);
                targetCount++;
            }

            if (targetCount > 0)
            {
                var target = targets[random.Next(targetCount)];
                stack.Push(target);
                maze[target.X, target.Y] = true;

                if (target.X < x)
                {
                    maze[x - 1, y] = true;
                }
                else if (target.X > x)
                {
                    maze[x + 1, y] = true;
                }
                else if (target.Y < y)
                {
                    maze[x, y - 1] = true;
                }
                else if (target.Y > y)
                {
                    maze[x, y + 1] = true;
                }
            }
            else
            {
                stack.Pop();
            }
        }
    }
}
