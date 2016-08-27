using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXViewer.ModelLayout
{
    public class Layout
    {               
        private List<LayoutNode> _nodes = new List<LayoutNode>();

        public IList<LayoutNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }
    }
}
