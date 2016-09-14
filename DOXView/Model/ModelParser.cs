using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DOXView.ModelLayout;

namespace DOXView.Model
{
    public class ModelParser
    {
        private Layout layout;

        public ModelParser(Layout layout)
        {
            this.layout = layout;
        }

        public XmlModel parseXmlFile(string xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);

            return createModel(doc);
        }

        public XmlModel parseXmlString(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            return createModel(doc);
        }


        private XmlModel createModel(XmlDocument doc)
        {
            List<XmlModelNode> resultModelNodes = new List<XmlModelNode>();

            foreach (LayoutNode layoutNode in layout.Nodes)
            {
                List<XmlModelNode> matchesForTheCurrentLayoutNode = extractXmlModelNodes(layoutNode, doc);
                resultModelNodes.AddRange(matchesForTheCurrentLayoutNode);
            }
            
            return new XmlModel(resultModelNodes);
        }

        private List<XmlModelNode> extractXmlModelNodes(LayoutNode layoutNode, XmlNode docOrCurrentNode)
        {
            List<XmlModelNode> result = new List<XmlModelNode>();

            XmlNodeList foundXmlNodes = docOrCurrentNode.SelectNodes(layoutNode.Xpath);

            if (foundXmlNodes.Count == 0 && layoutNode.Required)
            {
                result.Add(new XmlModelNode(layoutNode.Description, true, null, null, null));
            }

            foreach (XmlNode xmlNode in foundXmlNodes)
            {
                List<XmlModelValue> values = extractValues(layoutNode.Values, xmlNode);
                List<XmlModelDataTable> dataTables = extractDataTables(layoutNode.DataTables, xmlNode);

                // Call recursively to add the children of this layout node
                List<XmlModelNode> childNodes = new List<XmlModelNode>();
                foreach (LayoutNode layoutChildNode in layoutNode.ChildNodes)
                {
                    List<XmlModelNode> childrenOfThisNode = extractXmlModelNodes(layoutChildNode, xmlNode);
                    childNodes.AddRange(childrenOfThisNode);
                }

                string nodeDescription = layoutNode.Description;
                if (layoutNode.CustomDescriptionXPath != null)
                {
                    XmlNode customDescriptionNode = xmlNode.SelectSingleNode(layoutNode.CustomDescriptionXPath);
                    if (customDescriptionNode != null) {
                        nodeDescription = nodeDescription + " ["
                            + (customDescriptionNode.Value != null ? customDescriptionNode.Value : customDescriptionNode.InnerText)
                            + "]";
                    }
                    else
                    {
                        nodeDescription = nodeDescription + " [" + layoutNode.CustomDescriptionXPath + "]";
                    }
                }

                XmlModelNode newNode = new XmlModelNode(nodeDescription, false, childNodes, values, dataTables);
                result.Add(newNode);
            }          

            return result;
        }

        private List<XmlModelDataTable> extractDataTables(List<LayoutDataTable> layoutDataTables, XmlNode xmlNode)
        {
            List<XmlModelDataTable> dataTables = new List<XmlModelDataTable>();

            foreach (LayoutDataTable layoutdataTable in layoutDataTables)
            {
				List<Dictionary<string, string>> records = extractDataTableRecords (xmlNode, layoutdataTable);
				dataTables.Add(new XmlModelDataTable(layoutdataTable.Title, records));
            }

            return dataTables;
        }

		private List<Dictionary<string, string>> extractDataTableRecords (XmlNode xmlNode, LayoutDataTable layoutdataTable)
		{
			List<Dictionary<string, string>> records = new List<Dictionary<string, string>> ();

			foreach (XmlNode xmlRecord in xmlNode.SelectNodes (layoutdataTable.RecordXPath)) {
				Dictionary<string, string> record = new Dictionary<string, string> ();

				foreach (LayoutDataTable.Column layoutColumn in layoutdataTable.Columns)
				{
					XmlNode valueNode = xmlRecord.SelectSingleNode (layoutColumn.ValueXPath);
					if (valueNode == null) {
						record.Add (layoutColumn.Name, "#ERR#");
					} else {
						record.Add (layoutColumn.Name, valueNode.Value ?? valueNode.InnerText);
					}
				}

				records.Add (record);
			}

			return records;
		}

        private static List<XmlModelValue> extractValues(List<LayoutValue> layoutValues, XmlNode xmlNode)
        {
            List<XmlModelValue> values = new List<XmlModelValue>();
            foreach (LayoutValue layoutValue in layoutValues)
            {
                XmlNodeList foundXmlValues = xmlNode.SelectNodes(layoutValue.XPath);
                foreach (XmlNode xmlValue in foundXmlValues)
                {
                    string textValue = xmlValue.Value != null ? xmlValue.Value : xmlValue.InnerText;
                    values.Add(new XmlModelValue(layoutValue.Description, textValue, false));
                }
                if (foundXmlValues.Count == 0 && layoutValue.Required)
                {
                    values.Add(new XmlModelValue(layoutValue.Description, null, true));
                }
            }
            return values;
        }
    }
}
