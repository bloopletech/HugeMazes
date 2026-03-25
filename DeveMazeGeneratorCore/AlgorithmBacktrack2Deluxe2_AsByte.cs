using DeveMazeGeneratorCore.Structures;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore;

public class AlgorithmBacktrack2Deluxe2_AsByte(Maze maze, Random random)
{
    public void Generate()
    {
        int width = maze.Width - 1;
        int height = maze.Height - 1;

        var stack = new Stack<MazePoint>();
        stack.Push(new(1, 1));
        maze[1, 1] = true;

        while (stack.Count != 0)
        {
            var cur = stack.Peek();

            bool validLeft = cur.X - 2 > 0 && !maze[cur.X - 2, cur.Y];
            bool validRight = cur.X + 2 < width && !maze[cur.X + 2, cur.Y];
            bool validUp = cur.Y - 2 > 0 && !maze[cur.X, cur.Y - 2];
            bool validDown = cur.Y + 2 < height && !maze[cur.X, cur.Y + 2];

            int validLeftByte = Unsafe.As<bool, byte>(ref validLeft);
            int validRightByte = Unsafe.As<bool, byte>(ref validRight);
            int validUpByte = Unsafe.As<bool, byte>(ref validUp);
            int validDownByte = Unsafe.As<bool, byte>(ref validDown);

            int targetCount = validLeftByte + validRightByte + validUpByte + validDownByte;

            if (targetCount == 0)
            {
                stack.Pop();
            }
            else
            {
                var chosenDirection = random.Next(targetCount);
                int countertje = 0;

                bool actuallyGoingLeft = validLeft & chosenDirection == countertje;
                byte actuallyGoingLeftByte = Unsafe.As<bool, byte>(ref actuallyGoingLeft);
                countertje += validLeftByte;

                bool actuallyGoingRight = validRight & chosenDirection == countertje;
                byte actuallyGoingRightByte = Unsafe.As<bool, byte>(ref actuallyGoingRight);
                countertje += validRightByte;

                bool actuallyGoingUp = validUp & chosenDirection == countertje;
                byte actuallyGoingUpByte = Unsafe.As<bool, byte>(ref actuallyGoingUp);
                countertje += validUpByte;

                bool actuallyGoingDown = validDown & chosenDirection == countertje;
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
