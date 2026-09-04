using System.Runtime.InteropServices;
using System.Text;
using HugeMazes.Extensions;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public static class Tiff
{
    private static readonly byte Magic = BitConverter.IsLittleEndian ? (byte)0x49 : (byte)0x4D;

    public static byte[] Header => [
        Magic,
        Magic,
        ..BitConverter.GetBytes((ushort)0x2B),
        ..BitConverter.GetBytes((ushort)0x08),
        0x00,
        0x00,
        ..BitConverter.GetBytes(16L)];

    public static readonly long HeaderLength = 16;

    public static byte[] Directory(byte[][] entries) => [
        ..BitConverter.GetBytes((ulong)entries.Length),
        ..entries.SelectMany(e => e),
        ..BitConverter.GetBytes(0L)];

    public static long DirectoryLength(int entryCount) => (entryCount * 20) + 16;

    public static byte[] Tag(TagType type, uint value) => Tag(type, [value]);

    public static byte[] Tag(TagType type, uint[] values) => GetBytes(
        type,
        ValueType.Int,
        values.Length,
        [..MemoryMarshal.AsBytes(values)]);

    public static byte[] Tag(TagType type, ulong value) => Tag(type, [value]);

    public static byte[] Tag(TagType type, ulong[] values) => GetBytes(
        type,
        ValueType.Long,
        values.Length,
        [..MemoryMarshal.AsBytes(values)]);

    public static byte[] Tag(TagType type, ushort value) => Tag(type, [value]);

    public static byte[] Tag(TagType type, ushort[] values) => GetBytes(
        type,
        ValueType.Short,
        values.Length,
        [..MemoryMarshal.AsBytes(values)]);

    public static byte[] Tag(TagType type, string value) => GetBytes(
        type,
        ValueType.Ascii,
        Encoding.ASCII.GetByteCount(value) + 1,
        [..Encoding.ASCII.GetBytes(value), 0x00]);

    public static byte[] Tag(TagType type, ValueType valueType, long count, long offset) => GetBytes(
        type,
        valueType,
        count,
        BitConverter.GetBytes(offset));

    private static byte[] GetBytes(TagType type, ValueType valueType, long length, byte[] value) => [
        ..BitConverter.GetBytes((ushort)type),
        ..BitConverter.GetBytes((ushort)valueType),
        ..BitConverter.GetBytes((ulong)length),
        ..value.Extend(8)
    ];

    public static byte[] GetAsciiBytes(string value) => [..Encoding.ASCII.GetBytes(value), 0x00];

    public static byte[] GetColorMapBytes(MazeColor[] palette) => [..MemoryMarshal.AsBytes(GetColorMap(palette))];

    private static ushort[] GetColorMap(MazeColor[] palette)
    {
        var reds = new ushort[palette.Length];
        var greens = new ushort[palette.Length];
        var blues = new ushort[palette.Length];

        for(var i = 0; i < palette.Length; i++)
        {
            reds[i] = Scale(palette[i].R);
            greens[i] = Scale(palette[i].G);
            blues[i] = Scale(palette[i].B);
        }

        return [..reds, ..greens, ..blues];
    }

    private static ushort Scale(byte value) => (ushort)(value * ushort.MaxValue / (double)byte.MaxValue);

    public enum TagType : ushort
    {
        Width = 0x100,
        Height = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotometricInterpolation = 0x106,
        ImageDescription = 0x10E,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripByteCount = 0x117,
        MinimumSampleValue = 0x118,
        MaximumSampleValue = 0x119,
        PlanarConfiguration = 0x11C,
        Software = 0x131,
        ColorMap = 0x140,
        SampleFormat = 0x153
    }

    public enum ValueType : ushort
    {
        Ascii = 0x02,
        Short = 0x03,
        Int = 0x04,
        Long = 0x10
    }
}
