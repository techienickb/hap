using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Web.Configuration;
using System.Configuration;

namespace HAP.Data.UserCard
{
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
