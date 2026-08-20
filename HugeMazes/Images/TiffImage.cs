using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffImage(IStore store, Guid mazeId, MazeSize size) : Storable(store, false), IImage<MazeColor>
{
    private static readonly long MazeIdOffset = Tiff.HeaderLength + Tiff.DirectoryLength(14);
    private const long MazeIdLength = 37;
    private static readonly long ArrayOffset = MazeIdOffset + MazeIdLength;

    private readonly LongArray<MazeColor> array = new(store.Offset(ArrayOffset - sizeof(long), true), size.Area, true);
    private bool written;

    public override long Extent => array.Extent + ArrayOffset;
    public Guid MazeId => mazeId;
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

    public override void Read() => throw new NotSupportedException();

    public override void Write()
    {
        if(written) return;
        written = true;

        array.Write();

        var deflatedLength = StoreDeflater.Deflate(store.Offset(ArrayOffset, true));

        var mazeIdBytes = Tiff.GetAsciiBytes(mazeId.ToString());

        store.Write<byte>(0, [
            ..Tiff.Header,
            ..Tiff.Directory([
                Tiff.Tag(Tiff.TagType.Width, (uint)size.Width),
                Tiff.Tag(Tiff.TagType.Height, (uint)size.Height),
                Tiff.Tag(Tiff.TagType.BitsPerSample, [0x08, 0x08, 0x08]),
                Tiff.Tag(Tiff.TagType.Compression, 0x08),
                Tiff.Tag(Tiff.TagType.PhotometricInterpolation, 0x02),
                Tiff.Tag(Tiff.TagType.ImageDescription, Tiff.ValueType.Ascii, mazeIdBytes.Length - 1, MazeIdOffset),
                Tiff.Tag(Tiff.TagType.StripOffsets, (ulong)ArrayOffset),
                Tiff.Tag(Tiff.TagType.SamplesPerPixel, 0x03),
                Tiff.Tag(Tiff.TagType.RowsPerStrip, (uint)size.Width),
                Tiff.Tag(Tiff.TagType.StripByteCount, (ulong)deflatedLength),
                Tiff.Tag(Tiff.TagType.MinimumSampleValue, [0, 0, 0]),
                Tiff.Tag(Tiff.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]),
                Tiff.Tag(Tiff.TagType.PlanarConfiguration, 0x01),
                Tiff.Tag(Tiff.TagType.SampleFormat, [0x01, 0x01, 0x01])
            ]),
            ..mazeIdBytes
        ]);
    }
}
