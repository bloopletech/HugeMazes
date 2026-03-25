using DeveMazeGeneratorCore.Imageification;
using DeveMazeGeneratorCore.InnerMaps;
using System;
using System.IO;
using Xunit;

namespace DeveMazeGeneratorCore.Tests.Generators;

public class MazeSamples
{
    [Fact]
    public void GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        var map = new BitArreintjeFastInnerMap(128, 128);

        for (int y = 33; y < 96; y++)
        {
            for (int x = 33; x < 96; x++)
            {
                map[x, y] = true;
            }
        }

        var random = new Random(1337);

        var algorithm = new AlgorithmBacktrack(map, random);
        algorithm.Generate();

        var path = PathFinder.GoFind(map, null);

        using (var fs = new FileStream("GeneratingAMazeWithABlockInTheMiddleWorks.png", FileMode.Create))
        {
            WithPath.SaveMazeAsImageDeluxePng(map, path, fs);
        }
    }
}
