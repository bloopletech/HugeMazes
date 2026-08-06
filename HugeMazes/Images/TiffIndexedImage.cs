using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffIndexedImage(IStore store, Guid mazeId, MazeSize size, MazeColor[] palette) : Storable(store, false), IImage<byte>
{
    public const int PaletteSize = 256;
    private const long MazeIdOffset = 252;
    private const long MazeIdLength = 37;
    private const long PaletteOffset = MazeIdOffset + MazeIdLength;
    private const long PaletteCount = PaletteSize * 3;
    private const long PaletteLength = PaletteCount * sizeof(ushort);
    private const long ArrayOffset = PaletteOffset + PaletteLength;

    private readonly LongArray<byte> array = new(store.Offset(ArrayOffset - sizeof(long), true), size.Area, true);
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

    public override void Read()
    {
    }

    public override void Write()
    {
        if(written) return;
        written = true;

        array.Write();

        var deflateStore = store.Offset(ArrayOffset, true);
        StoreDeflater.Deflate(deflateStore);

        var mazeIdBytes = Tiff.GetAsciiBytes(mazeId.ToString());

        store.Write<byte>(0, [
            ..Tiff.FixedHeader(11),
            ..Tiff.Tag(Tiff.TagType.Width, (uint)size.Width),
            ..Tiff.Tag(Tiff.TagType.Height, (uint)size.Height),
            ..Tiff.Tag(Tiff.TagType.BitsPerSample, 0x08),
            ..Tiff.Tag(Tiff.TagType.Compression, 0x08),
            ..Tiff.Tag(Tiff.TagType.PhotometricInterpolation, 0x03),
            ..Tiff.Tag(Tiff.TagType.ImageDescription, Tiff.ValueType.Ascii, mazeIdBytes.Length - 1, MazeIdOffset),
            ..Tiff.Tag(Tiff.TagType.StripOffsets, ArrayOffset),
            ..Tiff.Tag(Tiff.TagType.RowsPerStrip, (uint)size.Width),
            ..Tiff.Tag(Tiff.TagType.StripByteCount, (ulong)deflateStore.Length),
            ..Tiff.Tag(Tiff.TagType.PlanarConfiguration, 0x01),
            //XResolution
            //YResolution
            //ResolutionUnit
            ..Tiff.Tag(Tiff.TagType.ColorMap, Tiff.ValueType.Short, PaletteCount, PaletteOffset),
            ..BitConverter.GetBytes(0L),
            ..mazeIdBytes,
            ..Tiff.MapPaletteBytes(palette.Extend(PaletteSize))
        ]);
    }


}
