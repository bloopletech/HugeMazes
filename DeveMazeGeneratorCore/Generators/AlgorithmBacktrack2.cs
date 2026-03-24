using DeveMazeGeneratorCore.Factories;
using DeveMazeGeneratorCore.Generators.Helpers;
using DeveMazeGeneratorCore.InnerMaps;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;
using System.Collections.Generic;

namespace DeveMazeGeneratorCore.Generators;

public class AlgorithmBacktrack2 : IAlgorithm
{
    public Maze GoGenerate<M>(int width, int height, int seed, IInnerMapFactory<M> mapFactory, IRandomFactory randomFactory)
      where M : InnerMap
    {
        var innerMap = mapFactory.Create(width, height);
        var random = randomFactory.Create(seed);

        return GoGenerateInternal(innerMap, random);
    }

    private static Maze GoGenerateInternal<M>(M map, IRandom random) where M : InnerMap
    {
        long totSteps = (map.Width - 1L) / 2L * ((map.Height - 1L) / 2L);
        long currentStep = 1;

        int width = map.Width - 1;
        int height = map.Height - 1;

        var stackje = new Stack<MazePoint>();
        stackje.Push(new MazePoint(1, 1));
        map[1, 1] = true;

        MazePoint[] targets = new MazePoint[4];
        //Span<MazePoint> targets = stackalloc MazePoint[4];

        while (stackje.Count != 0)
        {
            currentStep++;
            MazePoint cur = stackje.Peek();

            int targetCount = 0;
            if (cur.X - 2 > 0 && !map[cur.X - 2, cur.Y])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = cur.X - 2;
                curTarget.Y = cur.Y;
                targetCount++;
            }
            if (cur.X + 2 < width && !map[cur.X + 2, cur.Y])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = cur.X + 2;
                curTarget.Y = cur.Y;
                targetCount++;
            }
            if (cur.Y - 2 > 0 && !map[cur.X, cur.Y - 2])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = cur.X;
                curTarget.Y = cur.Y - 2;
                targetCount++;
            }
            if (cur.Y + 2 < height && !map[cur.X, cur.Y + 2])
            {
                ref var curTarget = ref targets[targetCount];
                curTarget.X = cur.X;
                curTarget.Y = cur.Y + 2;
                targetCount++;
            }

            if (targetCount > 0)
            {
                var target = targets[random.Next(targetCount)];
                stackje.Push(target);
                map[target.X, target.Y] = true;

                currentStep++;

                if (target.X < cur.X)
                {
                    map[cur.X - 1, cur.Y] = true;
                }
                else if (target.X > cur.X)
                {
                    map[cur.X + 1, cur.Y] = true;
                }
                else if (target.Y < cur.Y)
                {
                    map[cur.X, cur.Y - 1] = true;
                }
                else if (target.Y > cur.Y)
                {
                    map[cur.X, cur.Y + 1] = true;
                }
            }
            else
            {
                stackje.Pop();
            }
        }

        return new Maze(map);
    }
}
