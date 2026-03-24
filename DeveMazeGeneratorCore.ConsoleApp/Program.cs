using DeveCoolLib.DeveConsoleMenu;
using DeveMazeGeneratorCore.Factories;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Generators.Helpers;
using DeveMazeGeneratorCore.Generators.SpeedOptimization;
using DeveMazeGeneratorCore.Helpers;
using DeveMazeGeneratorCore.Imageification;
using DeveMazeGeneratorCore.InnerMaps;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.PathFinders;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DeveMazeGeneratorCore.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"DeveMazeGeneratorCore version: {Assembly.GetEntryAssembly().GetName().Version}");

        var width = int.Parse(args[0], System.Globalization.NumberStyles.None);
        var height = int.Parse(args[1], System.Globalization.NumberStyles.None);
        var seed = args.Length > 2 ? int.Parse(args[2], System.Globalization.NumberStyles.None) : Environment.TickCount;

        Generate(width, height, seed);
    }

    public static void Generate(int width, int height, int seed)
    {
        var alg = new AlgorithmBacktrack2Deluxe2_AsByte();

        var innerMapFactory = new InnerMapFactory<BitArreintjeFastInnerMap>();
        var randomFactory = new RandomFactory<XorShiftRandom>();

        var actionThing = new NoAction();

        var maze = alg.GoGenerate(width, height, seed, innerMapFactory, randomFactory, actionThing);

        using (var fs = new FileStream($"GeneratedMazeNoPath{alg.GetType().Name}.png", FileMode.Create))
        {
            WithPath.SaveMazeAsImageDeluxePng(maze.InnerMap, new System.Collections.Generic.List<Structures.MazePointPos>(), fs);
        }

        Console.WriteLine("Finding path");

        var path = PathFinderDepthFirstSmartWithPos.GoFind(maze.InnerMap, null);
        Console.WriteLine("Found path :)");

        using (var fs = new FileStream($"GeneratedMaze{alg.GetType().Name}.png", FileMode.Create))
        {
            WithPath.SaveMazeAsImageDeluxePng(maze.InnerMap, path, fs);
        }

        var result = MazeVerifier.IsPerfectMaze(maze.InnerMap);
        Console.WriteLine($"Is our maze perfect?: {result}");
    }

    public static void ActualBenchmark2(int width, int height, int seed)
    {
        var fastestElapsed = TimeSpan.MaxValue;

        Console.WriteLine($"Generating mazes using AlgorithmBacktrack2Deluxe2_AsByte...");

        while (true)
        {
            var w = Stopwatch.StartNew();
            Generate(width, height, seed);
            w.Stop();

            bool foundFastest = false;
            if (w.Elapsed < fastestElapsed)
            {
                foundFastest = true;
                fastestElapsed = w.Elapsed;
            }

            var strToPrint = $"Generation time: {w.Elapsed}" + (foundFastest ? " <<<<<<<< new fastest time" : "");
            var strToPrint2 = $"{strToPrint.PadRight(68, ' ')} Fastest: {fastestElapsed}";

            Console.WriteLine(strToPrint2);
            seed++;
        }
    }
}
