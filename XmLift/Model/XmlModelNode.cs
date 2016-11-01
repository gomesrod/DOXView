using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmLift.Model
{
    public class XmlModelNode
    {
        public string Description { get; private set; }
        public Boolean IsError { get; private set; }

        public List<XmlModelNode> ChildNodes { get; private set; }
        public List<XmlModelValue> Values { get; private set; }

        public List<XmlModelDataTable> DataTables { get; private set; }

        public XmlModelNode(String desc, Boolean er, List<XmlModelNode> children,
                List<XmlModelValue> vals, List<XmlModelDataTable> dataTables)
        {
            Description = desc;
            IsError = er;
            ChildNodes = children;
            Values = vals;
            DataTables = dataTables;
        }
    }
}
