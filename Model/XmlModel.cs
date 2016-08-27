using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXViewer.Model
{
    public class XmlModel
    {
        private IList<XmlModelNode> nodes;

        internal XmlModel(IList<XmlModelNode> modelNodes)
        {
            nodes = modelNodes;
        }

        public IList<XmlModelNode> Nodes
        {
            get
            {
                return nodes;
            }
        }
    }
}
