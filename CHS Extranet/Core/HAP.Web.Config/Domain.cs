using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Domain
    {
        XmlElement el;
        public Domain(XmlNode node)
        {
            el = node as XmlElement;
        }

        public string UPN
        {
            get { return el.GetAttribute("upn"); }
            set { el.SetAttribute("upn", value); }
        }

        public string Domain
        {
            get { return el.GetAttribute("domain"); }
            set { el.SetAttribute("domain", value); }
        }

        public string StudentsGroup
        {
            get { return el.GetAttribute("studentsgroup"); }
            set { el.SetAttribute("studentsgroup", value); }
        }

        public string User
        {
            get { return el.GetAttribute("username"); }
            set { el.SetAttribute("username", value); }
        }
        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(el.GetAttribute("password"))) return "";
                RijndaelManaged aesAlg = null;
                string plaintext = null;

                try
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_key, _salt);

                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    byte[] bytes = Convert.FromBase64String(el.GetAttribute("password"));
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
                if (string.IsNullOrEmpty(value)) el.SetAttribute("password", "");
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
                el.SetAttribute("password", outStr);
            }
        }
        private byte[] _salt = Encoding.ASCII.GetBytes("zQuPbTaqzK");
        private string _key = "mKnooh8A8VbhbHngxKRu";
    }
}
