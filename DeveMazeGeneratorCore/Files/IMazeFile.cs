using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Files;

public static class IMazeFile
{
    public static void Save(string fileName, IMaze maze)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        IMazeSerializer.Serialize(fs, maze);
    }

    public static async Task SaveAsync(string fileName, IMaze maze)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await IMazeSerializer.SerializeAsync(fs, maze);
    }

    public static IMaze Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return IMazeSerializer.Deserialize(fs);
    }

    public static async Task<IMaze> LoadAsync(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await IMazeSerializer.DeserializeAsync(fs);
    }
}
