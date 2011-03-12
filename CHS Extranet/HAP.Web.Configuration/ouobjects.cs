using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class ouobjects : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ouobject();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new ouobject(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ouobject)element).Name;
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


        public ouobject this[int index]
        {
            get { return (ouobject)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public ouobject this[string Name]
        {
            get { return (ouobject)BaseGet(Name); }
        }

        public int IndexOf(ouobject ouobject)
        {
            return BaseIndexOf(ouobject);
        }

        public void Add(ouobject ouobject)
        {
            BaseAdd(ouobject);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(ouobject ouobject)
        {
            if (BaseIndexOf(ouobject) >= 0)
                BaseRemove(ouobject.Name);
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