using DeveMazeGeneratorCore.Structures;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Algorithms;

public class AlgorithmBacktrack2Deluxe2_AsByte(Maze maze, Random random) : Algorithm(maze, random)
{
    public override void Generate()
    {
        int width = Maze.Width - 1;
        int height = Maze.Height - 1;

        var stack = new Stack<MazePoint>();
        stack.Push(new(1, 1));
        Maze[1, 1] = true;

        while(stack.Count != 0)
        {
            var cur = stack.Peek();

            var validLeft = cur.X - 2 > 0 && !Maze[cur.X - 2, cur.Y];
            var validRight = cur.X + 2 < width && !Maze[cur.X + 2, cur.Y];
            var validUp = cur.Y - 2 > 0 && !Maze[cur.X, cur.Y - 2];
            var validDown = cur.Y + 2 < height && !Maze[cur.X, cur.Y + 2];

            int validLeftByte = Unsafe.As<bool, byte>(ref validLeft);
            int validRightByte = Unsafe.As<bool, byte>(ref validRight);
            int validUpByte = Unsafe.As<bool, byte>(ref validUp);
            int validDownByte = Unsafe.As<bool, byte>(ref validDown);

            var targetCount = validLeftByte + validRightByte + validUpByte + validDownByte;

            if(targetCount == 0)
            {
                stack.Pop();
            }
            else
            {
                var chosenDirection = Random.Next(targetCount);
                var countertje = 0;

                var actuallyGoingLeft = validLeft & chosenDirection == countertje;
                byte actuallyGoingLeftByte = Unsafe.As<bool, byte>(ref actuallyGoingLeft);
                countertje += validLeftByte;

                var actuallyGoingRight = validRight & chosenDirection == countertje;
                byte actuallyGoingRightByte = Unsafe.As<bool, byte>(ref actuallyGoingRight);
                countertje += validRightByte;

                var actuallyGoingUp = validUp & chosenDirection == countertje;
                byte actuallyGoingUpByte = Unsafe.As<bool, byte>(ref actuallyGoingUp);
                countertje += validUpByte;

                var actuallyGoingDown = validDown & chosenDirection == countertje;
                byte actuallyGoingDownByte = Unsafe.As<bool, byte>(ref actuallyGoingDown);

                var nextX = cur.X + actuallyGoingLeftByte * -2 + actuallyGoingRightByte * 2;
                var nextY = cur.Y + actuallyGoingUpByte * -2 + actuallyGoingDownByte * 2;

                var nextXInBetween = cur.X - actuallyGoingLeftByte + actuallyGoingRightByte;
                var nextYInBetween = cur.Y - actuallyGoingUpByte + actuallyGoingDownByte;

                stack.Push(new(nextX, nextY));
                Maze[nextXInBetween, nextYInBetween] = true;
                Maze[nextX, nextY] = true;
            }
        }
    }
}
