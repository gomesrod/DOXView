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

			Assert.AreEqual (1, model.Nodes.Count);

			validateCustInfoNode (model.Nodes[0]);
		}

		private void validateCustInfoNode(XmlModelNode custInfoNode) {
            Assert.AreEqual("Invoice Information", custInfoNode.Description);
            Assert.IsFalse(custInfoNode.IsError);
            Assert.AreEqual(6, custInfoNode.Values.Count);
		}
	}
}

