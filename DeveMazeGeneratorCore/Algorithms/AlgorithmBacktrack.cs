using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Algorithms;

public class AlgorithmBacktrack(Maze maze, Random random) : Algorithm(maze, random)
{
    public override void Generate()
    {
        var width = Maze.Width - 1;
        var height = Maze.Height - 1;

        var capacityEstimate = Convert.ToInt32(Math.Ceiling(width * height * 0.05));

        var stack = new Stack<MazePoint>(capacityEstimate);
        stack.Push(new(1, 1));
        Maze[1, 1] = true;

        Span<MazePoint> targets = stackalloc MazePoint[4];
        targets.Clear();

        while(stack.Count != 0)
        {
            var cur = stack.Peek();
            var x = cur.X;
            var y = cur.Y;

            var targetCount = 0;
            if(x - 2 > 0 && !Maze[x - 2, y])
            {
                targets[targetCount].Set(x - 2, y);
                targetCount++;
            }
            if(x + 2 < width && !Maze[x + 2, y])
            {
                targets[targetCount].Set(x + 2, y);
                targetCount++;
            }
            if(y - 2 > 0 && !Maze[x, y - 2])
            {
                targets[targetCount].Set(x, y - 2);
                targetCount++;
            }
            if(y + 2 < height && !Maze[x, y + 2])
            {
                targets[targetCount].Set(x, y + 2);
                targetCount++;
            }

            if(targetCount > 0)
            {
                var target = targets[Random.Next(targetCount)];
                stack.Push(target);
                Maze[target.X, target.Y] = true;

                if(target.X < x)
                {
                    Maze[x - 1, y] = true;
                }
                else if(target.X > x)
                {
                    Maze[x + 1, y] = true;
                }
                else if(target.Y < y)
                {
                    Maze[x, y - 1] = true;
                }
                else if(target.Y > y)
                {
                    Maze[x, y + 1] = true;
                }
            }
            else
            {
                stack.Pop();
            }
        }
    }
}
