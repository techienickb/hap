using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagelinks : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new homepagelink();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new homepagelink(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((homepagelink)element).Name;
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


        public homepagelink this[int index]
        {
            get { return (homepagelink)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public homepagelink this[string Name]
        {
            get { return (homepagelink)BaseGet(Name); }
        }

        public int IndexOf(homepagelink link)
        {
            return BaseIndexOf(link);
        }

        public void Add(homepagelink link)
        {
            BaseAdd(link);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(homepagelink link)
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