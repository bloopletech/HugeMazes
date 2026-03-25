using DeveMazeGeneratorCore.Imageification;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using System.Diagnostics;
using System.Reflection;

namespace DeveMazeGeneratorCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class MazesController : ControllerBase
{
    private FontCollection _fontCollection;

    //private static ConcurrentDictionary<int, byte[]> _zoomImageCache = new();

    public MazesController()
    {
        var assembly = Assembly.GetExecutingAssembly();

        _fontCollection = new();
        using Stream stream = assembly.GetManifestResourceStream("DeveMazeGeneratorCore.Web.SecularOne-Regular.ttf");
        _fontCollection.Add(stream);
    }

    // GET api/values
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "Test1", "value2" };
    }

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("Maze/{width}/{height}", Name = "GenerateMaze")]
    public ActionResult GenerateMaze(int width, int height)
    {
        var maze = MazeGenerator.Generate(width, height);

        using (var memoryStream = new MemoryStream())
        {
            WithoutPath.MazeToImage(maze, memoryStream);

            var data = memoryStream.ToArray();
            return File(data, "image/png");
        }
    }

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("MazePath/{width}/{height}", Name = "GenerateMazeWithPath")]
    public ActionResult GenerateMazeWithPath(int width, int height)
    {
        var w = Stopwatch.StartNew();
        var maze = MazeGenerator.Generate(width, height);
        var mazeGenerationTime = w.Elapsed;

        w.Restart();
        var path = PathFinder.GoFind(maze, null);
        var pathGenerationTime = w.Elapsed;

        w.Restart();
        using (var memoryStream = new MemoryStream())
        {
            WithPath.SaveMazeAsImageDeluxePng(maze, path, memoryStream);
            var toImageTime = w.Elapsed;

            Console.WriteLine($"Maze generation time: {mazeGenerationTime}, Path find time: {pathGenerationTime}, To image time: {toImageTime}");

            var data = memoryStream.ToArray();
            return File(data, "image/png");
        }
    }

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("MazePathSeed/{seed}/{width}/{height}", Name = "GenerateMazeWithPathSeed")]
    public ActionResult GenerateMazeWithPathSeed(int seed, int width, int height)
    {
        var w = Stopwatch.StartNew();
        var maze = MazeGenerator.Generate(width, height, seed);
        var mazeGenerationTime = w.Elapsed;

        w.Restart();
        var path = PathFinder.GoFind(maze, null);
        var pathGenerationTime = w.Elapsed;

        w.Restart();
        using (var memoryStream = new MemoryStream())
        {
            WithPath.SaveMazeAsImageDeluxePng(maze, path, memoryStream);
            var toImageTime = w.Elapsed;

            Console.WriteLine($"Maze generation time: {mazeGenerationTime}, Path find time: {pathGenerationTime}, To image time: {toImageTime}");

            var data = memoryStream.ToArray();
            return File(data, "image/png");
        }
    }
}
