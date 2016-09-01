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
            List<XmlModelNode> modelNodes = new List<XmlModelNode>();

            List<XmlModelNode> currentModelNodeList = modelNodes;

            XmlNodeList foundXmlNodes = doc.SelectNodes(layout.Nodes[0].Xpath);

            if (foundXmlNodes.Count == 0 && layout.Nodes[0].Required) {
                currentModelNodeList.Add(new XmlModelNode(layout.Nodes[0].Description, true));
            }

            foreach(XmlNode xmlNode in foundXmlNodes) 
            {
                XmlModelNode newNode = new XmlModelNode(layout.Nodes[0].Description, false);
                currentModelNodeList.Add(newNode);

                foreach (LayoutValue layoutValue in layout.Nodes[0].Values)
                {
                    XmlNodeList foundXmlValues = xmlNode.SelectNodes(layoutValue.XPath);
                    foreach (XmlNode xmlValue in foundXmlValues)
                    {
                        string textValue = xmlValue.Value != null ? xmlValue.Value : xmlValue.InnerText;
                        newNode.Values.Add(new XmlModelValue(layoutValue.Description, textValue, false));
                    }
                    if (foundXmlValues.Count == 0 && layoutValue.Required) {
                        newNode.Values.Add(new XmlModelValue(layoutValue.Description, null, true));
                    }
                }
            }
            
            return new XmlModel(modelNodes);
        }
    }
}
