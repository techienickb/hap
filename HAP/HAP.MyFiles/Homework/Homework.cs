using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HAP.AD;
using System.DirectoryServices.AccountManagement;
using System.Web;
using System.Web.Security;
using HAP.Web.Configuration;
using System.Runtime.Serialization;

namespace HAP.MyFiles.Homework
{
    [DataContract()]
    public class Homework
    {
        [DataMember()]
        public string Name { get; set; }
        [DataMember()]
        public string Description { get; set; }
        [DataMember()]
        public string Start { get; set; }
        [DataMember()]
        public string End { get; set; }
        [DataMember()]
        public string Teacher { get; set; }
        [DataMember()]
        public string Path { get; set; }
        [DataMember()]
        public string TeacherName
        {
            get
            {
                return ADUtils.FindUserInfos(this.Teacher)[0].Notes;
            }
            private set { }
        }
        [DataMember()]
        public bool Mine { get; set; }
        [IgnoreDataMember()]
        public List<UserNode> UserNodes { get; set; }
        [DataMember()]
        public UserNode[] Nodes
        {
            get { return UserNodes.ToArray(); }
        }
        [IgnoreDataMember()]
        public string Token { get; set; }
        public Homework(string Teacher)
        {
            this.Teacher = Teacher;
            Name = Description = "";
            Start = End = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            UserNodes = new List<UserNode>();
            Mine = (IsVisible() == "Admin" || IsVisible() == "Teacher");
        }

        public Homework(XmlNode node, string Teacher) : this(Teacher)
        {
            Name = node.Attributes["name"].Value;
            Description = node.SelectSingleNode("description").InnerText;
            Start = node.Attributes["start"].Value;
            End = node.Attributes["end"].Value;
            Token = node.Attributes["token"].Value;
            Path = node.Attributes["path"].Value;
            foreach (XmlNode n in node.ChildNodes)
                if (n.Name == "add" || n.Name == "remove") UserNodes.Add(UserNode.Parse(n));
        }

        public UserInfo[] getMembers(string UserType)
        {
            List<UserInfo> results = new List<UserInfo>();
            if (UserType == "Teacher") results.Add(ADUtils.FindUserInfos(Teacher)[0]);
            UserNodes.Reverse();
            foreach (UserNode n in UserNodes)
            {
                if (n.Method == "Add")
                {
                    switch (n.Type)
                    {
                        case "User":
                            results.Add(ADUtils.FindUserInfos(n.Value)[0]);
                            break;
                        case "Role":
                            foreach (string s in ADUtils.GetUsersInRole(ADUtils.DirectoryRoot, hapConfig.Current.AD.UPN, n.Value, true)) results.Add(ADUtils.FindUserInfos(s)[0]);
                            break;
                        case "OU":
                            results.AddRange(ADUtils.FindUsersIn(n.Value));
                            break;
                    }
                }
                else
                {
                    switch (n.Type)
                    {
                        case "User":
                            results.RemoveAll(ui => ui.UserName.ToLower() == n.Value.ToLower());
                            break;
                        case "Role":
                            foreach (string s in ADUtils.GetUsersInRole(ADUtils.DirectoryRoot, hapConfig.Current.AD.UPN, n.Value, true)) results.RemoveAll(ui => ui.UserName.ToLower() == s.ToLower());
                            break;
                        case "OU":
                            foreach (UserInfo i in ADUtils.FindUsersIn(n.Value))
                                results.RemoveAll(ui => ui.UserName.ToLower() == i.UserName.ToLower());
                            break;
                    }
                }
            }
            UserNodes.Reverse();
            return results.ToArray();
        }

        public string IsVisible()
        { 
            if (HttpContext.Current.User.IsInRole("Domain Admins")) return "Admin";
            if (HttpContext.Current.User.Identity.Name.ToLower().Equals(Teacher.ToLower())) return "Teacher";
            UserNodes.Reverse();
            foreach (UserNode n in UserNodes)
            {
                if (n.Method == "Add")
                {
                    switch (n.Type)
                    {
                        case "User":
                            if (HttpContext.Current.User.Identity.Name.ToLower().Equals(n.Value.ToLower())) return n.Mode;
                            break;
                        case "Role":
                            if (HttpContext.Current.User.IsInRole(n.Value)) return n.Mode;
                            break;
                        case "OU":
                            if (ADUtils.FindUsersIn(n.Value).Count(ui => ui.UserName.ToLower().Equals(HttpContext.Current.User.Identity.Name.ToLower())) == 1) return n.Mode;
                            break;
                    }
                }
                else
                {
                    switch (n.Type)
                    {
                        case "User":
                            if (HttpContext.Current.User.Identity.Name.ToLower().Equals(n.Value.ToLower())) return "None";
                            break;
                        case "Role":
                            if (HttpContext.Current.User.IsInRole(n.Value)) return "None";
                            break;
                        case "OU":
                            if (ADUtils.FindUsersIn(n.Value).Count(ui => ui.UserName.ToLower().Equals(HttpContext.Current.User.Identity.Name.ToLower())) == 1) return "None";
                            break;
                    }
                }
            }
            UserNodes.Reverse();
            return "None";
        }
    }
}
