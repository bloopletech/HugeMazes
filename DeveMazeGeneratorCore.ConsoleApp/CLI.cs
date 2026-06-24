using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Solvers;

namespace DeveMazeGeneratorCore.ConsoleApp;

public class CLI(Options options)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211")]
    public static bool SkipReuse = false;

    private string? mazeFileName;
    private IMaze? maze;
    private string? pathFileName;
    private IMazePath? path;

    public void Run()
    {
        try
        {
            var tasks = new List<Action>();
            while(options.HasNext()) tasks.Add(CreateTask(options.Next()));
            foreach(var task in tasks)
            {
                task();

                if(SkipReuse)
                {
                    maze?.Dispose();
                    maze = null;
                    path?.Dispose();
                    path = null;
                }
            }
        }
        finally
        {
            maze?.Dispose();
            path?.Dispose();
        }
    }

    private Action CreateTask(string task) => task switch
    {
        "generate" => GenerateTask(),
        "verify" => VerifyTask(),
        "solve" => SolveTask(),
        "verify-path" => VerifyPathTask(),
        "render" => RenderTask(),
        "render-path" => RenderPathTask(),
        "benchmark" => BenchmarkTask(),
        "benchmark-direction-maze-path" => BenchmarkDirectionMazePathTask(),
        _ => throw new InvalidOperationException($"Unknown task: {task}"),
    };

    private Action GenerateTask()
    {
        var width = options.NextInt().MakeUneven();
        var height = options.NextInt().MakeUneven();
        int? seed = options.HasNextInt() ? options.NextInt() : null;
        mazeFileName = options.NextFileName($"{Environment.TickCount}.maze");

        return () =>
        {
            maze = DeveMazeGeneratorCore.Generate(width, height, seed, new StreamStore(mazeFileName));
            maze.Write();
            Console.WriteLine($"Saved maze to {mazeFileName}");
        };
    }

    private Action VerifyTask()
    {
        mazeFileName ??= options.Next();
        return () =>
        {
            maze ??= Load(mazeFileName);
            var result = Verifier.IsPerfectMaze(maze);
            Console.WriteLine($"Is our maze perfect?: {result}");
        };
    }

    private Action SolveTask()
    {
        mazeFileName ??= options.Next();
        pathFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".path"));
        return () =>
        {
            maze ??= Load(mazeFileName);
            path = DeveMazeGeneratorCore.Solve(maze, new StreamStore(pathFileName));
            path.Write();
            Console.WriteLine($"Saved path to {pathFileName}");
        };
    }

    private Action VerifyPathTask()
    {
        pathFileName ??= options.Next();
        return () =>
        {
            path ??= LoadPath(pathFileName);
            var result = Verifier.IsPerfectPath(path);
            Console.WriteLine($"Is our path perfect?: {result}");
        };
    }

    private Action RenderTask()
    {
        mazeFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".ppm"));
        return () =>
        {
            maze ??= Load(mazeFileName);
            using var image = DeveMazeGeneratorCore.Render(maze, new StreamStore(imageFileName));
            image.Write();
            Console.WriteLine($"Saved maze image to {imageFileName}");
        };
    }

    private Action RenderPathTask()
    {
        mazeFileName ??= options.Next();
        pathFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(pathFileName, ".path.ppm"));
        return () =>
        {
            maze ??= Load(mazeFileName);
            path ??= LoadPath(pathFileName);
            using var image = DeveMazeGeneratorCore.Render(maze, path, new StreamStore(imageFileName));
            image.Write();
            Console.WriteLine($"Saved maze with path image to {imageFileName}");
        };
    }

    private static Action BenchmarkTask() => () =>
    {
        var maze = DeveMazeGeneratorCore.BenchmarkBaseline();
        var result = Verifier.IsPerfectMaze(maze);
        if(!result) throw new InvalidOperationException("Maze is not perfect");
    };

    private static Action BenchmarkDirectionMazePathTask() => () =>
    {
        var maze = DeveMazeGeneratorCore.BenchmarkBaseline();
        var path = DeveMazeGeneratorCore.Solve(maze, null);
        var result = Verifier.IsPerfectPath(path);
        if(!result) throw new InvalidOperationException("Path is not perfect");
    };

    private static IMaze Load(string fileName)
    {
        return MazeSerializer.Read(new StreamStore(fileName));
    }

    private static IMazePath LoadPath(string fileName)
    {
        return MazePathSerializer.Read(new StreamStore(fileName));
    }
}
