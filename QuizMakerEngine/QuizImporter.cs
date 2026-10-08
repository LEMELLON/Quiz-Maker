using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace QuizMakerEngine.Services;

public static class QuizImporter
{
    public static string DecodeQuizCode(string combinedCode)
    {
        var parts = combinedCode.Split('.');
        if (parts.Length != 2) throw new Exception("Invalid Quiz Code format.");

        // Decode the Base64URL key and payload
        string secretKey = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        byte[] combinedBytes = Base64UrlDecode(parts[1]);

        // Extract byte components based on exact lengths used during export
        int saltLen = 16;
        int ivLen = 12;
        int tagLen = 16;
        int cipherLen = combinedBytes.Length - saltLen - ivLen - tagLen;

        byte[] salt = new byte[saltLen];
        byte[] iv = new byte[ivLen];
        byte[] cipherText = new byte[cipherLen];
        byte[] tag = new byte[tagLen];

        Buffer.BlockCopy(combinedBytes, 0, salt, 0, saltLen);
        Buffer.BlockCopy(combinedBytes, saltLen, iv, 0, ivLen);
        Buffer.BlockCopy(combinedBytes, saltLen + ivLen, cipherText, 0, cipherLen);
        Buffer.BlockCopy(combinedBytes, saltLen + ivLen + cipherLen, tag, 0, tagLen);

        // 1. Derive the AES key using PBKDF2 (150,000 iterations)
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(secretKey, salt, 150000, HashAlgorithmName.SHA256, 32);

        // 2. Decrypt the payload
        byte[] compressed = new byte[cipherLen];
        using (var aesGcm = new AesGcm(key, tagLen))
        {
            aesGcm.Decrypt(iv, cipherText, tag, compressed);
        }

        // 3. Decompress the GZip byte stream back into JSON
        using var ms = new MemoryStream(compressed);
        using var gzip = new GZipStream(ms, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);

        return reader.ReadToEnd();
    }

    private static byte[] Base64UrlDecode(string base64Url)
    {
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}