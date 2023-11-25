using System.Buffers;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;

namespace LukeFZ.Shared;

public static class Lz4
{
    public static ReadOnlySpan<byte> DecompressFrame(ReadOnlySpan<byte> input)
    {
        return LZ4Frame.Decode(input, new ArrayBufferWriter<byte>()).WrittenSpan;
    }

    public static int Decompress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        return LZ4Codec.Decode(input, output);
    }

    public static ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> input)
    {
        var output = new byte[LZ4Codec.MaximumOutputSize(input.Length)];
        var len = LZ4Codec.Decode(input, output);
        return output.AsSpan(0, len);
    }
}