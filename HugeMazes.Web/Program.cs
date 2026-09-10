using HugeMazes.Extensions;
using HugeMazes.IO;
using static HugeMazes.HugeMazes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.UseHttpsRedirection();

app.Map("/{width}/{height}/{seed?}", (int width, int height, int? seed) =>
{
    var id = Guid.NewGuid();
    using var maze = Generate(IStore.Create(), id, width.RoundDownOdd(), height.RoundDownOdd(), seed);
    var imageStream = new MemoryStream();
    using var image = Render(new StreamStore(imageStream, true), maze);
    image.Write();

    imageStream.Position = 0;
    return TypedResults.Stream(imageStream, "image/tiff", $"{id}.tiff");
});

app.Map("/path/{width}/{height}/{seed?}", (int width, int height, int? seed) =>
{
    var id = Guid.NewGuid();
    using var maze = Generate(IStore.Create(), id, width.RoundDownOdd(), height.RoundDownOdd(), seed);
    using var path = Solve(IStore.Create(), maze);
    var imageStream = new MemoryStream();
    using var image = Render(new StreamStore(imageStream, true), maze, path);
    image.Write();

    imageStream.Position = 0;
    return TypedResults.Stream(imageStream, "image/tiff", $"{id}.path.tiff");
});

app.Run();