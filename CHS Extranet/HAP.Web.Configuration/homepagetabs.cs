using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagetabs : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new homepagetab();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new homepagetab(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((homepagetab)element).Name;
        }

        public new string AddElementName
        {
            get { return base.AddElementName; }
            set { base.AddElementName = value; }
        }

        public new string ClearElementName
        {
            get { return base.ClearElementName; }
            set { base.AddElementName = value; }
        }

        public new string RemoveElementName
        {
            get { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }


        public homepagetab this[int index]
        {
            get { return (homepagetab)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public homepagetab this[string Name]
        {
            get { return (homepagetab)BaseGet(Name); }
        }

        public int IndexOf(homepagetab link)
        {
            return BaseIndexOf(link);
        }

        public void Add(homepagetab link)
        {
            BaseAdd(link);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(homepagetab link)
        {
            if (BaseIndexOf(link) >= 0)
                BaseRemove(link.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
            // Add custom code here.
        }


        public Dictionary<string, homepagetab> FilteredTabs
        {
            get
            {
                Dictionary<string, homepagetab> links = new Dictionary<string, homepagetab>();
                foreach (homepagetab link in this)
                    if (link.ShowTo == "All") links.Add(link.Name, link);
                    else if (link.ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in link.ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = HttpContext.Current.User.IsInRole(s);
                        if (vis) links.Add(link.Name, link);
                    }
                return links;
            }
        }

    }
}