using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagelinkgroups : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new homepagelinkgroup();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new homepagelinkgroup(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((homepagelinkgroup)element).Name;
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


        public homepagelinkgroup this[int index]
        {
            get { return (homepagelinkgroup)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public homepagelinkgroup this[string Name]
        {
            get { return (homepagelinkgroup)BaseGet(Name); }
        }

        public int IndexOf(homepagelinkgroup link)
        {
            return BaseIndexOf(link);
        }

        public void Add(homepagelinkgroup link)
        {
            BaseAdd(link);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(homepagelinkgroup link)
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


    }
}