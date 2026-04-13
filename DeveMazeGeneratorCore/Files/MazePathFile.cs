namespace DeveMazeGeneratorCore.Files;

public static class MazePathFile
{
    public static void Save(string fileName, MazePath path)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        MazePathSerializer.Serialize(fs, path);
    }

    public static async Task SaveAsync(string fileName, MazePath path)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await MazePathSerializer.SerializeAsync(fs, path);
    }

    public static MazePath Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return MazePathSerializer.Deserialize(fs);
    }

    public static async Task<MazePath> LoadAsync(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await MazePathSerializer.DeserializeAsync(fs);
    }
}
