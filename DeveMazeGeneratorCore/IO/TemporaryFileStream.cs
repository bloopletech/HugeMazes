namespace DeveMazeGeneratorCore.IO;

public class TemporaryFileStream(FileMode mode, FileAccess access) : FileStream(Path.GetTempFileName(), mode, access)
{
    public TemporaryFileStream() : this(FileMode.Open, FileAccess.ReadWrite)
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if(disposing) File.Delete(Name);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        File.Delete(Name);
        GC.SuppressFinalize(this);
    }
}
