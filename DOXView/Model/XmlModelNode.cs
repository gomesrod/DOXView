using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.Model
{
    public class XmlModelNode
    {
        private String name;
        private Boolean error;
        private List<XmlModelNode> childNodes = new List<XmlModelNode>();
        private List<XmlModelValue> values = new List<XmlModelValue>();

        public XmlModelNode(String n, Boolean er)
        {
            name = n;
            error = er;
        }

        public String Name 
        {
            get
            {
                return name;
            }
        }

        public List<XmlModelNode> Nodes
        {
            get
            {
                return childNodes;
            }
        }

        public List<XmlModelValue> Values{
            get{
                return values;
            }
        }

        public Boolean IsError
        {
            get
            {
                return error;
            }
        }
    }
}
