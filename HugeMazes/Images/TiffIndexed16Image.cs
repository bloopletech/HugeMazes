using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffIndexed16Image : Storable, IImage<byte>
{
    public const int PaletteSize = 16;
    private const long MazeIdOffset = 272;
    private const long MazeIdLength = 37;
    private const long PaletteOffset = MazeIdOffset + MazeIdLength;
    private const long PaletteCount = PaletteSize * 3;
    private const long PaletteLength = PaletteCount * sizeof(short);
    private const long ArrayOffset = PaletteOffset + PaletteLength;

    private readonly LongArray<byte> array;
    private bool written;
    private readonly Guid mazeId;
    private readonly MazeSize size;
    private readonly MazeColor[] palette;
    private readonly int arrayWidth;

    public TiffIndexed16Image(
        IStore store,
        Guid mazeId,
        MazeSize size,
        MazeColor[] palette,
        bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.mazeId = mazeId;
        this.size = size;
        this.palette = palette;
        arrayWidth = size.Width.RoundUpEven();
        array = new(store.Offset(ArrayOffset - sizeof(long), true), (arrayWidth * size.Height).DivCeil(2), true);
    }

    public override long Extent => array.Extent + ArrayOffset;
    public Guid MazeId => mazeId;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;
    public MazeColor[] Palette => palette;

    public byte this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var (index, isHigh) = Index(x, y);
            var (high, low) = SplitByte(array[index]);
            return isHigh ? high : low;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (index, isHigh) = Index(x, y);
            var (high, low) = SplitByte(array[index]);
            if(isHigh) high = value;
            else low = value;
            array[index] = JoinByte(high, low);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (long, bool) Index(int x, int y)
    {
        var index = x + ((long)y * arrayWidth);
        return (index / 2, long.IsEvenInteger(index));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (byte, byte) SplitByte(byte value) => ((byte)((value & 0b1111_0000) >> 4), (byte)(value & 0b1111));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte JoinByte(byte high, byte low) => (byte)((high << 4) | low);

    public override void Read()
    {
    }

    public override void Write()
    {
        if(written) return;
        written = true;

        array.Write();

        var mazeIdBytes = Tiff.GetAsciiBytes(mazeId.ToString());

        store.Write<byte>(0, [
            ..Tiff.FixedHeader(12),
            ..Tiff.Tag(Tiff.TagType.Width, (uint)size.Width),
            ..Tiff.Tag(Tiff.TagType.Height, (uint)size.Height),
            ..Tiff.Tag(Tiff.TagType.BitsPerSample, 0x04),
            ..Tiff.Tag(Tiff.TagType.Compression, 0x01),
            ..Tiff.Tag(Tiff.TagType.PhotometricInterpolation, 0x03),
            ..Tiff.Tag(Tiff.TagType.ImageDescription, Tiff.ValueType.Ascii, mazeIdBytes.Length - 1, MazeIdOffset),
            ..Tiff.Tag(Tiff.TagType.StripOffsets, ArrayOffset),
            ..Tiff.Tag(Tiff.TagType.Orientation, 0x01),
            ..Tiff.Tag(Tiff.TagType.RowsPerStrip, uint.MaxValue),
            ..Tiff.Tag(Tiff.TagType.StripByteCount, (ulong)array.Length),
            ..Tiff.Tag(Tiff.TagType.PlanarConfiguration, 0x01),
            ..Tiff.Tag(Tiff.TagType.ColorMap, Tiff.ValueType.Short, PaletteCount, PaletteOffset),
            ..BitConverter.GetBytes(0L),
            ..mazeIdBytes,
            ..Tiff.MapPaletteBytes(palette.Extend(PaletteSize))
        ]);
    }
}
