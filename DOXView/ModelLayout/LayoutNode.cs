using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.ModelLayout
{
    public class LayoutNode
    {
		public List<LayoutNode> ChildNodes{ get; private set;}
		public List<LayoutValue> Values { get; private set;}

        public String Description { get; private set; }
        public String Xpath { get; private set; }
        public Boolean Required { get; private set; }

		public LayoutNode (string desc, string path, Boolean req, List<LayoutNode> children, List<LayoutValue> vals)
        {
            Description = desc;
            Xpath = path;
            Required = req;
			ChildNodes = children;
			Values = vals;
        }
    }
}
