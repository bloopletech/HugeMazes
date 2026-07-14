using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class LongIndexedTiffImage(
    IStore store,
    MazeSize size,
    MazeColor[] palette,
    bool leaveOpen = false) : Storable(store, leaveOpen), IIndexedImage
{
    public const int PaletteSize = 256;
    private const long HeaderSize = 1848;

    private readonly LongArray<byte> array = new(store.Offset(HeaderSize, true), size.Area, true);

    public override long Extent => array.Extent + HeaderSize;
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
        byte[] header = [
            0x49,
            0x49,
            ..BitConverter.GetBytes((ushort)0x2B),
            ..BitConverter.GetBytes((ushort)0x08),
            0x00,
            0x00,
            ..BitConverter.GetBytes(16L),
            ..BitConverter.GetBytes(14L),
            ..new TiffTag(TiffTag.TagType.Width, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.Height, [(uint)size.Height]),
            ..new TiffTag(TiffTag.TagType.BitsPerSample, [0x08, 0x08, 0x08]),
            ..new TiffTag(TiffTag.TagType.Compression, [0x01]),
            ..new TiffTag(TiffTag.TagType.PhotometricInterpolation, [0x03]),
            ..new TiffTag(TiffTag.TagType.StripOffsets, [(ulong)HeaderSize + sizeof(long)]),
            ..new TiffTag(TiffTag.TagType.Orientation, [0x01]),
            ..new TiffTag(TiffTag.TagType.SamplesPerPixel, [0x01]),
            ..new TiffTag(TiffTag.TagType.RowsPerStrip, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.StripByteCount, [(ulong)size.Area]),
            ..new TiffTag(TiffTag.TagType.MinimumSampleValue, [0, 0, 0]),
            ..new TiffTag(TiffTag.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]),
            ..new TiffTag(TiffTag.TagType.PlanarConfiguration, [0x01]),
            //..new TiffTag(TiffTag.TagType.SampleFormat, [(short)0x01, (short)0x01, (short)0x01]),
            ..new TiffTag(TiffTag.TagType.ColorMap, TiffTag.ValueType.Short, PaletteSize * 3, BitConverter.GetBytes(312L)),
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

/*
ag = 320 (140.H)
Type = SHORT
N = 3 * (2**BitsPerSample)
This field defines a Red-Green-Blue color map (often called a lookup table) for
palette color images. In a palette-color image, a pixel value is used to index into an
RGB-lookup table. For example, a palette-color pixel having a value of 0 would
be displayed according to the 0th Red, Green, Blue triplet.
In a TIFF ColorMap, all the Red values come first, followed by the Green values,
then the Blue values. In the ColorMap, black is represented by 0,0,0 and white is
represented by 65535, 65535, 6553
*/