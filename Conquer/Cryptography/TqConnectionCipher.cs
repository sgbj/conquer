using static System.Buffers.Binary.BinaryPrimitives;

namespace Conquer.Cryptography;

public class TqConnectionCipher : IConnectionCipher
{
    private static readonly byte[] Key = new byte[0x200];
    private ushort _encryptCount, _decryptCount;
    private byte[] _key = Key;

    static TqConnectionCipher()
    {
        Span<byte> seed = stackalloc byte[] { 0x9D, 0x0F, 0xFA, 0x13, 0x62, 0x79, 0x5C, 0x6D };

        for (var i = 0; i < 0x100; i++)
        {
            Key[i] = seed[0];
            Key[i + 0x100] = seed[4];
            seed[0] = (byte)((seed[1] + seed[0] * seed[2]) * seed[0] + seed[3]);
            seed[4] = (byte)((seed[5] - seed[4] * seed[6]) * seed[4] + seed[7]);
        }
    }

    public void Encrypt(ReadOnlySpan<byte> source, Span<byte> destination) =>
        Transform(source, destination, Key, ref _encryptCount);

    public void Decrypt(ReadOnlySpan<byte> source, Span<byte> destination) =>
        Transform(source, destination, _key, ref _decryptCount);

    public void SetKeys(uint id, uint token)
    {
        var seed = (token + id) ^ 0x4321 ^ token;

        Span<byte> temp1 = stackalloc byte[4];
        WriteUInt32LittleEndian(temp1, seed);
        Span<byte> temp2 = stackalloc byte[4];
        WriteUInt32LittleEndian(temp2, seed * seed);

        _key = new byte[0x200];

        for (var i = 0; i < 0x100; i++)
        {
            _key[i] = (byte)(temp1[i % 4] ^ Key[i]);
            _key[i + 0x100] = (byte)(temp2[i % 4] ^ Key[i + 0x100]);
        }
    }

    private static void Transform(ReadOnlySpan<byte> source, Span<byte> destination, ReadOnlySpan<byte> key,
        ref ushort count)
    {
        for (var i = 0; i < source.Length; i++)
        {
            destination[i] = (byte)(source[i] ^ 0xAB);
            destination[i] = (byte)((destination[i] << 4) | (destination[i] >> 4));
            destination[i] = (byte)(key[((count >> 8) & 0xFF) + 0x100] ^ destination[i]);
            destination[i] = (byte)(key[count & 0xFF] ^ destination[i]);
            count++;
        }
    }
}