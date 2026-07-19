using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffIndexedImage(IStore store, MazeSize size, MazeColor[] palette) : Storable(store, false), IIndexedImage
{
    public const int PaletteSize = 256;
    private const long PaletteOffset = 1024;
    private const long ArrayOffset = 4096;

    private readonly LongArray<byte> array = new(store.Offset(ArrayOffset - sizeof(long), true), size.Area, true);
    private bool written;

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

        var magic = BitConverter.IsLittleEndian ? (byte)0x49 : (byte)0x4D;
        byte[] header = [
            magic,
            magic,
            ..BitConverter.GetBytes((ushort)0x2B),
            ..BitConverter.GetBytes((ushort)0x08),
            0x00,
            0x00,
            ..BitConverter.GetBytes(16L),
            ..BitConverter.GetBytes(10L),
            ..new TiffTag(TiffTag.TagType.Width, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.Height, [(uint)size.Height]),
            ..new TiffTag(TiffTag.TagType.BitsPerSample, [0x08]),
            ..new TiffTag(TiffTag.TagType.Compression, [0x08]),
            ..new TiffTag(TiffTag.TagType.PhotometricInterpolation, [0x03]),
            ..new TiffTag(TiffTag.TagType.StripOffsets, [ArrayOffset]),
            ..new TiffTag(TiffTag.TagType.RowsPerStrip, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.StripByteCount, [(ulong)0]),
            ..new TiffTag(TiffTag.TagType.PlanarConfiguration, [0x01]),
            //XResolution
            //YResolution
            //ResolutionUnit
            ..new TiffTag(
                TiffTag.TagType.ColorMap,
                TiffTag.ValueType.Short,
                PaletteSize * 3,
                BitConverter.GetBytes(PaletteOffset)),
            ..BitConverter.GetBytes(0L)
        ];

        store.Write(0, header);
        store.Write(PaletteOffset, MemoryMarshal.AsBytes(MapPalette()));
        array.Write();

        var deflateStore = store.Offset(ArrayOffset, true);
        new StoreDeflater(deflateStore).Deflate();
        store.Write(176L, deflateStore.Length);
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
