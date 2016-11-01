using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace XmLift.GUI.DictionaryToGridAdapter
{
    class DictionaryFieldPropertyDescriptor : PropertyDescriptor
    {
        private string propertyName;

        public DictionaryFieldPropertyDescriptor(string propname)
            : base(propname, new Attribute[] { })
        {
            propertyName = propname;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override object GetValue(object component)
        {
            return ((Dictionary<string, string>)component)[propertyName];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }

        public override void ResetValue(object component)
        {
            // Not relevant.
        }

        public override void SetValue(object component, object value)
        {
            ((Dictionary<string, string>)component).Add(propertyName, (string)value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
