using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Box.Common
{
    public  class CryptUtil
    {
        public string key;
        public string iv;

        public CryptUtil(string key, string iv) {
            this.key = key;
            this.iv = iv;
        }

        public CryptUtil(Box.Common.BoxSettings boxSettings) {
            this.key = boxSettings.ENCRYPT_KEY;
            this.iv = boxSettings.ENCRYPT_IV;            
        }

        private SymmetricAlgorithm GetAlgorithm()
        {
            SymmetricAlgorithm myRijndael = new RijndaelManaged();
            //myRijndael.Padding = PaddingMode.PKCS7;
            //myRijndael.Mode = CipherMode.CBC;
            //myRijndael.KeySize = 256;
            //myRijndael.BlockSize = 256;
            myRijndael.Key = Convert.FromBase64String(key);
            myRijndael.IV = Convert.FromBase64String(iv);

            return myRijndael;
        }

        public byte[] EncryptBytes(byte[] file)
        {
            SymmetricAlgorithm alg = GetAlgorithm();
            byte[] encrypted;

            using (var stream = new MemoryStream())
            using (var encrypt = new CryptoStream(stream, alg.CreateEncryptor(alg.Key, alg.IV), CryptoStreamMode.Write))
            {
                encrypt.Write(file, 0, file.Length);
                encrypt.FlushFinalBlock();
                encrypted = stream.ToArray();
            }

            alg.Clear();
            return encrypted;
        }

        public byte[] DecryptBytes(byte[] file)
        {
            SymmetricAlgorithm alg = GetAlgorithm();
            byte[] decrypted;

            using (var stream = new MemoryStream())
            using (var encrypt = new CryptoStream(stream, alg.CreateDecryptor(alg.Key, alg.IV), CryptoStreamMode.Write))
            {
                encrypt.Write(file, 0, file.Length);
                encrypt.FlushFinalBlock();
                decrypted = stream.ToArray();
            }
            alg.Clear();
            return decrypted;
        }
    }
}
