namespace DeveMazeGeneratorCore.IO;

public class InsufficientDiskSpaceException(string name, long size, string drive, long freeSpace) : IOException
{
    public string Name => name;
    public long Size => size;
    public string Drive => drive;
    public long FreeSpace => freeSpace;
}
