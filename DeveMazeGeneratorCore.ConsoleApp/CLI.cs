using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Files;

namespace DeveMazeGeneratorCore.ConsoleApp;

public class CLI(Options options)
{
    private string? mazeFileName;
    private IMaze? maze;
    private string? pathFileName;
    private MazePath? path;

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
            maze = DeveMazeGeneratorCore.Generate(width, height, seed);
            await IMazeFile.SaveAsync(mazeFileName, maze);
            Console.WriteLine($"Saved maze to {mazeFileName}");
        };
    }

    private Func<Task> VerifyTask()
    {
        mazeFileName ??= options.Next();
        return async () =>
        {
            maze ??= await IMazeFile.LoadAsync(mazeFileName);
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
            maze ??= await IMazeFile.LoadAsync(mazeFileName);
            path = PathFinder.Find(maze);
            await MazePathFile.SaveAsync(pathFileName, path);
            Console.WriteLine($"Saved solution to {pathFileName}");
        };
    }

    private Func<Task> RenderTask()
    {
        mazeFileName ??= options.Next();
        var imageFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".png"));
        return async () =>
        {
            maze ??= await IMazeFile.LoadAsync(mazeFileName);
            using var image = ImageCreator.CreateImage(maze);
            await ImageCreator.Save(imageFileName, image);

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
            maze ??= await IMazeFile.LoadAsync(mazeFileName);
            path ??= await MazePathFile.LoadAsync(pathFileName);
            using var image = ImageCreator.CreateImage(maze, path);
            await ImageCreator.Save(imageFileName, image);

            Console.WriteLine($"Saved maze with solution image to {imageFileName}");
        };
    }

    private Func<Task> BenchmarkTask() => async () =>
    {
        var maze = DeveMazeGeneratorCore.BenchmarkBaseline();
        var result = Verifier.IsPerfectMaze(maze);
        if(!result) throw new InvalidOperationException("Maze is not perfect");
    };
}
