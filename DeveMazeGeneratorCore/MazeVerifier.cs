using DeveMazeGeneratorCore.Helpers;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore;

public class MazeVerifier
{
    public static bool IsPerfectMaze(Maze map)
    {
        var copiedInnerMap = map.Clone();

        FloodFill(copiedInnerMap);

        //Make uneven because a maze actually is this size and not an even number
        var unevenHeight = MathHelper.MakeUneven(copiedInnerMap.Height);
        var unevenWidth = MathHelper.MakeUneven(copiedInnerMap.Width);

        for (int y = 0; y < unevenHeight; y++)
        {
            for (int x = 0; x < unevenWidth; x++)
            {
                if (copiedInnerMap[x, y] == false)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static void FloodFill(Maze map)
    {
        var stackje = new Stack<MazePoint>();
        stackje.Push(new MazePoint(0, 0));

        int x = 0;
        int y = 0;

        int width = map.Width;
        int height = map.Height;

        while (stackje.Count != 0)
        {
            var cur = stackje.Pop();
            x = cur.X;
            y = cur.Y;

            map[x, y] = true;

            if (x - 1 > 0 && !map[x - 1, y]) stackje.Push(new MazePoint(x - 1, y));
            if (x + 1 < width - 1 && !map[x + 1, y]) stackje.Push(new MazePoint(x + 1, y));
            if (y - 1 > 0 && !map[x, y - 1]) stackje.Push(new MazePoint(x, y - 1));
            if (y + 1 < height - 1 && !map[x, y + 1]) stackje.Push(new MazePoint(x, y + 1));
        }
    }
}
