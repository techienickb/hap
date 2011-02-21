using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using HAP.Web.Configuration;
using System.Configuration;

namespace HAP.Web.UserCard
{
    public class Sys
    {
        public static XmlDocument Doc
        {
            get
            {
                XmlDocument doc;
                if (HttpContext.Current.Cache["usercard"] == null)
                {
                    doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/UserCards.xml"));

                    HttpContext.Current.Cache.Insert("usercard", doc, new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/App_Data/UserCards.xml")));

                }
                else doc = HttpContext.Current.Cache["usercard"] as XmlDocument;
                return doc;
            }
            set
            {
                XmlWriterSettings set = new XmlWriterSettings();
                set.Indent = true;
                set.IndentChars = "   ";
                set.Encoding = System.Text.Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/UserCards.xml"), set);
                value.Save(writer);
                writer.Flush();
                writer.Close();
                HttpContext.Current.Cache.Remove("usercard");
            }
        }
        public Department[] getDepartments()
        {
            List<Department> deps = new List<Department>();
            foreach (XmlNode node in Doc.SelectNodes("/usercards/departments/department"))
                deps.Add(new Department(node.Attributes[0].Value));
            return deps.ToArray();
        }

        public void addDepartment(string name)
        {
            XmlElement e = Doc.CreateElement("department");
            e.SetAttribute("name", name);
            XmlDocument d = Doc;
            d.SelectSingleNode("/usercards/departments").AppendChild(e);
            Doc = d;
        }

        public void removeDepartment(string name)
        {
            XmlDocument doc = Doc;

            XmlNode node = doc.SelectSingleNode("/usercards/departments/department[@name='" + name + "']");
            doc.SelectSingleNode("/usercards/departments").RemoveChild(node);
            Doc = doc;
        }

        public void addForm(string name, string ou)
        {
            XmlElement e = Doc.CreateElement("form");
            e.SetAttribute("name", name);
            e.SetAttribute("ou", ou);
            XmlDocument d = Doc;
            d.SelectSingleNode("/usercards/forms").AppendChild(e);
            Doc = d;
        }

        public void removeForm(string name)
        {
            XmlDocument doc = Doc;

            XmlNode node = doc.SelectSingleNode("/usercards/forms/form[@name='" + name + "']");
            doc.SelectSingleNode("/usercards/forms").RemoveChild(node);
            Doc = doc;
        }

        public Form[] getForms()
        {
            List<Form> deps = new List<Form>();
            foreach (XmlNode node in Doc.SelectNodes("/usercards/forms/form"))
                deps.Add(new Form(node.Attributes["name"].Value, node.Attributes["ou"].Value));
            return deps.ToArray();
        }

        public Form[] getForms(string ou)
        {
            List<Form> deps = new List<Form>();
            foreach (XmlNode node in Doc.SelectNodes("/usercards/forms/form[@ou='" + ou + "']"))
                deps.Add(new Form(node.Attributes["name"].Value, node.Attributes["ou"].Value));
            return deps.ToArray();
        }

    }

    public class Form
    {
        public Form() { }
        public Form(string name, string ou) { Name = name; OU = ou; }
        public string Name { get; set; }
        public string OU { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    public class Department
    {
        public Department(string name) { Name = name; }
        public Department() { }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Init
    {
        public Init()
        {
            StudentGroupName = hapConfig.Current.ADSettings.StudentsGroupName;
            ADConString = ConfigurationManager.ConnectionStrings[hapConfig.Current.ADSettings.ADConnectionString].ConnectionString;
            StudentEmailFormat = hapConfig.Current.BaseSettings.StudentEmailFormat;
            username = hapConfig.Current.ADSettings.ADUsername;
            password = hapConfig.Current.ADSettings.ADPassword;
        }
        public string StudentGroupName { get; set; }
        public string ADConString { get; set; }
        public string StudentEmailFormat { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}