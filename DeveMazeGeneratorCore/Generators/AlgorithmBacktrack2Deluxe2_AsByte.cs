using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Generators;

public class AlgorithmBacktrack2Deluxe2_AsByte(IMaze maze, Random random) : IAlgorithm
{
    public void Generate()
    {
        maze.EnsureMinimumSize();
        maze.EnsureOddSize();

        int width = maze.Width - 1;
        int height = maze.Height - 1;

        var stack = new Stack<MazePoint>();
        stack.Push(new(1, 1));
        maze[1, 1] = true;

        while(stack.Count != 0)
        {
            var cur = stack.Peek();

            var validLeft = cur.X - 2 > 0 && !maze[cur.X - 2, cur.Y];
            var validRight = cur.X + 2 < width && !maze[cur.X + 2, cur.Y];
            var validUp = cur.Y - 2 > 0 && !maze[cur.X, cur.Y - 2];
            var validDown = cur.Y + 2 < height && !maze[cur.X, cur.Y + 2];

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
                var chosenDirection = random.Next(targetCount);
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
                maze[nextXInBetween, nextYInBetween] = true;
                maze[nextX, nextY] = true;
            }
        }
    }
}
