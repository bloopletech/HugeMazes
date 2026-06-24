using System;
using System.IO;
using System.Threading.Tasks;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Solvers;
using DeveMazeGeneratorCore.Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeveMazeGeneratorCore.Tests.Generators;

[TestClass]
public class MazeSamples
{
    [TestMethod]
    public void GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        using var maze = new BitGridMaze(IStore.CreateMemory(), new Size(129, 129));

        for(int y = 33; y < 96; y++)
        {
            for(int x = 33; x < 96; x++)
            {
                maze[x, y] = true;
            }
        }

        var random = new Random(1337);

        var generator = new BacktrackGenerator(maze, random);
        generator.Generate();

        using var path = new MazePath(IStore.CreateMemory());
        var solver = ISolver.Create(SolverType.DepthFirstSmart, maze, path);
        solver.Solve();

        using var image = Renderer.Render(maze, path, IStore.CreateMemory(), RenderColours.Default);
        image.Write();
    }
}
