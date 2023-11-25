using System.Text;

namespace LukeFZ.Shared;

public static class Base32
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string FromBytes(ReadOnlySpan<byte> bytes)
    {
        var output = new StringBuilder();
        for (int bitIndex = 0; bitIndex < bytes.Length * 8; bitIndex += 5)
        {
            var dualbyte = bytes[bitIndex / 8] << 8;
            if (bitIndex / 8 + 1 < bytes.Length)
                dualbyte |= bytes[bitIndex / 8 + 1];
            dualbyte = 0x1f & (dualbyte >> (16 - bitIndex % 8 - 5));
            output.Append(Alphabet[dualbyte]);
        }

        return output.ToString();
    }

    public static byte[] Base32ToBytes(ReadOnlySpan<char> base32)
    {
        using var ms = new MemoryStream();

        for (int bitIndex = 0; bitIndex / 5 + 1 < base32.Length; bitIndex += 8)
        {
            var dualbyte = Alphabet.IndexOf(base32[bitIndex / 5]) << 10;

            if (bitIndex / 5 + 1 < base32.Length)
                dualbyte |= Alphabet.IndexOf(base32[bitIndex / 5 + 1]) << 5;

            if (bitIndex / 5 + 2 < base32.Length)
                dualbyte |= Alphabet.IndexOf(base32[bitIndex / 5 + 2]);

            dualbyte = 0xff & (dualbyte >> (15 - bitIndex % 5 - 8));
            ms.WriteByte((byte)dualbyte);
        }

        return ms.ToArray();
    }
}