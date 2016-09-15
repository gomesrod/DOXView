using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DOXView.GUI.DictionaryToGridAdapter
{
    class DictionaryTypedList : List<Dictionary<string, string>>, ITypedList
    {
        PropertyDescriptorCollection propertyDescriptors;

        public DictionaryTypedList(List<Dictionary<string, string>> source)
        {
            if (source.Count > 0)
            {
                propertyDescriptors = buildPropertyDescriptorsFromItem(source[0]);
            }
            else
            {
                propertyDescriptors = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
            }

            this.AddRange(source);
        }

        private PropertyDescriptorCollection buildPropertyDescriptorsFromItem(Dictionary<string, string> dictionary)
        {
            PropertyDescriptorCollection propDescCol = new PropertyDescriptorCollection(new PropertyDescriptor[] { });

            foreach (string colname in dictionary.Keys)
            {
                propDescCol.Add(new DictionaryFieldPropertyDescriptor(colname));
            }

            return propDescCol;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return propertyDescriptors;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "Dictionary<string,string>";
        }
    }
}
