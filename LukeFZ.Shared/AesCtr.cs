using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;

namespace LukeFZ.Shared;

public static class AesCtr
{
    // Does it like mbedTLS
    public static void Encrypt(Span<byte> input, byte[] key, byte[] iv)
        => Crypt(input, key, iv);

    public static void Decrypt(Span<byte> input, byte[] key, byte[] iv)
        => Crypt(input, key, iv);

    private static void Crypt(Span<byte> data, byte[] key, byte[] iv)
    {
        const int blockSize = 16;

        Debug.Assert(key.Length == blockSize);
        Debug.Assert(iv.Length == blockSize);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        aes.Key = key;

        using var encryptor = aes.CreateEncryptor();

        var remaining = data.Length;
        var offset = 0;

        var nonce = ArrayPool<byte>.Shared.Rent(blockSize);
        Buffer.BlockCopy(iv, 0, nonce, 0, iv.Length);

        var block = ArrayPool<byte>.Shared.Rent(blockSize);
        while (remaining > 0)
        {
            encryptor.TransformBlock(nonce, 0, blockSize, block, 0);

            var current = Math.Min(remaining, blockSize);
            for (int i = 0; i < current; i++)
            {
                data[offset + i] ^= block[i];
            }

            for (int i = blockSize; i > 0; i--)
            {
                if (++nonce[i - 1] != 0)
                    break;
            }

            remaining -= current;
            offset += current;
        }

        ArrayPool<byte>.Shared.Return(nonce);
        ArrayPool<byte>.Shared.Return(block);
    }
}