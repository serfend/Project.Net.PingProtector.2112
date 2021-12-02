using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.FileHelper
{
    public class CiperFile
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string? Path { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public byte[] Key { get; set; } = new byte[] { 0xde, 0xad, 0xbe, 0xef, 0x00, 0x01, 0x02, 0x03, 0xbe, 0xef, 0x00, 0x01, 0x01, 0x02, 0x03, 0xbe };
        public byte[] IV { get; set; } = new byte[] { 0xde, 0xad, 0xbe, 0xef, 0x00, 0x01, 0x02, 0x03, 0xbe, 0xef, 0x00, 0x01, 0x01, 0x02, 0x03, 0xbe };
        public string? Content { set; get; }

        public void Load()
        {
            if (!File.Exists(Path)) return;
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform encryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using var fs = new FileStream(Path, FileMode.Open);
                    using var cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Read);
                    using var sr = new StreamReader(cs);
                    Content = sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }
        }
        public void Save()
        {
            if (!File.Exists(Path)) File.CreateText(Path);
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using var fs = new FileStream(Path, FileMode.Open);
                    using var cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write);
                    using var sw = new StreamWriter(cs);
                    sw.Write(Content);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
