using System.Runtime.InteropServices;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public struct TiffTag(TiffTag.TagType type, TiffTag.ValueType valueType, long length, byte[] value)
{
    public TiffTag(TagType type, uint[] values) : this(
        type,
        ValueType.Int,
        values.Length,
        [.. MemoryMarshal.AsBytes(values)])
    {
    }

    public TiffTag(TagType type, ulong[] values) : this(
        type,
        ValueType.Long,
        values.Length,
        [.. MemoryMarshal.AsBytes(values)])
    {
    }

    public TiffTag(TagType type, ushort[] values) : this(
        type,
        ValueType.Short,
        values.Length,
        [.. MemoryMarshal.AsBytes(values)])
    {
    }

    public readonly byte[] Bytes => [
        ..BitConverter.GetBytes((ushort)type),
            ..BitConverter.GetBytes((ushort)valueType),
            ..BitConverter.GetBytes((ulong)length),
            ..value,
            ..new byte[8 - value.Length]
    ];

    public readonly IEnumerator<byte> GetEnumerator() => Bytes.AsEnumerable().GetEnumerator();

    public enum TagType : ushort
    {
        Width = 0x100,
        Height = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotometricInterpolation = 0x106,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripByteCount = 0x117,
        MinimumSampleValue = 0x118,
        MaximumSampleValue = 0x119,
        PlanarConfiguration = 0x11C,
        ColorMap = 0x140,
        SampleFormat = 0x153
    }

    public enum ValueType : ushort
    {
        Short = 0x03,
        Int = 0x04,
        Long = 0x10
    }
}