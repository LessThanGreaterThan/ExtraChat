using ASodium;

namespace ExtraChat.Util; 

internal static class SecretBox {
    internal static byte[] Encrypt(byte[] key, byte[] bytes) {
        var nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
        var ciphertext = SodiumSecretBoxXChaCha20Poly1305.Create(bytes, nonce, key);
        return nonce.Concat(ciphertext);
    }

    internal static byte[] Decrypt(byte[] key, byte[] bytes) {
        var nonceLength = SodiumSecretBoxXChaCha20Poly1305.GetNonceBytesLength();

        var nonce = bytes[..nonceLength];
        var ciphertext = bytes[nonceLength..];

        return SodiumSecretBoxXChaCha20Poly1305.Open(ciphertext, nonce, key);
    }
}
