using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using QuizMakerEngine.Models;

namespace QuizMakerEngine.Services;

public static class QuizExporter
{
    private const int Iterations = 150000;
    private const int SaltLen = 16;
    private const int IvLen = 12;

    public static string ExportToQuizCode(List<QuestionCollection> questions, string quizTitle, string secretKey)
    {
        var quizPayload = new
        {
            title = quizTitle,
            formatVersion = 2,
            questions = questions
        };

        byte[] plainBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(quizPayload));

        // 1. GZip Compression
        byte[] compressed;
        using (var ms = new MemoryStream())
        {
            using (var gzip = new GZipStream(ms, CompressionLevel.Optimal))
            {
                gzip.Write(plainBytes, 0, plainBytes.Length);
            }
            compressed = ms.ToArray();
        }

        // 2. Key Derivation (PBKDF2)
        byte[] salt = RandomNumberGenerator.GetBytes(SaltLen);
        byte[] iv = RandomNumberGenerator.GetBytes(IvLen);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(secretKey, salt, Iterations, HashAlgorithmName.SHA256, 32);

        // 3. AES-256-GCM Encryption
        byte[] cipherText = new byte[compressed.Length];
        byte[] tag = new byte[16];

        using (var aesGcm = new AesGcm(key, 16))
        {
            aesGcm.Encrypt(iv, compressed, cipherText, tag);
        }

        // 4. Combine Salt + IV + CipherText + Tag
        byte[] combined = new byte[SaltLen + IvLen + cipherText.Length + tag.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, SaltLen);
        Buffer.BlockCopy(iv, 0, combined, SaltLen, IvLen);
        Buffer.BlockCopy(cipherText, 0, combined, SaltLen + IvLen, cipherText.Length);
        Buffer.BlockCopy(tag, 0, combined, SaltLen + IvLen + cipherText.Length, tag.Length);

        string codePart = Convert.ToBase64String(combined).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        string keyPart = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey)).Replace("+", "-").Replace("/", "_").TrimEnd('=');

        return $"{keyPart}.{codePart}";
    }
}