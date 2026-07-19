using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffIndexed16Image : Storable, IIndexedImage
{
    public const int PaletteSize = 16;
    private const long HeaderLength = 252;
    private const long ColorMapCount = PaletteSize * 3;
    private const long ColorMapLength = ColorMapCount * sizeof(short);
    private const long ArrayOffset = HeaderLength + ColorMapLength;

    private readonly LongArray<byte> array;
    private readonly MazeSize size;
    private readonly MazeColor[] palette;
    private readonly int arrayWidth;

    public TiffIndexed16Image(
        IStore store,
        MazeSize size,
        MazeColor[] palette,
        bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.size = size;
        this.palette = palette;
        arrayWidth = size.Width.RoundUpEven();
        array = new(store.Offset(ArrayOffset, true), (arrayWidth * size.Height).DivCeil(2), true);
    }

    public override long Extent => array.Extent + ArrayOffset;
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
        var magic = BitConverter.IsLittleEndian ? (byte)0x49 : (byte)0x4D;
        byte[] header = [
            magic,
            magic,
            ..BitConverter.GetBytes((ushort)0x2B),
            ..BitConverter.GetBytes((ushort)0x08),
            0x00,
            0x00,
            ..BitConverter.GetBytes(16L),
            ..BitConverter.GetBytes(11L),
            ..new TiffTag(TiffTag.TagType.Width, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.Height, [(uint)size.Height]),
            ..new TiffTag(TiffTag.TagType.BitsPerSample, [0x04]),
            ..new TiffTag(TiffTag.TagType.Compression, [0x01]),
            ..new TiffTag(TiffTag.TagType.PhotometricInterpolation, [0x03]),
            ..new TiffTag(TiffTag.TagType.StripOffsets, [(ulong)ArrayOffset + sizeof(long)]),
            ..new TiffTag(TiffTag.TagType.Orientation, [0x01]),
            ..new TiffTag(TiffTag.TagType.RowsPerStrip, [uint.MaxValue]),
            ..new TiffTag(TiffTag.TagType.StripByteCount, [(ulong)array.Length]),
            ..new TiffTag(TiffTag.TagType.PlanarConfiguration, [0x01]),
            ..new TiffTag(TiffTag.TagType.ColorMap, TiffTag.ValueType.Short, ColorMapCount, BitConverter.GetBytes(HeaderLength)),
            ..BitConverter.GetBytes(0L),
            ..MemoryMarshal.AsBytes(MapPalette()),
        ];

        store.Write(0, header);
        array.Write();
    }

    private ushort[] MapPalette()
    {
        MazeColor[] extendedPalette = [..palette, ..new MazeColor[PaletteSize - palette.Length]];

        var reds = new ushort[PaletteSize];
        var greens = new ushort[PaletteSize];
        var blues = new ushort[PaletteSize];

        for(var i = 0; i < extendedPalette.Length; i++)
        {
            reds[i] = Scale(extendedPalette[i].R);
            greens[i] = Scale(extendedPalette[i].G);
            blues[i] = Scale(extendedPalette[i].B);
        }

        return [..reds, ..greens, ..blues];
    }

    private static ushort Scale(byte value) => (ushort)(value * ushort.MaxValue / (double)byte.MaxValue);
}
