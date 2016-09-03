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
			try {
            	doc.LoadXml(xmlString);
			} catch (XmlException xe) {
				throw new ParserException ("The layout configuration contains invalid XML. Error: " + xe.Message, xe);
			}
            return buildLayout(doc);
        }
        
        public Layout parseXmlFile(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
			try {
				doc.Load(xmlPath);
			} catch (XmlException xe) {
				throw new ParserException ("The layout configuration contains invalid XML. Error: " + xe.Message, xe);
			}
            return buildLayout(doc);
        }

        private Layout buildLayout(XmlDocument doc)
        {			
			XmlNode currentNode = doc.SelectSingleNode ("/DOXViewLayout"); 
			string description = currentNode.SelectSingleNode ("LayoutDescription").InnerText;
			string evaluationXPath = currentNode.SelectSingleNode ("EvaluationXPath").InnerText;

			List<LayoutNode> children = extractChildNodes (currentNode);

			Layout resultLayout = new Layout (description, evaluationXPath, children);
			return resultLayout;
        }

		private List<LayoutNode> extractChildNodes (XmlNode currentNode)
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

				List<LayoutNode> children = extractChildNodes (node);
				List<LayoutValue> values = extractValues (node);

				LayoutNode layoutNode = new LayoutNode (desc, xpath, required, children, values);

				resultList.Add (layoutNode);
			}
			return resultList;
		}

		private List<LayoutValue> extractValues (XmlNode node) 
		{
			List<LayoutValue> values = new List<LayoutValue>();
			foreach (XmlNode valueNode in node.SelectNodes("Value")) 
			{
				string valueDesc = valueNode.SelectSingleNode ("@Description").Value;
				string valueXpath = valueNode.SelectSingleNode ("@XPath").Value;

				XmlNode valueRequiredNode = valueNode.SelectSingleNode ("@Required");
				Boolean valueRequired;
				if (valueRequiredNode != null) {
					valueRequired = Boolean.Parse (valueRequiredNode.Value);
				} else {
					// Use default value
					valueRequired = true;
				}

				values.Add (new LayoutValue(valueDesc, valueXpath, valueRequired));
			}	

			return values;
		}
    }
}
