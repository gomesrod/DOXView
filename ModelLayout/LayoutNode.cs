using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXViewer.ModelLayout
{
    public class LayoutNode
    {
        private List<LayoutNode> childNodes = new List<LayoutNode>();
        private List<LayoutValue> values = new List<LayoutValue>();

        public String Description { get; private set; }
        public String Xpath { get; private set; }
        public Boolean Required { get; private set; }

        public LayoutNode(string desc, string path, Boolean req)
        {
            Description = desc;
            Xpath = path;
            Required = req;
        }

        public List<LayoutNode> Nodes
        {
            get
            {
                return childNodes;
            }
        }

        public List<LayoutValue> Values
        {
            get
            {
                return values;
            }
        }
    }
}
