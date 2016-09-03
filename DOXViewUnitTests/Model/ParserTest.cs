using System;
using System.Collections.Generic;
using NUnit.Framework;
using DOXView.ModelLayout;

namespace DOXView.Model
{
    [TestFixture]
    public class ParserTest
    {
        /*
         * All the tests will be based on the XML sample below
         * */
        private static String XML_INPUT =
"<Bill>                                                                                     " +
"	<Block>                                                                                 " +
"		<Customer Id='99912331'>                                                            " +
"			<Name>John Sample</Name>                                                        " +
"			<AddressPart part='street'>Telecom Customers Street</AddressPart>               " +
"			<AddressPart part='number'>9123</AddressPart>                                   " +
"			<AddressPart part='zip'>15121241</AddressPart>                                  " +
"		</Customer>                                                                         " +
"	</Block>                                                                                " +
"	<Block>                                                                                 " +
"		<ChargesSummary group='1'>                                                          " +
"			<Charge>                                                                        " +
"				<Description>Your Monthly Plan</Description>                                " +
"				<Value>99.99</Value>                                                        " +
"			</Charge>                                                                       " +
"			<Charge>                                                                        " +
"				<Description>Out of bundle calls</Description>                              " +
"				<Value>10.63</Value>                                                        " +
"			</Charge>                                                                       " +
"			<ChargeTotal>110.62</ChargeTotal>                                               " +
"		</ChargesSummary>                                                                   " +
"	</Block>	                                                                            " +
"	<Block>                                                                                 " +
"		<Calls>                                                                             " +
"			<Call type='local' destnumber='118171621' seconds='530' value='0.00'>           " +
"				<Tax Value='0.00'></Tax>                                                    " +
"			</Call>                                                                         " +
"			<Call type='local' destnumber='111231212' seconds='142' value='0.00'>           " +
"				<Tax Value='0.06'></Tax>                                                    " +
"			</Call>                                                                         " +
"			<Call type='long-distance' destnumber='3710488371' seconds='142' value='4.10'>  " +
"				<Tax Value='0.15'></Tax>                                                    " +
"			</Call>                                                                         " +
"			<Call type='long-distance' destnumber='4122391235' seconds='142' value='6.20'>  " +
"				<Tax Value='0.18'></Tax>                                                    " +
"			</Call>                                                                         " +
"			<Call type='local' destnumber='114123498' seconds='1059' value='0.00'>          " +
"				<Tax Value='0.00'></Tax>                                                    " +
"			</Call>                                                                         " +
"		</Calls>                                                                            " +
"	</Block>                                                                                " +
"</Bill>                                                                                    ";
        
        [Test]
        public void SingleOccurenceRootLevelNode()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("CustomerData", "/Bill/Block/Customer", true, 
					new List<LayoutNode>(), new List<LayoutValue>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);
            
            Assert.AreEqual("CustomerData", model.Nodes[0].Name);
            Assert.AreEqual(false, model.Nodes[0].IsError);
        }

        [Test]
        public void NonExistentNode_NotRequired()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
            layout.Nodes.Add(new LayoutNode("NonexistentField", "/This/Path/Is/Fake", false,
				new List<LayoutNode>(), new List<LayoutValue>()));
            
            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            // If the field is not required, it will just not be put in the result
            Assert.AreEqual(0, model.Nodes.Count);
        }

        [Test]
        public void NonExistentNode_Required()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("NonexistentField", "/This/Path/Is/Fake", true, 
				new List<LayoutNode>(), new List<LayoutValue>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            // If the field is required, it comes to the output with an error status
            Assert.AreEqual("NonexistentField", model.Nodes[0].Name);
            Assert.AreEqual(true, model.Nodes[0].IsError);
        }


        [Test]
        public void MultipleOccurencesRootLevelNode()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("AddressPart", "/Bill/Block/Customer/AddressPart", true, 
				new List<LayoutNode>(), new List<LayoutValue>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(3, model.Nodes.Count);

            Assert.AreEqual("AddressPart", model.Nodes[0].Name);
            Assert.AreEqual("AddressPart", model.Nodes[1].Name);
            Assert.AreEqual("AddressPart", model.Nodes[2].Name);
            
        }

        [Test]
        public void SingleOccurenceRootLevelNode_WithValues()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CustomerData", "/Bill/Block/Customer", true, 
				new List<LayoutNode>(), new List<LayoutValue>());
            rootNode.Values.Add(new LayoutValue("Customer ID", "@Id"));
            rootNode.Values.Add(new LayoutValue("Customer Name", "Name"));            
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual("CustomerData", model.Nodes[0].Name);
            Assert.AreEqual(false, model.Nodes[0].IsError);

            Assert.AreEqual(2, model.Nodes[0].Values.Count);

            Assert.AreEqual("Customer ID", model.Nodes[0].Values[0].Description);
            Assert.AreEqual("99912331", model.Nodes[0].Values[0].Value);

            Assert.AreEqual("Customer Name", model.Nodes[0].Values[1].Description);
            Assert.AreEqual("John Sample", model.Nodes[0].Values[1].Value);
        }

        [Test]
        public void NonExistentValue_NotRequired()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CustomerData", "/Bill/Block/Customer", true, 
				new List<LayoutNode>(), new List<LayoutValue>());
            rootNode.Values.Add(new LayoutValue("Missing", "@NotAValue"));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual(0, model.Nodes[0].Values.Count);
        }

        [Test]
        public void NonExistentValue_Required()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CustomerData", "/Bill/Block/Customer", true, 
				new List<LayoutNode>(), new List<LayoutValue>());
            rootNode.Values.Add(new LayoutValue("Missing", "@NotAValue", true));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual(1, model.Nodes[0].Values.Count);
            Assert.AreEqual("Missing", model.Nodes[0].Values[0].Description);
            Assert.AreEqual(true, model.Nodes[0].Values[0].IsError);
        }

        [Test]
        public void MultipleOccurenceRootLevelNodes_WithAttributes()
        {
			Layout layout = new Layout("Test Layout", "/Bill", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("Charge", "/Bill/Block/ChargesSummary/Charge", true, 
				new List<LayoutNode>(), new List<LayoutValue>());
            rootNode.Values.Add(new LayoutValue("Charge Description", "Description"));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(2, model.Nodes.Count);

            Assert.AreEqual("Charge", model.Nodes[0].Name);
            Assert.AreEqual(false, model.Nodes[0].IsError);

            Assert.AreEqual(1, model.Nodes[0].Values.Count);
            Assert.AreEqual("Charge Description", model.Nodes[0].Values[0].Description);
            Assert.AreEqual("Your Monthly Plan", model.Nodes[0].Values[0].Value);

            Assert.AreEqual("Charge", model.Nodes[1].Name);
            Assert.AreEqual(false, model.Nodes[1].IsError);

            Assert.AreEqual(1, model.Nodes[1].Values.Count);
            Assert.AreEqual("Charge Description", model.Nodes[1].Values[0].Description);
            Assert.AreEqual("Out of bundle calls", model.Nodes[1].Values[0].Value);
        }

    }
}
