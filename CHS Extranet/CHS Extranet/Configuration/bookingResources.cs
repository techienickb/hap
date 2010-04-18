using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class bookingresources : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new bookingResource();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new bookingResource(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((bookingResource)element).Name;
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


        public bookingResource this[int index]
        {
            get { return (bookingResource)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public bookingResource this[string Name]
        {
            get { return (bookingResource)BaseGet(Name); }
        }

        public int IndexOf(bookingResource resource)
        {
            return BaseIndexOf(resource);
        }

        public void Add(bookingResource resource)
        {
            BaseAdd(resource);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(bookingResource resource)
        {
            if (BaseIndexOf(resource) >= 0)
                BaseRemove(resource.Name);
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