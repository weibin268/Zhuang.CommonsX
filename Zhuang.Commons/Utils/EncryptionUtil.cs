using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zhuang.Commons.Utils
{
    public static class EncryptionUtil
    {
        public static byte[] Default_IV = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08};

        public static string EncryptByMD5(string content)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(content));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();

        }

        public static string EncryptByAES(string content, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string DecryptByAES(string content, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(content);

            RijndaelManaged rDel = new RijndaelManaged();

            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        public static string EncryptByDES(string content, string key)
        {

            byte[] byKey = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] byIV = Default_IV;

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            cryptoProvider.Mode = CipherMode.ECB;
            cryptoProvider.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(content);
                sw.Flush();
                cs.FlushFinalBlock();
                sw.Flush();
                return ByteToHex(ms.ToArray());
            }
        }

        public static string DecryptByDES(string content, string key)
        {
            byte[] byKey = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] byIV = Default_IV;

            byte[] byEnc;

            try
            {
                byEnc = HexToByte(content);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            cryptoProvider.Mode = CipherMode.ECB;
            cryptoProvider.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Write))
            {
                cst.Write(byEnc,0,byEnc.Length);
                cst.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static string ByteToHex(byte[] bytes)
        {
            //return BitConverter.ToString(bytes).Replace("-", string.Empty);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static byte[] HexToByte(string str)
        {
            str = str.Replace(" ", "");

            if ((str.Length % 2) != 0)
            {
                str += " ";
            }

            byte[] returnBytes = new byte[str.Length / 2];

            for (int i = 0; i < returnBytes.Length; i++)
            {
            
                returnBytes[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }
        
    }
}
