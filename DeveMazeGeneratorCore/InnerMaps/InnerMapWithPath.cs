namespace DeveMazeGeneratorCore.InnerMaps;

public class InnerMapWithPath<M>(int width, int height, int startX, int startY, M map, M pathMap) : IMapPart where M : InnerMap
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public int StartX { get; } = startX;
    public int StartY { get; } = startY;

    public M Map { get; } = map;
    public M PathMap { get; } = pathMap;
}
