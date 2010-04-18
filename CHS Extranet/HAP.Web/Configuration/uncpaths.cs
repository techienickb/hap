using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class uncpaths : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new uncpath();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new uncpath(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((uncpath)element).Drive;
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


        public uncpath this[int index]
        {
            get { return (uncpath)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public uncpath this[string Drive]
        {
            get { return (uncpath)BaseGet(Drive); }
        }

        public int IndexOf(uncpath path)
        {
            return BaseIndexOf(path);
        }

        public void Add(uncpath path)
        {
            BaseAdd(path);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(uncpath path)
        {
            if (BaseIndexOf(path) >= 0)
                BaseRemove(path.Drive);
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