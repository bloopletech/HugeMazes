using DeveMazeGeneratorCore.ConsoleApp;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using static DeveMazeGeneratorCore.DeveMazeGeneratorCore;
using static DeveMazeGeneratorCore.Verifier;

var skipReuse = true;
IStore.LongOverride = true;

var options = new Options(args);

if(!options.HasNext()) options = new(["help"]);
if(options.HasNextInt())
{
    var width = options.Next();
    var height = options.HasNext() ? options.Next() : width;
    options = new(["generate", width, height, "verify", "solve", "verify-path", "render", "render-path"]);
}

string? mazeFileName = null;
IMaze? maze = null;
string? pathFileName = null;
IMazePath? path = null;

Console.WriteLine($"Invoked with: {string.Join(' ', args)}");
Console.WriteLine($"Resolves to: {options}");

var tasks = new List<(string, Action)>();
while(options.HasNext()) tasks.Add(CreateTask(options.Next()));

Console.WriteLine("Running tasks:");
foreach(var description in tasks.Select(t => t.Item1)) Console.WriteLine(description);
Console.WriteLine();

try
{
    foreach(var task in tasks.Select(t => t.Item2))
    {
        task();

        if(skipReuse)
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

(string, Action) CreateTask(string task) => task switch
{
    "help" => HelpTask(),
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

static (string, Action) HelpTask() => ("help", () => Console.Write("""
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

(string, Action) GenerateTask()
{
    var width = options.NextInt().MakeUneven();
    var height = options.NextInt().MakeUneven();
    int? seed = options.HasNextInt() ? options.NextInt() : null;
    mazeFileName = options.NextFileName($"{Environment.TickCount}.maze");
    var description = new { width, height, seed, mazeFileName };

    return ($"generate {description}", () =>
    {
        Console.WriteLine($"Generating maze...");
        maze = Generate(width, height, seed, new StreamStore(mazeFileName));
        maze.Write();
        Console.WriteLine($"Saved maze to {mazeFileName}");
    });
}

(string, Action) VerifyTask()
{
    mazeFileName ??= options.Next();
    var description = new { mazeFileName };

    return ($"verify {description}", () =>
    {
        Console.WriteLine($"Verifying maze...");
        maze ??= Load(mazeFileName);
        var result = IsPerfectMaze(maze);
        Console.WriteLine($"Is our maze perfect?: {result}");
    });
}

(string, Action) SolveTask()
{
    mazeFileName ??= options.Next();
    pathFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".path"));
    var description = new { mazeFileName, pathFileName };

    return ($"solve {description}", () =>
    {
        Console.WriteLine($"Solving maze...");
        maze ??= Load(mazeFileName);
        path = Solve(maze, new StreamStore(pathFileName));
        path.Write();
        Console.WriteLine($"Saved maze path to {pathFileName}");
    });
}

(string, Action) VerifyPathTask()
{
    mazeFileName ??= options.Next();
    pathFileName ??= options.Next();
    var description = new { mazeFileName, pathFileName };

    return ($"verify-path {description}", () =>
    {
        Console.WriteLine($"Verifying maze path...");
        maze ??= Load(mazeFileName);
        path ??= LoadPath(pathFileName);
        var result = IsPerfectPath(maze, path);
        Console.WriteLine($"Is our maze path perfect?: {result}");
    });
}

(string, Action) RenderTask()
{
    mazeFileName ??= options.Next();
    var imageFileName = options.NextFileName(Path.ChangeExtension(mazeFileName, ".ppm"));
    var description = new { mazeFileName, imageFileName };

    return ($"render {description}", () =>
    {
        Console.WriteLine($"Rendering maze...");
        maze ??= Load(mazeFileName);
        using var image = Render(maze, new StreamStore(imageFileName));
        image.Write();
        Console.WriteLine($"Saved maze image to {imageFileName}");
    });
}

(string, Action) RenderPathTask()
{
    mazeFileName ??= options.Next();
    pathFileName ??= options.Next();
    var imageFileName = options.NextFileName(Path.ChangeExtension(pathFileName, ".path.ppm"));
    var description = new { mazeFileName, pathFileName, imageFileName };

    return ($"render-path {description}", () =>
    {
        Console.WriteLine($"Rendering maze path...");
        maze ??= Load(mazeFileName);
        path ??= LoadPath(pathFileName);
        using var image = Render(maze, path, new StreamStore(imageFileName));
        image.Write();
        Console.WriteLine($"Saved maze with path image to {imageFileName}");
    });
}

static (string, Action) BenchmarkTask() => ("benchmark", () =>
{
    var maze = BenchmarkBaseline();
    var result = IsPerfectMaze(maze);
    if(!result) throw new InvalidOperationException("Maze is not perfect");
});

static (string, Action) BenchmarkDirectionMazePathTask() => ("benchmark-direction-maze-path", () =>
{
    var maze = BenchmarkBaseline();
    var path = Solve(maze);
    var result = IsPerfectPath(maze, path);
    if(!result) throw new InvalidOperationException("Path is not perfect");
});

static IMaze Load(string fileName) => MazeSerializer.Read(new StreamStore(fileName));

static IMazePath LoadPath(string fileName) => MazePathSerializer.Read(new StreamStore(fileName));
