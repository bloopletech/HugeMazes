using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;
using System;
using System.IO;
using Xunit;

namespace DeveMazeGeneratorCore.Tests.Generators;

public class MazeSamples
{
    [Fact]
    public void GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        var maze = new BitArreintjeFastInnerMap(128, 128);

        for(int y = 33; y < 96; y++)
        {
            for(int x = 33; x < 96; x++)
            {
                maze[x, y] = true;
            }
        }

        var random = new Random(1337);

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        var path = PathFinder.Find(maze);

        var image = ImageCreator.CreateImage(maze, path);

        using var fs = new FileStream("GeneratingAMazeWithABlockInTheMiddleWorks.png", FileMode.Create);
        ImageCreator.SaveImage(image, fs);
    }
}
