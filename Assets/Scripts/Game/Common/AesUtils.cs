using System.IO;
using System.Security.Cryptography;

public static class AesUtils
{
    static Aes mAes;

    static AesUtils()
    {
        mAes = Aes.Create();
    }

    public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
    {
        byte[] encrypt_data = null;
        using (var stream = new MemoryStream())
        {
            Encrypt(stream, data, key, iv);
            encrypt_data = stream.ToArray();
        }
        return encrypt_data;
    }

    public static void Encrypt(Stream output, byte[] data, byte[] key, byte[] iv)
    {
        mAes.Key = key;
        mAes.IV = iv;
        using (var crypto_stream = new CryptoStream(output, mAes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
        {
            using (var writer = new BinaryWriter(crypto_stream))
            {
                writer.Write(data);
            }
        }
    }

    public static byte[] Decrypt(byte[] data, int length, byte[] key, byte[] iv)
    {
        byte[] decrypt_data = null;
        if (length > 0)
        {
            using (var encrypt_stream = new MemoryStream(data))
            {
                using (var crypto_stream = new CryptoStream(encrypt_stream, mAes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    using (var br = new BinaryReader(crypto_stream))
                    {
                        decrypt_data = br.ReadBytes(length);
                    }
                }
            }
        }
        return decrypt_data;
    }
}