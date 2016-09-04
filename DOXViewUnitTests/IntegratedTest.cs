using System;
using NUnit.Framework;
using DOXView.Model;
using DOXView.ModelLayout;

namespace DOXViewUnitTests
{
	// This class simulates a real usage scenario, when both the Layout
	// and the XML data will be loaded from files
	[TestFixture]
	public class IntegratedTest
	{
		private ModelParser parser;

		[SetUp]
		public void SetUp()
		{
			LayoutParser layoutParser = new LayoutParser ();

			// The IDE is supposed to run this program from [DoxViewTests directory]/bin/[Debug|Release]
			// We go back three levels to find the sample files in the root of the project
			Layout layout = layoutParser.parseXmlFile ("../../../DemoFiles/SampleLayout.xml");

			parser = new ModelParser (layout);
		}

		[Test]
		public void RunIntegratedTest ()
		{
			XmlModel model = parser.parseXmlFile("../../../DemoFiles/SampleData.xml");

			Assert.AreEqual (2, model.Nodes.Count);

			validateCustInfoNode (model.Nodes[0]);
            validateTelecomServicesSummary(model.Nodes[1]);
		}

        private void validateTelecomServicesSummary(XmlModelNode servicesSummaryNode)
        {
            Assert.AreEqual("Telecom Services Summary", servicesSummaryNode.Description);
            Assert.AreEqual(2, servicesSummaryNode.ChildNodes.Count);

            XmlModelNode charge1 = servicesSummaryNode.ChildNodes[0];
            Assert.AreEqual("Charge [Your Monthly Plan]", charge1.Description);
            Assert.IsFalse(charge1.IsError);

            Assert.AreEqual(1, charge1.Values.Count);
            Assert.AreEqual("Amount", charge1.Values[0].Description);
            Assert.AreEqual("99.99", charge1.Values[0].Value);

            XmlModelNode charge2 = servicesSummaryNode.ChildNodes[1];
            Assert.AreEqual("Charge [Out-of-bundle calls]", charge2.Description);
            Assert.IsFalse(charge2.IsError);

            Assert.AreEqual(1, charge2.Values.Count);
            Assert.AreEqual("Amount", charge2.Values[0].Description);
            Assert.AreEqual("10.63", charge2.Values[0].Value);
        }

		private void validateCustInfoNode(XmlModelNode custInfoNode) {
            Assert.AreEqual("Invoice Information", custInfoNode.Description);
            Assert.IsFalse(custInfoNode.IsError);

            Assert.AreEqual(7, custInfoNode.Values.Count);

            XmlModelValue custId = custInfoNode.Values[0];
            Assert.AreEqual("Customer ID", custId.Description);
            Assert.AreEqual("99912331", custId.Value);
            Assert.IsFalse(custId.IsError);

            // Jumping to the 6th value (country), which was not found and is
            // expected to be marked as error
            XmlModelValue country = custInfoNode.Values[5];
            Assert.AreEqual("Country", country.Description);
            Assert.IsNull(country.Value);
            Assert.IsTrue(country.IsError);
		}
	}
}

