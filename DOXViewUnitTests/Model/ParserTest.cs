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
"<Library>" +
"   <Owner Name='Mike The Art Collector' Age='54' />" +
"	<Disks>" +
"	   <CD>" +
"	      <Artist>Michael Jackson</Artist>" +
"	      <Title>Thriller</Title>" +
"	      <Gender>Pop</Gender>" +
"	   </CD>" +
"	   <CD>" +
"	      <Artist>Nirvana</Artist>" +
"	      <Title>Nevermind</Title>" +
"	      <Gender>Rock</Gender>" +
"	   </CD>" +
"	</Disks>" +
"	<Books>" +
"      <Book>" +
"         <Author>Bram Stoker</Author>" +
"         <Title>Dracula</Title>" +
"      </Book>" +
"      <Book>" +
"         <Author>Miguel de Cervantes</Author>" +
"         <Title>Dom Quixote</Title>" +
"      </Book>" +
"	</Books>" +
"</Library>";
        
        [Test]
        public void SingleOccurenceRootLevelNode()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("Owner Information", "/Library/Owner", true, 
					new List<LayoutNode>(), new List<LayoutValue>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual("Owner Information", model.Nodes[0].Description);
            Assert.AreEqual(false, model.Nodes[0].IsError);
        }

        [Test]
        public void NonExistentNode_NotRequired()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
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
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
            
            LayoutNode node = new LayoutNode("NonexistentField", "/This/Path/Is/Fake", true,
                new List<LayoutNode>(), new List<LayoutValue>());
            LayoutValue someValue = new LayoutValue("Some Value", "@does_not_exist_as_well");
            node.Values.Add(someValue);

			layout.Nodes.Add(node);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            // If the field is required, it comes to the output with an error status
            Assert.AreEqual("NonexistentField", model.Nodes[0].Description);
            Assert.AreEqual(true, model.Nodes[0].IsError);

            // The value is not inserted
            Assert.AreEqual(0, model.Nodes[0].Values.Count);
        }


        [Test]
        public void MultipleOccurencesRootLevelNode()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("CD", "/Library/Disks/CD", true, 
				new List<LayoutNode>(), new List<LayoutValue>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(2, model.Nodes.Count);

            Assert.AreEqual("CD", model.Nodes[0].Description);
            Assert.AreEqual("CD", model.Nodes[1].Description);           
        }

        [Test]
        public void NonExistentValue_NotRequired()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CustomerData", "/Library/Owner", true, 
				new List<LayoutNode>(), new List<LayoutValue>());
            rootNode.Values.Add(new LayoutValue("Missing", "@NotAValue", false));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual(0, model.Nodes[0].Values.Count);
        }

        [Test]
        public void NonExistentValue_Required()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
            LayoutNode rootNode = new LayoutNode("CustomerData", "/Library/Owner", true,
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
        public void NodesWithValues()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CD Info", "/Library/Disks/CD", true, 
				new List<LayoutNode>(), new List<LayoutValue>());
            rootNode.Values.Add(new LayoutValue("CD Gender", "Gender"));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(2, model.Nodes.Count);

            // Instance 1
            Assert.AreEqual("CD Info", model.Nodes[0].Description);
            Assert.AreEqual(false, model.Nodes[0].IsError);

            Assert.AreEqual(1, model.Nodes[0].Values.Count);
            Assert.AreEqual("CD Gender", model.Nodes[0].Values[0].Description);
            Assert.AreEqual("Pop", model.Nodes[0].Values[0].Value);

            // Instance 2
            Assert.AreEqual("CD Info", model.Nodes[1].Description);
            Assert.AreEqual(false, model.Nodes[1].IsError);

            Assert.AreEqual(1, model.Nodes[1].Values.Count);
            Assert.AreEqual("CD Gender", model.Nodes[1].Values[0].Description);
            Assert.AreEqual("Rock", model.Nodes[1].Values[0].Value);
        }

    }
}
