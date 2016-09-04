﻿using System;
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
                result.Add(new XmlModelNode(layoutNode.Description, true, new List<XmlModelNode>(), new List<XmlModelValue>()));
            }

            foreach (XmlNode xmlNode in foundXmlNodes)
            {
                List<XmlModelValue> values = extractValues(layoutNode.Values, xmlNode);

                // Call recursively to add the children of this layout node
                List<XmlModelNode> childNodes = new List<XmlModelNode>();
                foreach (LayoutNode layoutChildNode in layoutNode.ChildNodes)
                {
                    List<XmlModelNode> childrenOfThisNode = extractXmlModelNodes(layoutChildNode, xmlNode);
                    childNodes.AddRange(childrenOfThisNode);
                }

                XmlModelNode newNode = new XmlModelNode(layoutNode.Description, false, childNodes, values);
                result.Add(newNode);
            }          

            return result;
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
