using DeveMazeGeneratorCore;
using DeveMazeGeneratorCore.ConsoleApp;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Serializers;

var slowMode = true;
var options = new Options(args);
IMaze? maze = null;
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

async Task LoadMaze()
{
    if (maze == null)
    {
        mazeFileName = options.Next();
        maze = await MazeSerializer.Load(mazeFileName);
    }
    else if (slowMode)
    {
        maze = await MazeSerializer.Load(mazeFileName);
    }
}

async Task LoadPath()
{
    if (path == null)
    {
        pathFileName = options.Next();
        path = await MazePathSerializer.Load(pathFileName);
    }
    else if (slowMode)
    {
        path = await MazePathSerializer.Load(pathFileName);
    }
}

async Task Generate()
{
    var width = MathExtensions.MakeUneven(options.NextInt());
    var height = MathExtensions.MakeUneven(options.NextInt());
    int? seed = options.HasNextInt() ? options.NextInt() : null;
    mazeFileName = options.NextFilename($"{Environment.TickCount}.maze");

    maze = DeveMazeGeneratorCore.DeveMazeGeneratorCore.Generate(width, height, seed);
    await MazeSerializer.Save(mazeFileName, maze);
    Console.WriteLine($"Saved maze to {mazeFileName}");
}

async Task Verify()
{
    await LoadMaze();
    var result = Verifier.IsPerfectMaze(maze);
    Console.WriteLine($"Is our maze perfect?: {result}");
}

async Task Solve()
{
    await LoadMaze();

    pathFileName = options.NextFilename(Path.ChangeExtension(mazeFileName, ".path")!);

    path = PathFinder.Find(maze);
    await MazePathSerializer.Save(pathFileName, path);

    Console.WriteLine($"Saved solution to {pathFileName}");
}

async Task MazeImage()
{
    await LoadMaze();

    var imageFileName = options.NextFilename(Path.ChangeExtension(mazeFileName, ".png")!);
   
    using var image = ImageCreator.CreateImage(maze);
    await ImageCreator.SaveImage(imageFileName, image);

    Console.WriteLine($"Saved maze image to {imageFileName}");
}

async Task PathImage()
{
    await LoadMaze();
    await LoadPath();

    var imageFileName = options.NextFilename(Path.ChangeExtension(pathFileName, ".path.png")!);

    using var image = ImageCreator.CreateImage(maze, path);
    await ImageCreator.SaveImage(imageFileName, image);

    Console.WriteLine($"Saved maze with solution image to {imageFileName}");
}

void Benchmark()
{
    var maze = DeveMazeGeneratorCore.DeveMazeGeneratorCore.BenchmarkBaseline();
    var result = Verifier.IsPerfectMaze(maze);
    if(!result) throw new InvalidOperationException("Maze is not perfect");
}
