using System.Runtime.CompilerServices;
using System.Text;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes;

public class LongImage : Storable, IImage
{
    private LongArray<Colour> array;
    private MazeSize size;

    public LongImage(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        array = null!;
    }

    public LongImage(IStore store, MazeSize size, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        array = new(store.Offset(Offset, true), size.Area, true);
    }

    public override long Extent => array.Extent + Offset;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    private byte[] Header => Encoding.ASCII.GetBytes($"P6 {size.Width:D} {size.Height:D} 255\n");
    private int Offset => Header.Length - sizeof(long);

    public Colour this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var index = x + ((long)y * size.Width);
            return array[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var index = x + ((long)y * size.Width);
            array[index] = value;
        }
    }

    public IEnumerable<(int, int)> ByPixel()
    {
        for(var y = 0; y < size.Height; y++)
        {
            for(var x = 0; x < size.Width; x++) yield return (x, y);
        }
    }

    public override void Read()
    {
        size = store.Read<MazeSize>(0);
        array = new(store.Offset(Offset, true), size.Area, true);
    }

    public override void Write()
    {
        array.Write();
        store.Write(0, Header);
    }
}
