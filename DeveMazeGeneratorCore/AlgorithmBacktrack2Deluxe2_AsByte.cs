using DeveMazeGeneratorCore.Structures;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore;

public class AlgorithmBacktrack2Deluxe2_AsByte(Maze map, Random random)
{
    public void Generate()
    {
        int width = map.Width - 1;
        int height = map.Height - 1;

        var stackje = new Stack<ImmutableMazePoint>();
        stackje.Push(new ImmutableMazePoint(1, 1));
        map[1, 1] = true;

        while (stackje.Count != 0)
        {
            var cur = stackje.Peek();

            bool validLeft = cur.X - 2 > 0 && !map[cur.X - 2, cur.Y];
            bool validRight = cur.X + 2 < width && !map[cur.X + 2, cur.Y];
            bool validUp = cur.Y - 2 > 0 && !map[cur.X, cur.Y - 2];
            bool validDown = cur.Y + 2 < height && !map[cur.X, cur.Y + 2];

            int validLeftByte = Unsafe.As<bool, byte>(ref validLeft);
            int validRightByte = Unsafe.As<bool, byte>(ref validRight);
            int validUpByte = Unsafe.As<bool, byte>(ref validUp);
            int validDownByte = Unsafe.As<bool, byte>(ref validDown);

            int targetCount = validLeftByte + validRightByte + validUpByte + validDownByte;

            if (targetCount == 0)
            {
                stackje.Pop();
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

                stackje.Push(new ImmutableMazePoint(nextX, nextY));
                map[nextXInBetween, nextYInBetween] = true;
                map[nextX, nextY] = true;
            }
        }
    }
}
