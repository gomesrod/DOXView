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
"    <Node Description='RootNode2' XPath='/node2' Required='False' CustomDescriptionXPath='@SomeRelevantAttribute'> " +
"        <LayoutDataTable Title='A List Of Values' RecordXPath='repeatingTag'>" +
"             <Column Name='Column 1' ValueXPath='@column1' />" +
"             <Column Name='Column 2' ValueXPath='@column2' />" +
"        </LayoutDataTable>" +
"    </Node>                                                                             " +
"</DOXViewLayout>                                                                      ";

        [Test]
        public void InvalidXml_malformed()
        {
			String xmlIn = "<invalid XML></not_closing_the_right_tag>";

			LayoutParser parser = new LayoutParser();

			try {
				parser.parseXmlString (xmlIn);
				Assert.Fail("A Parse Exception was expected here");
			} catch (ParserException pe) {
				Assert.IsTrue(pe.Message.Contains("invalid XML"), "Checking Exception message");
			}
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
            Assert.IsNull(rootNode1.CustomDescriptionXPath);
            Assert.IsTrue(rootNode1.Required);
			Assert.AreEqual (1, rootNode1.ChildNodes.Count); // One child node

			//RootNode1 values
			Assert.AreEqual(2, rootNode1.Values.Count);
			LayoutValue val1 = rootNode1.Values[0];
			Assert.AreEqual ("RootNodeAttribute1", val1.Description);
			Assert.AreEqual ("@attrib1", val1.XPath);
			Assert.AreEqual (false, val1.Required);

			LayoutValue val2 = rootNode1.Values[1];
			Assert.AreEqual ("RootNodeSomeInnerValue", val2.Description);
			Assert.AreEqual ("childTag/another", val2.XPath);
			Assert.AreEqual (true, val2.Required); // Not specified, use default

			//RootNode1 nested node
			LayoutNode nested = rootNode1.ChildNodes[0];
			Assert.AreEqual("NestedNode", nested.Description);
			Assert.AreEqual("nestedUnderRoot", nested.Xpath);
			Assert.IsTrue(nested.Required); // Not specified, use default

			// Nested Node Values
			Assert.AreEqual(1, nested.Values.Count);
			LayoutValue val3 = nested.Values[0];
			Assert.AreEqual ("NestedAttribute", val3.Description);
			Assert.AreEqual ("@nestedAttrib", val3.XPath);
			Assert.AreEqual (true, val3.Required); // Not specified, use default
            
            //RootNode2 attributes
            LayoutNode rootNode2 = layout.Nodes[1];
            Assert.AreEqual("RootNode2", rootNode2.Description);
            Assert.AreEqual("/node2", rootNode2.Xpath);
            Assert.AreEqual("@SomeRelevantAttribute", rootNode2.CustomDescriptionXPath);
            Assert.IsFalse(rootNode2.Required);
            Assert.AreEqual(0, rootNode2.ChildNodes.Count); // Zero child nodes

            //LayoutDataTable inside RootNode2
            List<LayoutDataTable> dataTables = rootNode2.DataTables;
            Assert.AreEqual(1, dataTables.Count);
            Assert.AreEqual("A List Of Values", dataTables[0].Title);
            Assert.AreEqual("repeatingTag", dataTables[0].RecordXPath);

            Assert.AreEqual(2, dataTables[0].Columns.Count);

            LayoutDataTable.Column dataTableV1 = dataTables[0].Columns[0];
            Assert.AreEqual("Column 1", dataTableV1.Description);
            Assert.AreEqual("@column1", dataTableV1.XPath);

            LayoutDataTable.Column dataTableV2 = dataTables[0].Columns[1];
            Assert.AreEqual("Column 2", dataTableV2.Description);
            Assert.AreEqual("@column2", dataTableV2.XPath);
        }
    }
}
