using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using HAP.Web.Configuration;

namespace HAP.AD
{
    public class TokenGenerator
    {
        static byte[] _salt = Encoding.ASCII.GetBytes("yV9vL9Wbkh");
        static string _key = "5ajX29BJfPM38" + HttpContext.Current.Server.MachineName + "xdv9DCsgWTn";

        public static string ConvertToPlain(string token)
        {
            if (!string.IsNullOrEmpty(hapConfig.Current.Key)) _key = hapConfig.Current.Key;
            if (!string.IsNullOrEmpty(hapConfig.Current.Salt)) _salt = Encoding.ASCII.GetBytes(hapConfig.Current.Salt);
            RijndaelManaged aesAlg = null;
            string plaintext = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_key, _salt);

                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);         
                byte[] bytes = Convert.FromBase64String(token);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (aesAlg != null) aesAlg.Clear();
            }

            return plaintext;
        }

        public static string ConvertToToken(string value)
        {
            if (!string.IsNullOrEmpty(hapConfig.Current.Key)) _key = hapConfig.Current.Key;
            if (!string.IsNullOrEmpty(hapConfig.Current.Salt)) _salt = Encoding.ASCII.GetBytes(hapConfig.Current.Salt);
            string outStr = "";
            RijndaelManaged aesAlg = null;
            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_key, _salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(value);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null) aesAlg.Clear();
            }
            return outStr;
        }
    }
}
