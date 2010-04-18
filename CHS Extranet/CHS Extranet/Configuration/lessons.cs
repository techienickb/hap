using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class lessons : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new lesson();
        }

        protected override ConfigurationElement CreateNewElement(string elementname)
        {
            return new lesson(elementname);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((lesson)element).Name;
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


        public lesson this[int index]
        {
            get { return (lesson)BaseGet(index); }
            set { if (BaseGet(index) != null) { BaseRemoveAt(index); } BaseAdd(index, value); }
        }

        new public lesson this[string Name]
        {
            get { return (lesson)BaseGet(Name); }
        }

        public int IndexOf(lesson lesson)
        {
            return BaseIndexOf(lesson);
        }

        public void Add(lesson lesson)
        {
            BaseAdd(lesson);
            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(lesson lesson)
        {
            if (BaseIndexOf(lesson) >= 0)
                BaseRemove(lesson.Name);
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