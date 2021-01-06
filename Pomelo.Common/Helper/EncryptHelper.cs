using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pomelo.Common.Helper
{
    public class EncryptHelper
    { 
        /// <summary>
        /// MD5+Base64加密
        /// </summary>
        /// <param name="content">加密的文本</param>
        /// <returns>返回加密的密文</returns>
        public static string MD5Base64Compress(string content)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(content);
            byte[] targetData = md5.ComputeHash(fromData);

            return Convert.ToBase64String(targetData).ToLower();
        }


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content">加密的文本</param>
        /// <returns>返回加密的密文</returns>
        public static string MD5Compress(string content)
        {
            string result = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(content))).Replace("-", "").ToLower();
            return result;
        }


        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }



        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptText(string input, string key)
        {

            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AESEncryptBytes(bytesToBeEncrypted, passwordBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }


        private static byte[] AESEncryptBytes(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            var saltBytes = new byte[9] { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

            using (var ms = new MemoryStream())
            {
                using (var AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(32);
                    AES.IV = key.GetBytes(16);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptText(string input, string key)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AESDecryptBytes(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }



        private static byte[] AESDecryptBytes(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            var saltBytes = new byte[9] { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

            using (var ms = new MemoryStream())
            {
                using (var AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(32);
                    AES.IV = key.GetBytes(16);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }


        /// <summary>
        /// 进行sha1签名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HashCode(string str)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = System.Text.UTF8Encoding.Default.GetBytes(str);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string signature = BitConverter.ToString(bytes_sha1_out);
            signature = signature.Replace("-", "").ToLower();
            return signature;
        }
    }
}
