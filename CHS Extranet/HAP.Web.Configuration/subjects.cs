using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class subjects : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new subject();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new subject(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((subject)element).Name;
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


        public subject this[int index]
        {
            get { return (subject)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public subject this[string Name]
        {
            get { return (subject)BaseGet(Name); }
        }

        public int IndexOf(subject subject)
        {
            return BaseIndexOf(subject);
        }

        public void Add(subject subject)
        {
            BaseAdd(subject);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(subject subject)
        {
            if (BaseIndexOf(subject) >= 0)
                BaseRemove(subject.Name);
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