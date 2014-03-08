using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace HAP.BookingSystem
{
    public class BookingRules : List<BookingRule>
    {
        public BookingRules() : base()
        {
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/app_data/bookingrules.xml")))
            {
                StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("~/app_data/bookingrules.xml"));
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<bookingrules />");
                sw.Close();
                sw.Dispose();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/app_data/bookingrules.xml"));
            foreach (XmlNode node in doc.SelectNodes("/bookingrules/rule")) this.Add(new BookingRule(node));
        }

        public static void Execute(Booking b, Resource r, BookingSystem bs, bool IsRemoveEvent)
        {
            BookingRules br = new BookingRules();
            foreach (BookingRule rule in br) rule.ExecuteRule(b, r, bs, IsRemoveEvent);
        }
    }

    public class BookingRule
    {
        public BookingRule(XmlNode node)
        {
            this.Conditions = new List<BookingCondition>();
            this.Actions = new List<string>();
            foreach (XmlNode n in node.ChildNodes)
                if (n.Name == "action") this.Actions.Add(n.InnerText);
                else this.Conditions.Add(new BookingCondition(n));
        }
        private List<BookingCondition> Conditions;
        private List<string> Actions;

        public void ExecuteRule(Booking b, Resource r, BookingSystem bs, bool IsRemoveEvent)
        {
            bool good = true;
            foreach (BookingCondition con in Conditions)
                switch (con.MasterOperation)
                {
                    case BookingConditionOperation.And:
                        good = good && con.IsConditionMet(b, r, bs);
                        break;
                    case BookingConditionOperation.Or:
                        good = good || con.IsConditionMet(b, r, bs);
                        break;
                }
            if (good)
            {
                foreach (string a in this.Actions)
                {
                    try
                    {
                        if (a.ToLower().StartsWith("bookcharging("))
                        {
                            string c = a.Remove(0, "bookcharging(".Length).TrimEnd(new char[] { ')' });
                            object o = BookingCondition.processCondition(c, b, r, bs);
                            if (o is Booking)
                            {
                                Booking ob = o as Booking;
                                XmlDocument doc = HAP.BookingSystem.BookingSystem.BookingsDoc;
                                if (!IsRemoveEvent && bs.islessonFree(b.Room, ob.Lesson))
                                {
                                    XmlElement node = doc.CreateElement("Booking");
                                    node.SetAttribute("date", b.Date.ToShortDateString());
                                    node.SetAttribute("lesson", ob.Lesson);
                                    node.SetAttribute("room", b.Room);
                                    if (r.Type == ResourceType.Laptops)
                                    {
                                        node.SetAttribute("ltroom", "--");
                                        node.SetAttribute("ltheadphones", b.LTHeadPhones.ToString());
                                    }
                                    node.SetAttribute("username", "systemadmin");
                                    node.SetAttribute("uid", b.uid);
                                    node.SetAttribute("name", "CHARGING");
                                    doc.SelectSingleNode("/Bookings").AppendChild(node);
                                }
                                else
                                {
                                    if (doc.SelectSingleNode("/Bookings/Booking[@date='" + b.Date.ToShortDateString() + "' and @lesson='" + b.Lesson + "' and @room='" + b.Room + "' and @uid='" + b.uid + "']") != null)
                                        doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + b.Date.ToShortDateString() + "' and @lesson='" + b.Lesson + "' and @room='" + b.Room + "' and @uid='" + b.uid + "']"));
                                }
                            }
                        }
                        else if (a.ToLower().StartsWith("book("))
                        {
                            string c = a.Remove(0, "book(".Length).TrimEnd(new char[] { ')' });
                            object o = BookingCondition.processCondition(c, b, r, bs);
                            if (o is Booking)
                            {
                                Booking ob = o as Booking;
                                XmlDocument doc = HAP.BookingSystem.BookingSystem.BookingsDoc;
                                if (!IsRemoveEvent)
                                {
                                    XmlElement node = doc.CreateElement("Booking");
                                    node.SetAttribute("date", b.Date.ToShortDateString());
                                    node.SetAttribute("lesson", ob.Lesson);
                                    node.SetAttribute("room", b.Room);
                                    if (r.Type == ResourceType.Laptops)
                                    {
                                        node.SetAttribute("ltroom", "--");
                                        node.SetAttribute("ltheadphones", b.LTHeadPhones.ToString());
                                    }
                                    else if (r.Type == ResourceType.Equipment) node.SetAttribute("equiproom", b.EquipRoom);
                                    node.SetAttribute("room", b.Room);
                                    node.SetAttribute("uid", b.uid);
                                    node.SetAttribute("username", b.Username);
                                    node.SetAttribute("name", b.Name);
                                    node.SetAttribute("count", b.Count.ToString());
                                    doc.SelectSingleNode("/Bookings").AppendChild(node);
                                }
                                else
                                {
                                    if (doc.SelectSingleNode("/Bookings/Booking[@date='" + b.Date.ToShortDateString() + "' and @lesson='" + b.Lesson + "' and @room='" + b.Room + "' and @uid='" + b.uid + "']") != null)
                                        doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + b.Date.ToShortDateString() + "' and @lesson='" + b.Lesson + "' and @room='" + b.Room + "' and @uid='" + b.uid + "']"));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HAP.Web.Logging.EventViewer.Log("BookingSystem.BookingRule", "Failed Action: " + Actions[0] + "\n\n" + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
            }
        }


        
    }
    public enum BookingConditionOperation { And, Or, Not, Equals, Null, GT, LT, GTE, LTE, NotNull }

    public class BookingCondition
    {
        public BookingCondition(XmlNode node)
        {
            this.Condition1 = node.Attributes["condition1"].Value;
            this.Condition2 = node.Attributes["condition2"].Value;
            this.Operation = (BookingConditionOperation)Enum.Parse(typeof(BookingConditionOperation), node.Attributes["operation"].Value);
            this.MasterOperation = (BookingConditionOperation)Enum.Parse(typeof(BookingConditionOperation), node.Name);
        }

        public string Condition1 { get; private set; }
        public string Condition2 { get; private set; }
        public BookingConditionOperation Operation { get; private set; }
        public BookingConditionOperation MasterOperation { get; private set; }

        public bool IsConditionMet(Booking b, Resource r, BookingSystem bs)
        {
            try
            {
                object comp1 = processCondition(Condition1, b, r, bs), comp2 = processCondition(Condition1, b, r, bs);

                switch (Operation)
                {
                    case BookingConditionOperation.Equals:
                        return comp1.Equals(comp2);
                    case BookingConditionOperation.GT:
                        if (comp1 is int)
                            return (int)comp1 > int.Parse(comp2.ToString());
                        else if (comp1 is DateTime)
                            return (DateTime)comp1 > DateTime.Parse(comp2.ToString());
                        return false;
                    case BookingConditionOperation.GTE:
                        if (comp1 is int)
                            return (int)comp1 >= int.Parse(comp2.ToString());
                        else if (comp1 is DateTime)
                            return (DateTime)comp1 >= DateTime.Parse(comp2.ToString());
                        return false;
                    case BookingConditionOperation.LT:
                        if (comp1 is int)
                            return (int)comp1 < int.Parse(comp2.ToString());
                        else if (comp1 is DateTime)
                            return (DateTime)comp1 < DateTime.Parse(comp2.ToString());
                        return false;
                    case BookingConditionOperation.LTE:
                        if (comp1 is int)
                            return (int)comp1 <= int.Parse(comp2.ToString());
                        else if (comp1 is DateTime)
                            return (DateTime)comp1 <= DateTime.Parse(comp2.ToString());
                        return false;
                    case BookingConditionOperation.Not:
                        return comp1.Equals(comp2) ? false : true;
                    case BookingConditionOperation.Null:
                        return comp1 == null;
                    case BookingConditionOperation.NotNull:
                        return comp1 != null;
                }
            }
            catch { }

            return false;
        }

        public static object processCondition(string Condition, Booking b, Resource r, BookingSystem bs)
        {
            int x = 0; bool bo = false;
            if (Condition.ToLower().StartsWith("resource."))
                return processCondition(r, Condition.Remove(0, "resource.".Length));
            else if (Condition.ToLower().StartsWith("booking.")) return processCondition(b, Condition.Remove(0, "booking.".Length));
            else if (Condition.ToLower().StartsWith("bookingsystem.")) return processCondition(r, Condition.Remove(0, "bookingsystem.".Length));
            else if (Condition.ToLower().StartsWith("user.")) return processCondition(HttpContext.Current.User, Condition.Remove(0, "user.".Length));
            else if (int.TryParse(Condition, out x)) return x;
            else if (bool.TryParse(Condition, out bo)) return bo;
            else return Condition;
        }

        public static object processCondition(object o, string Condition)
        {
            if (Condition == "") return o;
            string cons1 = Condition.IndexOf('.') == -1 ? Condition : Condition.Substring(0, Condition.IndexOf('.')).TrimEnd(new char[] { '.' });
            string cons2 = Condition.IndexOf('.') == -1 ? "" : Condition.Substring(Condition.IndexOf('.')).TrimStart(new char[] { '.' });
            if (cons1.Contains('('))
            {
                string[] conditions = cons1.Remove(0, cons1.IndexOf('(')).TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' }).Split(new char[] { ',' });
                cons1 = cons1.Substring(0, cons1.IndexOf('(')).TrimEnd(new char[] { '(' });

                if (conditions.Length == 1 && conditions[0] == "")
                {
                    return processCondition(o.GetType().GetMethod(cons1).Invoke(o, null), cons2);
                }
                else
                {
                    List<object> objects = new List<object>();
                    foreach (string s in conditions)
                    {
                        int x = 0;
                        if (int.TryParse(s, out x)) objects.Add(x);
                        else objects.Add(s);
                    }
                    return processCondition(o.GetType().GetMethod(cons1).Invoke(o, objects.ToArray()), cons2);
                }
            }
            else return processCondition(o.GetType().GetProperty(cons1).GetValue(o, null), cons2);
        }
    }
}
