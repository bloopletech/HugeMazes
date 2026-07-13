using HugeMazes.CLI;
using HugeMazes.ConsoleApp;
using HugeMazes.Extensions;
using HugeMazes.Generators;
using HugeMazes.IO;
using HugeMazes.Mazes;
using HugeMazes.Paths;
using NeoSmart.PrettySize;
using static HugeMazes.HugeMazes;
using static HugeMazes.Verifier;
using BoundCLITask = (string Name, string Arguments, System.Action Action);
using CLITask = (object? Description, System.Action Action);

//IStore.LongOverride = true;

using var measurer = new Measurer("program");
var options = ParseOptions();
string? mazeFileName = null;
IMaze? maze = null;
string? pathFileName = null;
IMazePath? path = null;

Console.WriteLine($"Arguments: {string.Join(' ', args)}");
Console.WriteLine($"Resolves to: {options}");

var tasks = new List<BoundCLITask>();
while(options.HasNext()) tasks.Add(CreateTask(options.Next()));

Console.WriteLine("Tasks:");
foreach(var (Name, Arguments, _) in tasks) Console.WriteLine($"{Name} {Arguments}");
Console.WriteLine();

try
{
    foreach(var (Name, Arguments, Action) in tasks) Measurer.Measure(Name, Action);
}
catch(InsufficientDiskSpaceException ex)
{
    var required = ex.Size - ex.FreeSpace;
    Console.WriteLine($"""
        Drive {ex.Drive} has insufficient free space to fully store {ex.Name}.
        The drive has {PrettySize.Bytes(ex.FreeSpace)} space free but the file needs {PrettySize.Bytes(ex.Size)}.
        You must free up {PrettySize.Bytes(required)} of disk space ({required:N0} bytes) or put the file on a different drive.
        """);
}
finally
{
    maze?.Dispose();
    path?.Dispose();
}

Options ParseOptions()
{
    var options = new Options(args);
    if(!options.HasNext()) return new(["help"]);
    if(options.HasNextInt())
    {
        var width = options.Next();
        var height = options.HasNext() ? options.Next() : width;
        return new(["generate", width, height, "verify", "solve", "verify-path", "render", "render-path"]);
    }
    return options;
}

BoundCLITask CreateTask(string task)
{
    var result = task switch
    {
        "help" => HelpTask(),
        "generate" => GenerateTask(),
        "verify" => VerifyTask(),
        "solve" => SolveTask(),
        "verify-path" => VerifyPathTask(),
        "render" => RenderTask(),
        "render-path" => RenderPathTask(),
        "benchmark" => BenchmarkTask(),
        _ => throw new InvalidOperationException($"Unknown task: {task}"),
    };
    return (task, result.Description?.ToString() ?? "", result.Action);
}

static CLITask HelpTask() => (null, () => Console.Write("""
    Generates random mazes, solves those mazes, and draws those mazes and maze paths as images.

    Provide list of tasks to perform as command line arguments.
    Available tasks:

    generate <width> <height> [mazeFileName]
        Generate a random maze of given width and height and save maze to mazeFileName
        If no filename provided, defaults to <random number>.maze

    verify <mazeFileName>
        Verify that given maze is valid

    solve <mazeFileName> [pathFileName]
        Solve given maze and save maze path to pathFileName
        If no path filename provided, defaults to <mazeFileName basename>.path

    verify-path <mazeFileName> <pathFileName>
        Verify that given maze path is valid

    render <mazeFileName> [imageFileName]
        Draw given maze and save image to imageFileName
        If no image filename provided, defaults to <mazeFileName basename>.ppm

    render-path <mazeFileName> <pathFileName> [imageFileName]
        Draw given maze, then draw given maze path on top, and then save image to imageFileName
        If no image filename provided, defaults to <mazeFileName basename>.path.ppm

    You can chain together tasks, and in that case, you don't have to repeat the same filenames.
    Example:

    generate 16384 16384 interesting.maze verify solve verify-path render render-path

    Will create the following files:

    interesting.maze
        The generated maze

    interesting.path
        The solution to the maze

    interesting.ppm
        The maze rendered as an image

    interesting.path.ppm
        The maze and solution rendered as an image
    """));

CLITask GenerateTask()
{
    var width = options.NextInt().MakeUneven();
    var height = options.NextInt().MakeUneven();
    int? seed = options.HasNextInt() ? options.NextInt() : null;
    mazeFileName = options.NextFileName($"{Environment.TickCount}.maze");
    var description = new { width, height, seed, mazeFileName };

    return (description, () =>
    {
        maze = Generate(Create(mazeFileName), width, height, seed);
        maze.Write();
        Console.WriteLine($"Saved maze to {mazeFileName}");
    });
}

CLITask VerifyTask()
{
    mazeFileName ??= options.Next();
    var description = new { mazeFileName };

    return (description, () =>
    {
        maze ??= Load(Open(mazeFileName));
        var result = IsPerfectMaze(maze);
        Console.WriteLine($"Is our maze perfect?: {result}");
    });
}

CLITask SolveTask()
{
    mazeFileName ??= options.Next();
    pathFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".path"));
    var description = new { mazeFileName, pathFileName };

    return (description, () =>
    {
        maze ??= Load(Open(mazeFileName));
        path = Solve(Create(pathFileName), maze);
        path.Write();
        Console.WriteLine($"Saved maze path to {pathFileName}");
    });
}

CLITask VerifyPathTask()
{
    mazeFileName ??= options.Next();
    pathFileName ??= options.Next();
    var description = new { mazeFileName, pathFileName };

    return (description, () =>
    {
        maze ??= Load(Open(mazeFileName));
        path ??= LoadPath(Open(pathFileName));
        var result = IsPerfectPath(maze, path);
        Console.WriteLine($"Is our maze path perfect?: {result}");
    });
}

CLITask RenderTask()
{
    mazeFileName ??= options.Next();
    var imageFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".ppm"));
    var description = new { mazeFileName, imageFileName };

    return (description, () =>
    {
        maze ??= Load(Open(mazeFileName));
        using var image = Render(Create(imageFileName), maze);
        image.Write();
        Console.WriteLine($"Saved maze image to {imageFileName}");
    });
}

CLITask RenderPathTask()
{
    mazeFileName ??= options.Next();
    pathFileName ??= options.Next();
    var imageFileName = options.NextFileName(Path.ChangeExtension(pathFileName, ".path.ppm"));
    var description = new { mazeFileName, pathFileName, imageFileName };

    return (description, () =>
    {
        maze ??= Load(Open(mazeFileName));
        path ??= LoadPath(Open(pathFileName));
        using var image = Render(Create(imageFileName), maze, path);
        image.Write();
        Console.WriteLine($"Saved maze with path image to {imageFileName}");
    });
}

static CLITask BenchmarkTask() => (null, () =>
{
    Generate(
        IStore.Create(false),
        BenchmarkSize,
        BenchmarkSize,
        BenchmarkSeed,
        MazeType.LongBitGridMaze,
        GeneratorType.Backtrack);
});
