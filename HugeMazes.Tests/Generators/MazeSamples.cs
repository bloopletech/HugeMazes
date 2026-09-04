using System;
using HugeMazes.Generators;
using HugeMazes.Images;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using HugeMazes.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HugeMazes.Tests.Generators;

[TestClass]
public class MazeSamples
{
    [TestMethod]
    public void GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        using var maze = new Maze(IStore.Create(), Guid.NewGuid(), new MazeSize(129, 129));

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

        using var path = new MazePath(IStore.Create());
        var solver = ISolver.Create(SolverType.Backtrack, maze, path);
        solver.Solve();

        using var image = Renderer.Render(IStore.Create(), maze, path, RenderPalette.Default);
        image.Write();
    }
}
