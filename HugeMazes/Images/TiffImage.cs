using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffImage(IStore store, MazeSize size) : Storable(store, false), IImage<MazeColor>
{
    private const long ArrayOffset = 4096;

    private readonly LongArray<MazeColor> array = new(store.Offset(ArrayOffset - sizeof(long), true), size.Area, true);
    private bool written;

    public override long Extent => array.Extent + ArrayOffset;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public MazeColor this[int x, int y]
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
            ..BitConverter.GetBytes(13L),
            ..new TiffTag(TiffTag.TagType.Width, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.Height, [(uint)size.Height]),
            ..new TiffTag(TiffTag.TagType.BitsPerSample, [0x08, 0x08, 0x08]),
            ..new TiffTag(TiffTag.TagType.Compression, [0x08]),
            ..new TiffTag(TiffTag.TagType.PhotometricInterpolation, [0x02]),
            ..new TiffTag(TiffTag.TagType.StripOffsets, [ArrayOffset]),
            ..new TiffTag(TiffTag.TagType.SamplesPerPixel, [0x03]),
            ..new TiffTag(TiffTag.TagType.RowsPerStrip, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.StripByteCount, [0L]),
            ..new TiffTag(TiffTag.TagType.MinimumSampleValue, [0, 0, 0]),
            ..new TiffTag(TiffTag.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]),
            ..new TiffTag(TiffTag.TagType.PlanarConfiguration, [0x01]),
            ..new TiffTag(TiffTag.TagType.SampleFormat, [0x01, 0x01, 0x01]),
            ..BitConverter.GetBytes(0L)
        ];

        store.Write(0, header);
        array.Write();

        var deflateStore = store.Offset(ArrayOffset, true);
        StoreDeflater.Deflate(deflateStore);
        store.Write(196L, deflateStore.Length);
    }
}
