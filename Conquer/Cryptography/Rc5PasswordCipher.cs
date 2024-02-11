using static System.Buffers.Binary.BinaryPrimitives;
using static System.Numerics.BitOperations;

namespace Conquer.Cryptography;

public class Rc5PasswordCipher : IPasswordCipher
{
    private const int WordSize = 16;
    private const int Rounds = 12;
    private const int KeySize = WordSize / 4;
    private const int SubkeySize = 2 * (Rounds + 1);
    private static readonly uint[] Key = new uint[KeySize];
    private static readonly uint[] Subkey = new uint[SubkeySize];

    static Rc5PasswordCipher()
    {
        Span<byte> seed = stackalloc byte[]
        {
            0x3C, 0xDC, 0xFE, 0xE8, 0xC4, 0x54, 0xD6, 0x7E,
            0x16, 0xA6, 0xF8, 0x1A, 0xE8, 0xD0, 0x38, 0xBE
        };

        for (var i = 0; i < KeySize; i++)
        {
            Key[i] = ReadUInt32LittleEndian(seed[(i * 4)..]);
        }

        Subkey[0] = 0xB7E15163;

        for (var i = 1; i < SubkeySize; i++)
        {
            Subkey[i] = Subkey[i - 1] - 0x61C88647;
        }

        for (uint a = 0, b = 0, i = 0, j = 0, k = 0; k < 3 * SubkeySize; k++)
        {
            a = Subkey[i] = RotateLeft(Subkey[i] + a + b, 3);
            b = Key[j] = RotateLeft(Key[j] + a + b, (int)(a + b));
            i = (i + 1) % SubkeySize;
            j = (j + 1) % KeySize;
        }
    }

    public void Encrypt(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        if (source.Length % 8 != 0)
        {
            throw new ArgumentException("Invalid block size", nameof(source));
        }

        var length = source.Length / 8;

        for (var word = 0; word < length; word++)
        {
            var a = ReadUInt32LittleEndian(source[(8 * word)..]) + Subkey[0];
            var b = ReadUInt32LittleEndian(destination[(8 * word + 4)..]) + Subkey[1];

            for (var round = 1; round <= Rounds; round++)
            {
                a = RotateLeft(a ^ b, (int)b) + Subkey[2 * round];
                b = RotateLeft(b ^ a, (int)a) + Subkey[2 * round + 1];
            }

            WriteUInt32LittleEndian(destination[(8 * word)..], a);
            WriteUInt32LittleEndian(destination[(8 * word + 4)..], b);
        }
    }

    public void Decrypt(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        if (source.Length % 8 != 0)
        {
            throw new ArgumentException("Invalid block size.", nameof(source));
        }

        var length = source.Length / 8;

        for (var word = 0; word < length; word++)
        {
            var a = ReadUInt32LittleEndian(source[(8 * word)..]);
            var b = ReadUInt32LittleEndian(source[(8 * word + 4)..]);

            for (var round = Rounds; round > 0; round--)
            {
                b = RotateRight(b - Subkey[2 * round + 1], (int)a) ^ a;
                a = RotateRight(a - Subkey[2 * round], (int)b) ^ b;
            }

            WriteUInt32LittleEndian(destination[(8 * word)..], a - Subkey[0]);
            WriteUInt32LittleEndian(destination[(8 * word + 4)..], b - Subkey[1]);
        }
    }
}