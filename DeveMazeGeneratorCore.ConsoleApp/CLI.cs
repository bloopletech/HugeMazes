using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.ConsoleApp;

public class CLI(Options options)
{
    private string? mazeFileName;
    private IMaze? maze;
    private string? pathFileName;
    private IMazePath? path;

    public async Task Run()
    {
        var tasks = new List<Func<Task>>();
        while(options.HasNext()) tasks.Add(CreateTask(options.Next()));
        foreach(var task in tasks) await task();
    }

    private Func<Task> CreateTask(string task) => task switch
    {
        "generate" => GenerateTask(),
        "verify" => VerifyTask(),
        "solve" => SolveTask(),
        "render" => RenderTask(),
        "render-path" => RenderPathTask(),
        "benchmark" => BenchmarkTask(),
        _ => throw new InvalidOperationException($"Unknown task: {task}"),
    };

    private Func<Task> GenerateTask()
    {
        var width = options.NextInt().MakeUneven();
        var height = options.NextInt().MakeUneven();
        int? seed = options.HasNextInt() ? options.NextInt() : null;
        mazeFileName = options.NextFileName($"{Environment.TickCount}.maze");

        return async () =>
        {
            using var fs = File.Open(mazeFileName, FileMode.Create);
            using var bs = new BinarySerializer(fs);
            maze = DeveMazeGeneratorCore.Generate(bs, width, height, seed);
            maze.Write();
            Console.WriteLine($"Saved maze to {mazeFileName}");
        };
    }

    private Func<Task> VerifyTask()
    {
        mazeFileName ??= options.Next();
        return async () =>
        {
            maze ??= await LoadAsync(mazeFileName);
            var result = Verifier.IsPerfectMaze(maze);
            Console.WriteLine($"Is our maze perfect?: {result}");
        };
    }

    private Func<Task> SolveTask()
    {
        mazeFileName ??= options.Next();
        pathFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".path"));
        return async () =>
        {
            maze ??= await LoadAsync(mazeFileName);
            using var fs = File.Open(pathFileName, FileMode.Create);
            using var bs = new BinarySerializer(fs);
            path = DeveMazeGeneratorCore.Solve(maze, MazePathType.MazePath, bs);
            path.Write();
            Console.WriteLine($"Saved solution to {pathFileName}");
        };
    }

    private Func<Task> RenderTask()
    {
        mazeFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".png"));
        return async () =>
        {
            maze ??= await LoadAsync(mazeFileName);
            using var image = Renderer.Render(maze, RenderColors.Default);
            await Renderer.Save(imageFileName, image);

            Console.WriteLine($"Saved maze image to {imageFileName}");
        };
    }

    private Func<Task> RenderPathTask()
    {
        mazeFileName ??= options.Next();
        pathFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(pathFileName, ".path.png"));
        return async () =>
        {
            maze ??= await LoadAsync(mazeFileName);
            path ??= await LoadPathAsync(pathFileName);
            using var image = Renderer.CreateImage(maze, path, RenderColors.Default);
            await Renderer.Save(imageFileName, image);

            Console.WriteLine($"Saved maze with solution image to {imageFileName}");
        };
    }

    private Func<Task> BenchmarkTask() => async () =>
    {
        var maze = DeveMazeGeneratorCore.BenchmarkBaseline();
        var result = Verifier.IsPerfectMaze(maze);
        if(!result) throw new InvalidOperationException("Maze is not perfect");
    };

    private static async Task<IMaze> LoadAsync(string fileName)
    {
        var fs = File.Open(fileName, FileMode.Open);
        var bs = new BinarySerializer(fs);
        return await MazeSerializer.ReadAsync(bs);
    }

    private static async Task<IMazePath> LoadPathAsync(string fileName)
    {
        var fs = File.Open(fileName, FileMode.Open);
        var bs = new BinarySerializer(fs);
        return await MazePathSerializer.ReadAsync(bs);
    }
}
