using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Serializers;

namespace DeveMazeGeneratorCore.ConsoleApp;

public class JobContext
{
    public string? MazeFileName { get; set; }
    public IMaze? Maze { get; set; }
    public string? PathFileName { get; set; }
    public MazePath? Path { get; set; }

    public string DefaultMazeFileName => $"{Environment.TickCount}.maze";
    public string DefaultPathFileName => System.IO.Path.ChangeExtension(MazeFileName!, ".path");
    public string DefaultMazeImageFileName => System.IO.Path.ChangeExtension(MazeFileName!, ".png");
    public string DefaultPathImageFileName => System.IO.Path.ChangeExtension(PathFileName!, ".path.png");

    public async Task<IMaze> LoadMaze() => Maze ??= await MazeSerializer.Load(MazeFileName!);
    public async Task<MazePath> LoadPath() => Path ??= await MazePathSerializer.Load(PathFileName!);
}

public interface IJob
{
    Task Run(JobContext context);
}

public readonly record struct GenerateJob(int Width, int Height, int? Seed, string MazeFileName) : IJob
{
    public static GenerateJob Parse(Options options, JobContext context)
    {
        var job = new GenerateJob(
            options.NextInt().MakeUneven(),
            options.NextInt().MakeUneven(),
            options.HasNextInt() ? options.NextInt() : null,
            options.NextFileName(context.DefaultMazeFileName));
        context.MazeFileName = job.MazeFileName;
        return job;
    }

    public async Task Run(JobContext context)
    {
        context.Maze = DeveMazeGeneratorCore.Generate(Width, Height, Seed);
        await MazeSerializer.Save(MazeFileName, context.Maze);
        Console.WriteLine($"Saved maze to {MazeFileName}");
    }
}

public readonly record struct VerifyJob() : IJob
{
    public static VerifyJob Parse(Options options, JobContext context)
    {
        context.MazeFileName ??= options.Next();
        return new();
    }

    public async Task Run(JobContext context)
    {
        var maze = await context.LoadMaze();
        var result = Verifier.IsPerfectMaze(maze);
        Console.WriteLine($"Is our maze perfect?: {result}");
    }
}

public readonly record struct SolveJob(string PathFileName) : IJob
{
    public static SolveJob Parse(Options options, JobContext context)
    {
        context.MazeFileName ??= options.Next();
        context.PathFileName = options.NextFileName(context.DefaultPathFileName);
        return new(context.PathFileName);
    }

    public async Task Run(JobContext context)
    {
        var maze = await context.LoadMaze();
        context.Path = PathFinder.Find(maze);
        await MazePathSerializer.Save(PathFileName, context.Path);
        Console.WriteLine($"Saved solution to {PathFileName}");
    }
}

public readonly record struct RenderJob(string ImageFileName) : IJob
{
    public static RenderJob Parse(Options options, JobContext context)
    {
        context.MazeFileName ??= options.Next();
        return new(options.NextFileName(context.DefaultMazeImageFileName));
    }

    public async Task Run(JobContext context)
    {
        var maze = await context.LoadMaze();
        using var image = ImageCreator.CreateImage(maze);
        await ImageCreator.Save(ImageFileName, image);

        Console.WriteLine($"Saved maze image to {ImageFileName}");
    }
}

public readonly record struct RenderPathJob(string ImageFileName) : IJob
{
    public static RenderPathJob Parse(Options options, JobContext context)
    {
        context.MazeFileName ??= options.Next();
        context.PathFileName ??= options.Next();
        return new(options.NextFileName(context.DefaultPathImageFileName));
    }

    public async Task Run(JobContext context)
    {
        var maze = await context.LoadMaze();
        var path = await context.LoadPath();
        using var image = ImageCreator.CreateImage(maze, path);
        await ImageCreator.Save(ImageFileName, image);

        Console.WriteLine($"Saved maze with solution image to {ImageFileName}");
    }
}

public readonly record struct BenchmarkJob() : IJob
{
    public static BenchmarkJob Parse(Options options, JobContext context) => new();

    public async Task Run(JobContext context)
    {
        var maze = DeveMazeGeneratorCore.BenchmarkBaseline();
        var result = Verifier.IsPerfectMaze(maze);
        if(!result) throw new InvalidOperationException("Maze is not perfect");
    }
}
