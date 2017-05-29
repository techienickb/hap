using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;
using System.Web;

namespace HAP.Web.Configuration
{
    public class adDomain
    {
        public adDomain(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            Username = node.Attributes["username"].Value;
            _Password = node.Attributes["password"].Value;
        }
        public adDomain() { }

        public string Name { get; set; }
        public string Username { get; set; }
        private string _Password;

        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_Password)) return "";
                RijndaelManaged aesAlg = null;
                string plaintext = null;

                try
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_key, _salt);

                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    byte[] bytes = Convert.FromBase64String(_Password);
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
            set
            {
                string outStr = "";
                if (string.IsNullOrEmpty(value)) _Password = "";
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
                _Password =  outStr;
            }
        }
        private byte[] _salt = Encoding.ASCII.GetBytes("zQuPb" + HttpContext.Current.Server.MachineName + "TaqzK");
        private string _key = "mKnooh8A8VbhbHngxKRu";
    }
}
