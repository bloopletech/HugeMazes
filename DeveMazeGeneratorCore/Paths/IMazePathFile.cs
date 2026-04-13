namespace DeveMazeGeneratorCore.Paths;

public static class IMazePathFile
{
    public static void Save(string fileName, IMazePath path)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        IMazePathSerializer.Serialize(fs, path);
    }

    public static async Task SaveAsync(string fileName, IMazePath path)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await IMazePathSerializer.SerializeAsync(fs, path);
    }

    public static IMazePath Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return IMazePathSerializer.Deserialize(fs);
    }

    public static async Task<IMazePath> LoadAsync(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await IMazePathSerializer.DeserializeAsync(fs);
    }
}
