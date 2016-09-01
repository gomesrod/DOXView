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
            throw new NotImplementedException();
        }
    }
}
