using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ExtensionsPack.Core
{
    public static class EncryptionExtensions
    {
        private const int DerivationIterations = 1000;

        private const int SaltBytes = 32;
        private const int IvBytes = 16;

        /// <summary>Decrypts cipher string</summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="passPhrase">Pass phrase</param>
        /// <returns>Plain text</returns>
        public static string Decrypt(string cipherText, string passPhrase)
        {
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(SaltBytes).ToArray();
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(SaltBytes).Take(IvBytes).ToArray();
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(SaltBytes + IvBytes).Take(cipherTextBytesWithSaltAndIv.Length - (SaltBytes + IvBytes)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                byte[] keyBytes = password.GetBytes(SaltBytes);

                using (var symmetricKey = new AesCryptoServiceProvider())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        var plainTextBytes = new byte[cipherTextBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    }
                }
            }
        }

        /// <summary>Encrypts plain text</summary>
        /// <param name="plainText">The plain text to encrypt</param>
        /// <param name="passPhrase">The pass phrase</param>
        /// <returns>Encrypted text</returns>
        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            byte[] saltStringBytes = GenerateBitsOfRandomEntropy(32);
            byte[] ivStringBytes = GenerateBitsOfRandomEntropy(16);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                byte[] keyBytes = password.GetBytes(SaltBytes);
                using (var symmetricKey = new AesCryptoServiceProvider())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                byte[] cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Generate bits of random entropy.</summary>
        /// <returns></returns>
        private static byte[] GenerateBitsOfRandomEntropy(int num)
        {
            var randomBytes = new byte[num]; // 32 Bytes will give us 256 bits.

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}