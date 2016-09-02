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
"    <LayoutDescription>My Test Layout</LayoutDescription>" +
"    <EvaluationXPath>/this/must/be/in/the/xml</EvaluationXPath>" +
"    <Node Description='RootNode1' XPath='/node1' Required='True'>                       " +
"        <Node Description='NestedNode' XPath='nestedUnderRoot'>                         " +
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

			Assert.AreEqual ("My Test Layout", layout.Description);
			Assert.AreEqual ("/this/must/be/in/the/xml", layout.EvaluationXPath);

            Assert.AreEqual(2, layout.Nodes.Count);

            //RootNode1 attributes
            LayoutNode rootNode1 = layout.Nodes[0];
            Assert.AreEqual("RootNode1", rootNode1.Description);
			Assert.AreEqual("/node1", rootNode1.Xpath);
            Assert.IsTrue(rootNode1.Required);
			Assert.AreEqual (1, rootNode1.ChildNodes.Count); // One child node

            //RootNode2 attributes
            LayoutNode rootNode2 = layout.Nodes[1];
            Assert.AreEqual("RootNode2", rootNode2.Description);
			Assert.AreEqual("/node2", rootNode2.Xpath);
            Assert.IsFalse(rootNode2.Required);
			Assert.AreEqual (0, rootNode2.ChildNodes.Count); // Zero child nodes

			//RootNode2 values
			Assert.AreEqual(2, rootNode2.Values.Count);
			LayoutValue val1 = rootNode2.Values[0];

			//RootNode1 nested node
			LayoutNode nested = rootNode1.ChildNodes[0];
			Assert.AreEqual("NestedNode", nested.Description);
			Assert.AreEqual("nestedUnderRoot", nested.Xpath);
			Assert.IsTrue(nested.Required); // Not specified, use default
        }
    }
}
