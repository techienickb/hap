using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace HAP.Web.Configuration
{
    public class AD
    {
        private XmlElement el;
        private XmlDocument doc;
        public AD(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/AD") == null) Initialize();
            if (doc.SelectSingleNode("/hapConfig").Attributes["salt"] != null) _salt = Encoding.ASCII.GetBytes(doc.SelectSingleNode("/hapConfig").Attributes["salt"].Value);
            if (doc.SelectSingleNode("/hapConfig").Attributes["key"] != null) _key = doc.SelectSingleNode("/hapConfig").Attributes["key"].Value;
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/AD");
        }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("AD");
            e.SetAttribute("username", "");
            e.SetAttribute("password", "");
            e.SetAttribute("upn", "");
            e.SetAttribute("studentsgroup", "");
            e.SetAttribute("usenestedlookups", "True");
            e.SetAttribute("secureldap", "False");
            e.SetAttribute("maxlogonattempts", "4");
            e.SetAttribute("maxrecursions", "10");
            e.SetAttribute("allow1usecodes", "False");
            e.SetAttribute("AuthMode", "Forms");
            XmlElement ous = doc.CreateElement("OUs");
            e.AppendChild(ous);
            XmlElement ips = doc.CreateElement("InternalIPs");
            e.AppendChild(ips);
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }

        public ous OUs
        {
            get
            {
                return new ous(ref doc);
            }
        }

        public string UPN
        {
            get { return el.GetAttribute("upn"); }
            set { el.SetAttribute("upn", value); }
        }
        public bool AllowOneUseCodes
        {
            get { return bool.Parse(el.GetAttribute("allow1usecodes")); }
            set { el.SetAttribute("allow1usecodes", value.ToString()); }
        }
        public bool SecureLDAP
        {
            get { return bool.Parse(el.GetAttribute("secureldap")); }
            set { el.SetAttribute("secureldap", value.ToString()); }
        }
        public bool UseNestedLookups
        {
            get { return bool.Parse(el.GetAttribute("usenestedlookups")); }
            set { el.SetAttribute("usenestedlookups", value.ToString()); }
        }
        public int MaxLogonAttemps
        {
            get { return int.Parse(el.GetAttribute("maxlogonattempts")); }
            set { el.SetAttribute("maxlogonattempts", value.ToString()); }
        }
        public int MaxRecursions
        {
            get { return int.Parse(el.GetAttribute("maxrecursions")); }
            set { el.SetAttribute("maxrecursions", value.ToString()); }
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
        public AuthMode AuthenticationMode
        {
            get
            {
                if (el.HasAttribute("AuthMode")) return (AuthMode)Enum.Parse(typeof(AuthMode), el.GetAttribute("AuthMode"));
                return AuthMode.Forms;
            }
        }
        public string[] InternalIP
        {
            get
            {
                List<string> s = new List<string>();
                XmlNode node = doc.SelectSingleNode("/hapConfig/AD/InternalIPs");

                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Attributes["ip"] != null)
                    {
                        s.Add(n.Attributes["ip"].Value.ToString());
                    }
                }
                return s.ToArray();
            }
        }
    }
    public enum AuthMode { Forms, Windows }
}
