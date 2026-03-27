namespace DeveMazeGeneratorCore.ConsoleApp;

public class Options(string[] args)
{
    private Queue<string> queue = new(args);

    public string Peek() => queue.Peek();
    public bool HasNext() => queue.Count > 0;

    public string Next() => queue.Dequeue();
    public string Next(string fallback) => HasNext() ? Next() : fallback;

    public bool HasNextInt() => HasNext() && int.TryParse(Peek(), System.Globalization.NumberStyles.None, null, out _);
    public int NextInt() => int.Parse(Next(), System.Globalization.NumberStyles.None);

    public bool HasNextFilename() => HasNext() && Peek().Contains('.');
    public string NextFilename(string fallback) => HasNextFilename() ? Next() : fallback;
}
