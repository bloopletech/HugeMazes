using DeveMazeGeneratorCore.Mazes.InnerStuff;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArreintjeFastInnerMap : IMaze
{
    private readonly int width;
    private readonly int height;
    private BitArreintjeFastInnerMapArray[] _innerData;

    public BitArreintjeFastInnerMap(int width, int height)
    {
        if(width < 3) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must >= 3");
        if(height < 3) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must >= 3");
        this.width = width;
        this.height = height;
        _innerData = new BitArreintjeFastInnerMapArray[width];
        for(int i = 0; i < width; i++)
        {
            _innerData[i] = new BitArreintjeFastInnerMapArray(height);
        }
    }

    public int Width => width;
    public int Height => height;

    public IMaze Clone()
    {
        var innerMapTarget = new BitArreintjeFastInnerMap(width, height);
        for(int i = 0; i < _innerData.Length; i++)
        {
            innerMapTarget._innerData[i] = _innerData[i].Clone();
        }
        return innerMapTarget;
    }

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _innerData[x][y];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _innerData[x][y] = value;
        }
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}
