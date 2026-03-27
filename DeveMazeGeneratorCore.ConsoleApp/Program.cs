using DeveMazeGeneratorCore;
using DeveMazeGeneratorCore.ConsoleApp;

var options = new Options(args);
Maze? maze = null;
string? mazeFileName = null;
MazePath? path = null;
string? pathFileName = null;

while(options.HasNext())
{
    var task = options.Next();
    if(task == "generate")
    {
        await Generate();
    }
    else if(task == "verify")
    {
        await Verify();
    }
    else if(task == "solve")
    {
        await Solve();
    }
    else if(task == "image")
    {
        await MazeImage();
    }
    else if(task == "path-image")
    {
        await PathImage();
    }
    else if(task == "benchmark")
    {
        Benchmark();
    }
}

async Task Generate()
{
    var width = options.NextInt();
    var height = options.NextInt();
    int? seed = options.HasNextInt() ? options.NextInt() : null;
    mazeFileName = options.NextFilename($"{Environment.TickCount}.maze");

    maze = DeveMazeGeneratorCore.DeveMazeGeneratorCore.Generate(width, height, seed);
    await maze.Save(mazeFileName);
    Console.WriteLine($"Saved maze to {mazeFileName}");
}

async Task Verify()
{
    if(maze == null)
    {
        mazeFileName = options.Next();
        maze = await Maze.Load(mazeFileName);
    }

    var result = Verifier.IsPerfectMaze(maze);
    Console.WriteLine($"Is our maze perfect?: {result}");
}

async Task Solve()
{
    if(maze == null)
    {
        mazeFileName = options.Next();
        maze = await Maze.Load(mazeFileName);
    }

    pathFileName = options.NextFilename(Path.ChangeExtension(mazeFileName, ".path")!);

    path = Solver.Solve(maze);
    await path.Save(pathFileName);

    Console.WriteLine($"Saved solution to {pathFileName}");
}

async Task MazeImage()
{
    if(maze == null)
    {
        mazeFileName = options.Next();
        maze = await Maze.Load(mazeFileName);
    }

    var imageFileName = options.NextFilename(Path.ChangeExtension(mazeFileName, ".png")!);
   
    using var image = ImageCreator.CreateImage(maze);
    using var fs = File.Open(imageFileName, FileMode.Create);
    ImageCreator.SaveImage(image, fs);

    Console.WriteLine($"Saved maze image to {imageFileName}");
}

async Task PathImage()
{
    if(maze == null)
    {
        mazeFileName = options.Next();
        maze = await Maze.Load(mazeFileName);
    }

    if(path == null)
    {
        pathFileName = options.Next();
        path = await MazePath.Load(pathFileName);
    }

    var imageFileName = options.NextFilename(Path.ChangeExtension(pathFileName, ".path.png")!);

    using var image = ImageCreator.CreateImage(maze, path);
    using var fs = File.Open(imageFileName, FileMode.Create);
    ImageCreator.SaveImage(image, fs);

    Console.WriteLine($"Saved maze with solution image to {imageFileName}");
}

void Benchmark()
{
    var maze = DeveMazeGeneratorCore.DeveMazeGeneratorCore.BenchmarkBaseline();
    var result = Verifier.IsPerfectMaze(maze);
    if(!result) throw new InvalidOperationException("Maze is not perfect");
}
