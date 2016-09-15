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

			Assert.AreEqual (3, model.Nodes.Count);

			validateCustInfoNode (model.Nodes[0]);
            validateTelecomServicesSummary(model.Nodes[1]);
			validateCallsNode(model.Nodes[2]);
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

		private void validateCustInfoNode(XmlModelNode custInfoNode) 
		{
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

		private void validateCallsNode(XmlModelNode callsNode)
		{
			Assert.AreEqual ("Calls", callsNode.Description);
			Assert.IsFalse (callsNode.IsError);

            Assert.AreEqual(2, callsNode.Values.Count);
            Assert.AreEqual("5", callsNode.Values[0].Value);
            // The value below depends on regional settings, so we accept both formats
            Assert.IsTrue("10.3".Equals(callsNode.Values[1].Value) || "10,3".Equals(callsNode.Values[1].Value), "Validate sum() value");

			Assert.AreEqual (2, callsNode.DataTables.Count);

			{
				XmlModelDataTable data1 = callsNode.DataTables [0];
				Assert.AreEqual ("Local Calls", data1.Title);
				Assert.AreEqual (3, data1.Records.Count);

				Assert.AreEqual (4, data1.Records [0].Count); // Number of keys in the record
				Assert.AreEqual ("118171621", data1.Records[0]["Dest Number"]);
				Assert.AreEqual ("530", data1.Records[0]["Duration"]);
				Assert.AreEqual ("0.00", data1.Records[0]["Price"]);
				Assert.AreEqual ("0.00", data1.Records[0]["Tax"]);

				Assert.AreEqual (4, data1.Records [1].Count);
				Assert.AreEqual ("111231212", data1.Records[1]["Dest Number"]);
				Assert.AreEqual ("142", data1.Records[1]["Duration"]);
				Assert.AreEqual ("0.00", data1.Records[1]["Price"]);
				Assert.AreEqual ("0.06", data1.Records[1]["Tax"]);

				Assert.AreEqual (4, data1.Records [2].Count);
				Assert.AreEqual ("114123498", data1.Records[2]["Dest Number"]);
				Assert.AreEqual ("1059", data1.Records[2]["Duration"]);
				Assert.AreEqual ("0.00", data1.Records[2]["Price"]);
				Assert.AreEqual ("0.00", data1.Records[2]["Tax"]);
			}
			{
				XmlModelDataTable data2 = callsNode.DataTables [1];
				Assert.AreEqual ("Long-Distance Calls", data2.Title);
				Assert.AreEqual (2, data2.Records.Count);

				Assert.AreEqual (4, data2.Records [0].Count);
				Assert.AreEqual ("3710488371", data2.Records [0] ["Dest Number"]);
				Assert.AreEqual ("142", data2.Records [0] ["Duration"]);
				Assert.AreEqual ("4.10", data2.Records [0] ["Price"]);
				Assert.AreEqual ("0.15", data2.Records [0] ["Tax"]);

				Assert.AreEqual (4, data2.Records [1].Count);
				Assert.AreEqual ("4122391235", data2.Records [1] ["Dest Number"]);
				Assert.AreEqual ("201", data2.Records [1] ["Duration"]);
				Assert.AreEqual ("6.20", data2.Records [1] ["Price"]);
				Assert.AreEqual ("0.18", data2.Records [1] ["Tax"]);
			}
		}
	}
}

