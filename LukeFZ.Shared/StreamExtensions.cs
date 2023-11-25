using System.Diagnostics;

namespace LukeFZ.Shared;

public static class StreamExtensions
{
    public static bool CheckedRead(this Stream stream, Span<byte> data)
    {
        Debug.Assert(stream.CanRead);

        var read = stream.Read(data);
        Debug.Assert(read == data.Length);

        return read == data.Length;
    }

    public static bool CheckedRead(this Stream stream, byte[] data, int offset, int count)
    {
        Debug.Assert(stream.CanRead);

        var read = stream.Read(data, offset, count);
        Debug.Assert(read == count);

        return read == count;
    }
}