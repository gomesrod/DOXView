using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.Model
{
    public class XmlModelValue
    {
        public Boolean IsError { get; private set; }
        public string Description { get; private set; }
        public string Value { get; private set; }

        public XmlModelValue(string desc, string val, Boolean er)
        {
            Description = desc;
            Value = val;
            IsError = er;
        }
    }
}
