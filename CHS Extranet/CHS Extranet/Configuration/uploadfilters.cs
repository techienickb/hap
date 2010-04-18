using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class uploadfilters : ConfigurationElementCollection
    {
        //Images (*.jpg;*.gif)|*.jpg;*.gif|All Files (*.*)|*.*

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new uploadfilter();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new uploadfilter(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((uploadfilter)element).Name;
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


        public uploadfilter this[int index]
        {
            get { return (uploadfilter)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public uploadfilter this[string Name]
        {
            get { return (uploadfilter)BaseGet(Name); }
        }

        public int IndexOf(uploadfilter filter)
        {
            return BaseIndexOf(filter);
        }

        public void Add(uploadfilter filter)
        {
            BaseAdd(filter);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(uploadfilter filter)
        {
            if (BaseIndexOf(filter) >= 0)
                BaseRemove(filter.Name);
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