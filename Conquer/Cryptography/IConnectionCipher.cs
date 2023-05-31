namespace Conquer.Cryptography;

public interface IConnectionCipher
{
    void Encrypt(ReadOnlySpan<byte> source, Span<byte> destination);
    void Decrypt(ReadOnlySpan<byte> source, Span<byte> destination);
}