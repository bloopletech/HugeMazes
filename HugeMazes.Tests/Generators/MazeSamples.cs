using System;
using System.IO;
using System.Threading.Tasks;
using HugeMazes.Generators;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Solvers;
using HugeMazes.Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HugeMazes.Tests.Generators;

[TestClass]
public class MazeSamples
{
    [TestMethod]
    public void GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        using var maze = new BitGridMaze(IStore.CreateMemory(), new MazeSize(129, 129));

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
        var solver = ISolver.Create(SolverType.Backtrack, maze, path);
        solver.Solve();

        using var image = Renderer.Render(IStore.CreateMemory(), maze, path, RenderPalette.Default);
        image.Write();
    }
}
