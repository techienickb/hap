using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HAP.Win.DirectEdit
{
    public class Decrypt
    {
        static byte[] _salt = Encoding.ASCII.GetBytes("S=u=i0Z/;nzXX=lg");
        static string _key = "jQ1gbMOoyKr4ic?!wTY1NVhoSv8JS/..Q,d-K*b5zC9RP2M8Vw9uepT=V-Cz+zxC";

        public static string ConvertToPlain(string token)
        {
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

    }
}
