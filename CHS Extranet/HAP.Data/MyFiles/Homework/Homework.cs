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

namespace HAP.Data.MyFiles.Homework
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
        public string TeacherName
        {
            get
            {
                return ADUtils.FindUserInfos(this.Teacher)[0].Notes;
            }
            private set { }
        }
        [IgnoreDataMember()]
        public List<UserNode> UserNodes { get; set; }
        public Homework(string Teacher)
        {
            this.Teacher = Teacher;
            Name = Description = "";
            Start = End = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            UserNodes = new List<UserNode>();
        }

        public Homework(XmlNode node, string Teacher) : this(Teacher)
        {
            Name = node.Attributes["name"].Value;
            Description = node.SelectSingleNode("description").InnerText;
            Start = node.Attributes["start"].Value;
            End = node.Attributes["end"].Value;
            foreach (XmlNode n in node.ChildNodes)
                if (n.Name == "add" || n.Name == "remove") UserNodes.Add(UserNode.Parse(n));
        }

        public UserInfo[] getMembers(UserNodeMode UserType)
        {
            List<UserInfo> results = new List<UserInfo>();
            if (UserType == UserNodeMode.Teacher) results.Add(ADUtils.FindUserInfos(Teacher)[0]);
            UserNodes.Reverse();
            foreach (UserNode n in UserNodes)
            {
                if (n.Method == UserNodeMethod.Add)
                {
                    switch (n.Type)
                    {
                        case UserNodeType.User:
                            results.Add(ADUtils.FindUserInfos(n.Value)[0]);
                            break;
                        case UserNodeType.Role:
                            foreach (string s in ADUtils.GetUsersInRole(ADUtils.DirectoryRoot, hapConfig.Current.AD.UPN, n.Value, true)) results.Add(ADUtils.FindUserInfos(s)[0]);
                            break;
                        case UserNodeType.OU:
                            results.AddRange(ADUtils.FindUsersIn(n.Value));
                            break;
                    }
                }
                else
                {
                    switch (n.Type)
                    {
                        case UserNodeType.User:
                            results.RemoveAll(ui => ui.UserName.ToLower() == n.Value.ToLower());
                            break;
                        case UserNodeType.Role:
                            foreach (string s in ADUtils.GetUsersInRole(ADUtils.DirectoryRoot, hapConfig.Current.AD.UPN, n.Value, true)) results.RemoveAll(ui => ui.UserName.ToLower() == s.ToLower());
                            break;
                        case UserNodeType.OU:
                            foreach (UserInfo i in ADUtils.FindUsersIn(n.Value))
                                results.RemoveAll(ui => ui.UserName.ToLower() == i.UserName.ToLower());
                            break;
                    }
                }
            }
            UserNodes.Reverse();
            return results.ToArray();
        }

        public UserNodeMode IsVisible()
        { 
            if (HttpContext.Current.User.IsInRole("Domain Admins")) return UserNodeMode.Admin;
            if (HttpContext.Current.User.Identity.Name.ToLower().Equals(Teacher.ToLower())) return UserNodeMode.Teacher;
            UserNodes.Reverse();
            foreach (UserNode n in UserNodes)
            {
                if (n.Method == UserNodeMethod.Add)
                {
                    switch (n.Type)
                    {
                        case UserNodeType.User:
                            if (HttpContext.Current.User.Identity.Name.ToLower().Equals(n.Value.ToLower())) return n.Mode;
                            break;
                        case UserNodeType.Role:
                            if (HttpContext.Current.User.IsInRole(n.Value)) return n.Mode;
                            break;
                        case UserNodeType.OU:
                            if (ADUtils.FindUsersIn(n.Value).Count(ui => ui.UserName.ToLower().Equals(HttpContext.Current.User.Identity.Name.ToLower())) == 1) return n.Mode;
                            break;
                    }
                }
                else
                {
                    switch (n.Type)
                    {
                        case UserNodeType.User:
                            if (HttpContext.Current.User.Identity.Name.ToLower().Equals(n.Value.ToLower())) return UserNodeMode.None;
                            break;
                        case UserNodeType.Role:
                            if (HttpContext.Current.User.IsInRole(n.Value)) return UserNodeMode.None;
                            break;
                        case UserNodeType.OU:
                            if (ADUtils.FindUsersIn(n.Value).Count(ui => ui.UserName.ToLower().Equals(HttpContext.Current.User.Identity.Name.ToLower())) == 1) return UserNodeMode.None;
                            break;
                    }
                }
            }
            UserNodes.Reverse();
            return UserNodeMode.None;
        }
    }
}
