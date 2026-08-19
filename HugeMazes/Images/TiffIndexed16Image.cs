using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffIndexed16Image(
    IStore store,
    Guid mazeId,
    MazeSize size,
    MazeColor[] palette,
    bool leaveOpen = false) : Storable(store, leaveOpen), IImage<byte>
{
    public const int PaletteSize = 16;
    private static readonly long MazeIdOffset = Tiff.HeaderLength + Tiff.DirectoryLength(12);
    private const long MazeIdLength = 37;
    private static readonly long PaletteOffset = MazeIdOffset + MazeIdLength;
    private const long PaletteCount = PaletteSize * 3;
    private const long PaletteLength = PaletteCount * sizeof(short);
    private static readonly long ArrayOffset = PaletteOffset + PaletteLength;

    private readonly LongArray<byte> array = new(
        store.Offset(ArrayOffset - sizeof(long), true),
        (size.WidthStride * size.Height).DivCeil(2),
        true);
    private bool written;

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
            return (byte)(isHigh ? (array[index] & 0xf0) >> 4 : array[index] & 0x0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (index, isHigh) = Index(x, y);
            ref var segment = ref array.Get(index);
            segment = (byte)(isHigh ? (value << 4) | (byte)(segment & 0x0f) : (byte)(segment & 0xf0) | value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (long, bool) Index(int x, int y)
    {
        var index = x + ((long)y * size.WidthStride);
        return (index >> 1, long.IsEvenInteger(index));
    }

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
            ..Tiff.Header,
            ..Tiff.Directory([
                Tiff.Tag(Tiff.TagType.Width, (uint)size.Width),
                Tiff.Tag(Tiff.TagType.Height, (uint)size.Height),
                Tiff.Tag(Tiff.TagType.BitsPerSample, 0x04),
                Tiff.Tag(Tiff.TagType.Compression, 0x01),
                Tiff.Tag(Tiff.TagType.PhotometricInterpolation, 0x03),
                Tiff.Tag(Tiff.TagType.ImageDescription, Tiff.ValueType.Ascii, mazeIdBytes.Length - 1, MazeIdOffset),
                Tiff.Tag(Tiff.TagType.StripOffsets, (ulong)ArrayOffset),
                Tiff.Tag(Tiff.TagType.Orientation, 0x01),
                Tiff.Tag(Tiff.TagType.RowsPerStrip, uint.MaxValue),
                Tiff.Tag(Tiff.TagType.StripByteCount, (ulong)array.Length),
                Tiff.Tag(Tiff.TagType.PlanarConfiguration, 0x01),
                Tiff.Tag(Tiff.TagType.ColorMap, Tiff.ValueType.Short, PaletteCount, PaletteOffset)
            ]),
            ..mazeIdBytes,
            ..Tiff.GetColorMapBytes(palette.Extend(PaletteSize))
        ]);
    }
}
