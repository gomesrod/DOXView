using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.ModelLayout
{
    public class Layout
    {       
		public string Description { get; private set;}
		public string EvaluationXPath { get; private set;}
		public List<LayoutNode> Nodes { get; private set;}

		public Layout(string desc, string evalxpath, List<LayoutNode> nodes)
		{
			Description = desc;
			EvaluationXPath = evalxpath;
			Nodes = nodes;
		}
    }
}
