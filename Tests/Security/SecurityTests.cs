using System;
using NUnit.Framework;
using NetCore.ExtensionPack.Core;

namespace SecurityTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WhenEncryptAndDecrypt_WithPassPhrase_TextsMatch()
        {
            var passPhrase1 = "aaaaaaaaaaaaa";
            var passPhrase2 = "bbbbbbbbbbbbb";
            var plainText = "hello world hello world hello world";

            var encrypted = EncryptionExtensions.Encrypt(plainText, passPhrase1);
            var decrypted1 = EncryptionExtensions.Decrypt(encrypted, passPhrase1);

            Assert.That(encrypted, Is.Not.EqualTo(plainText));
            Assert.That(decrypted1, Is.EqualTo(plainText));
            Assert.That(() => EncryptionExtensions.Decrypt(encrypted, passPhrase2), Throws.Exception);
        }
    }
}