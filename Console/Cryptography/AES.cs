using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LogicBase.Cryptography {
    public class AES {
        public readonly CipherMode cipherMode;
        public AES(CipherMode mode) {
            this.cipherMode = mode;
        }
        public RijndaelManaged GetRijndaelManaged(String secretKey) {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged {
                Mode = this.cipherMode,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged) {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged) {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
        public byte[] Encrypt(byte[] plainBytes, string key) {
            var rijndaelManaged = GetRijndaelManaged(key);
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public byte[] Decrypt(byte[] encryptedData, string key) {
            var rijndaelManaged = GetRijndaelManaged(key);
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
        public string Encrypt(string plainText, string key) {
            try {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
            } catch {
                return null;
            }
        }
        

        public string Decrypt(string encryptedText, string key) {
            try {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
            } catch {
                return null;
            }
        }
    }
}
