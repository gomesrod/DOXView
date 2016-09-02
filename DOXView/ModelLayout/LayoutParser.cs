using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DOXView.ModelLayout
{
    public class LayoutParser
    {
        public Layout parseXmlString(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            return buildLayout(doc);
        }
        
        public Layout parseXmlFile(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return buildLayout(doc);
        }

        private Layout buildLayout(XmlDocument doc)
        {			
			XmlNode currentNode = doc.SelectSingleNode ("/DOXViewLayout"); 
			string description = currentNode.SelectSingleNode ("LayoutDescription").InnerText;
			string evaluationXPath = currentNode.SelectSingleNode ("EvaluationXPath").InnerText;

			List<LayoutNode> children = getChildNodes (currentNode);

			Layout resultLayout = new Layout (description, evaluationXPath, children);
			return resultLayout;
        }

		private List<LayoutNode> getChildNodes (XmlNode currentNode)
		{
			List<LayoutNode> resultList = new List<LayoutNode>();
			foreach (XmlNode node in currentNode.SelectNodes("Node"))
			{
				string desc = node.SelectSingleNode ("@Description").Value;
				string xpath = node.SelectSingleNode ("@XPath").Value;

				XmlNode requiredNode = node.SelectSingleNode ("@Required");
				Boolean required;
				if (requiredNode != null) {
					required = Boolean.Parse (requiredNode.Value);
				} else {
					// Use default value
					required = true;
				}

				List<LayoutNode> children = getChildNodes (node);

				List<LayoutValue> values = null;

				LayoutNode layoutNode = new LayoutNode (desc, xpath, required, children, values);

				resultList.Add (layoutNode);
			}
			return resultList;
		}
			
    }
}
