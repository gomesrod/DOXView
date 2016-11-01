using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XmLift.ModelLayout
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
			XmlNode currentNode = doc.SelectSingleNode ("/XmLiftLayout"); 
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
                // Use default value TRUE when not present
				Boolean required = requiredNode != null ? Boolean.Parse (requiredNode.Value) : true;

                XmlNode custDescAttrNode = node.SelectSingleNode("@CustomDescriptionXPath");
                string custDescAttr = custDescAttrNode != null ? custDescAttrNode.Value : null;

				List<LayoutNode> children = extractChildNodes (node);
				List<LayoutValue> values = extractValues (node);
                List<LayoutDataTable> dataTables = extractDataTables(node);

                LayoutNode layoutNode = new LayoutNode(desc, xpath, required, custDescAttr, children, values, dataTables);

				resultList.Add (layoutNode);
			}
			return resultList;
		}

        private List<LayoutDataTable> extractDataTables(XmlNode node)
        {
            List<LayoutDataTable> dataTables = new List<LayoutDataTable>();
            foreach(XmlNode dataTableNode in node.SelectNodes("DataTable")) {
                string dataTableTitle = dataTableNode.SelectSingleNode("@Title").Value;
				string dataTableXpath = dataTableNode.SelectSingleNode("@RecordXPath").Value;

				List<LayoutDataTable.Column> values = extractColumns(dataTableNode);

                dataTables.Add(new LayoutDataTable(dataTableTitle, dataTableXpath, values));
            }

            return dataTables;
        }

		private List<LayoutDataTable.Column> extractColumns (XmlNode dataTableNode)
		{
			List<LayoutDataTable.Column> cols = new List<LayoutDataTable.Column> ();
			foreach(XmlNode node in dataTableNode.SelectNodes("Column")) 
			{
				string name = node.SelectSingleNode ("@Name").Value;
				string xpath = node.SelectSingleNode ("@ValueXPath").Value;

				cols.Add (new LayoutDataTable.Column(name, xpath));
			}
			return cols;
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
