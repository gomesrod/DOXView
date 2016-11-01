using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmLift.ModelLayout
{
    public class LayoutNode
    {
		public List<LayoutNode> ChildNodes{ get; private set;}
		public List<LayoutValue> Values { get; private set;}
        public List<LayoutDataTable> DataTables { get; private set; }

        public String Description { get; private set; }
        public String Xpath { get; private set; }
        public Boolean Required { get; private set; }
        public String CustomDescriptionXPath { get; private set; }

		public LayoutNode (string desc, string path, Boolean req, string customDescPath, 
            List<LayoutNode> children, List<LayoutValue> vals, List<LayoutDataTable> dataTables)
        {
            Description = desc;
            Xpath = path;
            Required = req;
            CustomDescriptionXPath = customDescPath;
			ChildNodes = children;
			Values = vals;
            DataTables = dataTables;
        }
    }
}
