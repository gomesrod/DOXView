using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXViewer.Model
{
    public class XmlModelValue
    {
        public string Description { get; private set; }
        public string Value { get; private set; }

        public XmlModelValue(string desc, string val)
        {
            Description = desc;
            Value = val;
        }
    }
}
