using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class SMTP
    {        
        private XmlDocument doc;
        private XmlElement el;
        public SMTP(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/SMTP") == null) Initialize();
            if (doc.SelectSingleNode("/hapConfig").Attributes["salt"] != null) _salt = Encoding.ASCII.GetBytes(doc.SelectSingleNode("/hapConfig").Attributes["salt"].Value);
            if (doc.SelectSingleNode("/hapConfig").Attributes["key"] != null) _key = doc.SelectSingleNode("/hapConfig").Attributes["key"].Value;
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/SMTP");
        }
        public void Initialize()
        {
            XmlElement e = doc.CreateElement("SMTP");
            e.SetAttribute("server", "");
            e.SetAttribute("port", "25");
            e.SetAttribute("enabled", "False");
            e.SetAttribute("ssl", "False");
            e.SetAttribute("from", "admin");
            e.SetAttribute("fromaddress", "admin@localhost.com");
            e.SetAttribute("tls", "False");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
        public string Server
        {
            get { return el.GetAttribute("server"); }
            set { el.SetAttribute("server", value); }
        }
        public int Port
        {
            get { return int.Parse(el.GetAttribute("port")); }
            set { el.SetAttribute("port", value.ToString()); }
        }
        public bool Enabled
        {
            get { return bool.Parse(el.GetAttribute("enabled")); }
            set { el.SetAttribute("enabled", value.ToString()); }
        }
        public bool SSL
        {
            get { return bool.Parse(el.GetAttribute("ssl")); }
            set { el.SetAttribute("ssl", value.ToString()); }
        }
        public bool TLS
        {
            get { return bool.Parse(el.GetAttribute("tls")); }
            set { el.SetAttribute("tls", value.ToString()); }
        }
        public string User
        {
            get { return el.GetAttribute("user"); }
            set { el.SetAttribute("user", value); }
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
        public string FromUser
        {
            get { return el.GetAttribute("from"); }
            set { el.SetAttribute("from", value); }
        }
        public string FromEmail
        {
            get { return el.GetAttribute("fromaddress"); }
            set { el.SetAttribute("fromaddress", value); }
        }
        public string Exchange
        {
            get { if (el.GetAttribute("exchange") == null) return ""; return el.GetAttribute("exchange"); }
            set { el.SetAttribute("exchange", value); }
        }
        static byte[] _salt = Encoding.ASCII.GetBytes("zsKrYE328e");
        static byte[] _salt2 = Encoding.ASCII.GetBytes("ys8cQPOvF7");
        static string _key = "0En9wSkynoVZBdAxswdg" + HttpContext.Current.Server.MachineName + "3iob91Rwzz8IzTvfBreW";
        public string ImpersonationUser
        {
            get { return el.GetAttribute("impersonationuser"); }
            set { el.SetAttribute("impersonationuser", value); }
        }
        public string ImpersonationPassword
        {
            get 
            {
                if (string.IsNullOrEmpty(el.GetAttribute("impersonationpassword"))) return "";
                RijndaelManaged aesAlg = null;
                string plaintext = null;

                try
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_key, _salt2);

                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    byte[] bytes = Convert.FromBase64String(el.GetAttribute("impersonationpassword"));
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
                if (string.IsNullOrEmpty(value)) el.SetAttribute("impersonationpassword", "");
                RijndaelManaged aesAlg = null;
                try
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(_key, _salt2);
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
                el.SetAttribute("impersonationpassword", outStr);
            }
        }
        public string ImpersonationDomain
        {
            get { return el.GetAttribute("impersonationdomain"); }
            set { el.SetAttribute("impersonationdomain", value); }
        }
        public bool EWSUseEmailoverAN
        {
            get { if (el.Attributes["ewsuseemailoveran"] != null) return bool.Parse(el.GetAttribute("ewsuseemailoveran")); return false; }
        }
    }
}
