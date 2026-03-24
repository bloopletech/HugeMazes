using System.Collections.Generic;

namespace DeveMazeGeneratorCore.Structures;

public class KruskalCell(int x, int y)
{
    public int Y { get; set; } = y;
    public int X { get; set; } = x;
    public bool Solid { get; set; }
    public List<KruskalCell> CellSet { get; set; } = new List<KruskalCell>();

    public string xeny()
    {
        return $"{X}-{Y}";
    }
}
