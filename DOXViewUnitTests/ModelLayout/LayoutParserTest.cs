using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DOXView.ModelLayout
{
	[TestFixture]
    public class LayoutParserTest
    {
        private static string XML_IN =
"<DOXViewLayout>                                                                       " +
"    <Node Description='RootNode1' XPath='/node1' Required='True'>                       " +
"        <Node Description='NestedNode'>                                                 " +
"            <Value Description='NestedAttribute' XPath='@nestedAttrib' Required='True' /> " +
"        </Node>                                                                         " +
"        <Value Description='RootNodeAttribute1' XPath='@attrib1' Required='False' />      " +
"        <Value Description='RootNodeSomeInnerValue' XPath='childTag/another' />           " +
"    </Node>                                                                             " +
"    <Node Description='RootNode2' XPath='/node2' Required='False'>                      " +
"    </Node>                                                                             " +
"</DOXViewLayout>                                                                      ";

        [Test]
        public void InvalidXml()
        {

        }

        [Test]
        public void ParseConfigurationXml()
        {
            LayoutParser parser = new LayoutParser();

            Layout layout = parser.parseXmlString(XML_IN);

            Assert.AreEqual(2, layout.Nodes.Count);

            //RootNode1 attributes
            LayoutNode rootNode1 = layout.Nodes[0];
            Assert.AreEqual("RootNode1", rootNode1.Description);
            Assert.AreEqual("/node1", rootNode1.Description);
            Assert.IsTrue(rootNode1.Required);

            //RootNode2 attributes
            LayoutNode rootNode2 = layout.Nodes[0];
            Assert.AreEqual("RootNode2", rootNode2.Description);
            Assert.AreEqual("/node2", rootNode2.Description);
            Assert.IsFalse(rootNode2.Required);
        }
    }
}
