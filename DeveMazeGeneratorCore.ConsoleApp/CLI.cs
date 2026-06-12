using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;

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
        "render" => RenderTask(),
        "render-path" => RenderPathTask(),
        "benchmark" => BenchmarkTask(),
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
            maze = DeveMazeGeneratorCore.Generate(new StreamStore(mazeFileName), width, height, seed);
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
            Console.WriteLine($"Saved solution to {pathFileName}");
        };
    }

    private Action RenderTask()
    {
        mazeFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".png"));
        return () =>
        {
            maze ??= Load(mazeFileName);
            using var image = Renderer.Render(maze, RenderColors.Default);
            Renderer.Save(imageFileName, image);

            Console.WriteLine($"Saved maze image to {imageFileName}");
        };
    }

    private Action RenderPathTask()
    {
        mazeFileName ??= options.Next();
        pathFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(pathFileName, ".path.png"));
        return () =>
        {
            maze ??= Load(mazeFileName);
            path ??= LoadPath(pathFileName);
            using var image = Renderer.CreateImage(maze, path, RenderColors.Default);
            Renderer.Save(imageFileName, image);

            Console.WriteLine($"Saved maze with solution image to {imageFileName}");
        };
    }

    private static Action BenchmarkTask() => () =>
    {
        var maze = DeveMazeGeneratorCore.BenchmarkBaseline();
        var result = Verifier.IsPerfectMaze(maze);
        if(!result) throw new InvalidOperationException("Maze is not perfect");
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
