using DeveMazeGeneratorCore.Imageification;
using DeveMazeGeneratorCore.Mazes;
using System.Diagnostics;
using System.Reflection;

namespace DeveMazeGeneratorCore.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"DeveMazeGeneratorCore version: {Assembly.GetEntryAssembly().GetName().Version}");

        var width = int.Parse(args[0], System.Globalization.NumberStyles.None);
        var height = int.Parse(args[1], System.Globalization.NumberStyles.None);
        int? seed = args.Length > 2 ? int.Parse(args[2], System.Globalization.NumberStyles.None) : null;

        Generate(width, height, seed);
    }

    public static void Generate(int width, int height, int? seed)
    {
        var maze = new BitArreintjeFastInnerMap(width, height);
        var random = seed != null ? new Random(seed.Value) : new Random();
        var alg = new AlgorithmBacktrack2Deluxe2_AsByte(maze, random);

        alg.Generate();

        using (var fs = new FileStream($"GeneratedMazeNoPath{alg.GetType().Name}.png", FileMode.Create))
        {
            WithPath.SaveMazeAsImageDeluxePng(maze, [], fs);
        }

        Console.WriteLine("Finding path");

        var path = PathFinder.GoFind(maze);
        Console.WriteLine("Found path :)");

        using (var fs = new FileStream($"GeneratedMaze{alg.GetType().Name}.png", FileMode.Create))
        {
            WithPath.SaveMazeAsImageDeluxePng(maze, path, fs);
        }

        var result = MazeVerifier.IsPerfectMaze(maze);
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
