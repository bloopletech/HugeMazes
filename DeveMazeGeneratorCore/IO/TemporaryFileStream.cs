namespace DeveMazeGeneratorCore.IO;

public class TemporaryFileStream : FileStream
{
    public TemporaryFileStream() : this(FileMode.Open, FileAccess.ReadWrite)
    {
    }

    public TemporaryFileStream(FileMode mode, FileAccess access) : base(Path.GetTempFileName(), mode, access)
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
    }
}
