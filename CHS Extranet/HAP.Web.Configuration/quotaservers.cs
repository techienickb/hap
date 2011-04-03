using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class quotaservers : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new quotaserver();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new quotaserver(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((quotaserver)element).Expression;
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


        public quotaserver this[int index]
        {
            get { return (quotaserver)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public quotaserver this[string Expression]
        {
            get { return (quotaserver)BaseGet(Expression); }
        }

        public int IndexOf(quotaserver quota)
        {
            return BaseIndexOf(quota);
        }

        public void Add(quotaserver quota)
        {
            BaseAdd(quota);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(quotaserver quota)
        {
            if (BaseIndexOf(quota) >= 0)
                BaseRemove(quota.Expression);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string expression)
        {
            BaseRemove(expression);
        }

        public void Clear()
        {
            BaseClear();
            // Add custom code here.
        }
    }
}
