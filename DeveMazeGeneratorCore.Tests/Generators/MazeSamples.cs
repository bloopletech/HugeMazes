using System;
using System.IO;
using System.Threading.Tasks;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeveMazeGeneratorCore.Tests.Generators;

[TestClass]
public class MazeSamples
{
    [TestMethod]
    public async Task GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        using var store = new StreamStore(new MemoryStream());
        var maze = new BitGridMaze(store, 129, 129);

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

        var path = new MazePath(IStore.CreateMemory());
        PathFinder.Find(maze, path);

        var image = Renderer.CreateImage(maze, path, RenderColors.Default);

        using var fs = new FileStream("GeneratingAMazeWithABlockInTheMiddleWorks.png", FileMode.Create);
        await Renderer.Serialize(fs, image);
    }
}
