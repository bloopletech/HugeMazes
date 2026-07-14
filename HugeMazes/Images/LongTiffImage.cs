using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class LongTiffImage(IStore store, MazeSize size, bool leaveOpen = false) : Storable(store, leaveOpen), IImage<MazeColor>
{
    private const long HeaderSize = 312;

    private readonly LongArray<MazeColor> array = new(store.Offset(HeaderSize, true), size.Area, true);

    public override long Extent => array.Extent + HeaderSize;
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
            ..new TiffTag(TiffTag.TagType.PhotometricInterpolation, [0x02]),
            ..new TiffTag(TiffTag.TagType.StripOffsets, [(ulong)HeaderSize + sizeof(long)]),
            ..new TiffTag(TiffTag.TagType.Orientation, [0x01]),
            ..new TiffTag(TiffTag.TagType.SamplesPerPixel, [0x03]),
            ..new TiffTag(TiffTag.TagType.RowsPerStrip, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.StripByteCount, [(ulong)size.Area * 3]),
            ..new TiffTag(TiffTag.TagType.MinimumSampleValue, [0, 0, 0]),
            ..new TiffTag(TiffTag.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]),
            ..new TiffTag(TiffTag.TagType.PlanarConfiguration, [0x01]),
            ..new TiffTag(TiffTag.TagType.SampleFormat, [0x01, 0x01, 0x01]),
            ..BitConverter.GetBytes(0L)
        ];

        store.Write(0, header);
        array.Write();
    }
}
