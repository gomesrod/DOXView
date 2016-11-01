using System;
using System.Collections.Generic;
using NUnit.Framework;
using XmLift.ModelLayout;

namespace XmLift.Model
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
"      <Book>" +
"         <Author>George Orwell</Author>" +
"         <Title>1984</Title>" +
"      </Book>" +
"	</Books>" +
"</Library>";
        
        [Test]
        public void SingleOccurenceRootLevelNode()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("Owner Information", "/Library/Owner", true, null,
                    new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>()));

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
            layout.Nodes.Add(new LayoutNode("NonexistentField", "/This/Path/Is/Fake", false, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>()));
            
            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            // If the field is not required, it will just not be put in the result
            Assert.AreEqual(0, model.Nodes.Count);
        }

        [Test]
        public void NonExistentNode_Required()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
            
            LayoutNode node = new LayoutNode("NonexistentField", "/This/Path/Is/Fake", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
            LayoutValue someValue = new LayoutValue("Some Value", "@does_not_exist_as_well");
            node.Values.Add(someValue);

			layout.Nodes.Add(node);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            // If the field is required, it comes to the output with an error status
            Assert.AreEqual("NonexistentField", model.Nodes[0].Description);
            Assert.AreEqual(true, model.Nodes[0].IsError);

            // The value is not inserted
            Assert.IsNull(model.Nodes[0].ChildNodes);
            Assert.IsNull(model.Nodes[0].DataTables);
            Assert.IsNull(model.Nodes[0].Values);
        }


        [Test]
        public void MultipleOccurencesRootLevelNode()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			layout.Nodes.Add(new LayoutNode("CD", "/Library/Disks/CD", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(2, model.Nodes.Count);

            Assert.AreEqual("CD", model.Nodes[0].Description);
            Assert.AreEqual("CD", model.Nodes[1].Description);           
        }

        [Test]
        public void MultipleOccurencesRootLevelNode_WithCustomDescriptions()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
            layout.Nodes.Add(new LayoutNode("CD", "/Library/Disks/CD", true, "Title",
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>()));

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(2, model.Nodes.Count);

            Assert.AreEqual("CD [Thriller]", model.Nodes[0].Description);
            Assert.AreEqual("CD [Nevermind]", model.Nodes[1].Description);
        }

        [Test]
        public void NonExistentValue_NotRequired()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CustomerData", "/Library/Owner", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
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
            LayoutNode rootNode = new LayoutNode("CustomerData", "/Library/Owner", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
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
        public void NodeWithValue_valueIsXmlAttribute()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
            LayoutNode rootNode = new LayoutNode("Library Owner", "/Library/Owner", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
            rootNode.Values.Add(new LayoutValue("Name", "@Name"));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual("Library Owner", model.Nodes[0].Description);
            Assert.AreEqual(false, model.Nodes[0].IsError);

            Assert.AreEqual(1, model.Nodes[0].Values.Count);
            Assert.AreEqual("Name", model.Nodes[0].Values[0].Description);
            Assert.AreEqual("Mike The Art Collector", model.Nodes[0].Values[0].Value);
        }


        [Test]
        public void NodeWithValue_valueIsXPathFunction()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
            LayoutNode rootNode = new LayoutNode("Disks", "/Library/Disks", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
            rootNode.Values.Add(new LayoutValue("CD Count", "count(CD)"));
            layout.Nodes.Add(rootNode);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);

            Assert.AreEqual("Disks", model.Nodes[0].Description);
            Assert.AreEqual(false, model.Nodes[0].IsError);

            Assert.AreEqual(1, model.Nodes[0].Values.Count);
            Assert.AreEqual("CD Count", model.Nodes[0].Values[0].Description);
            Assert.AreEqual("2", model.Nodes[0].Values[0].Value);
        }

        [Test]
        public void NodesWithValues_valueIsTagText()
        {
			Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());
			LayoutNode rootNode = new LayoutNode("CD Info", "/Library/Disks/CD", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
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

        [Test]
        public void SequenceOfNodes()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());

            LayoutNode node1 = new LayoutNode("CD", "/Library/Disks/CD", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
            LayoutNode node2 = new LayoutNode("Book", "/Library/Books/Book", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());

            layout.Nodes.Add(node1);
            layout.Nodes.Add(node2);
            
            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(5, model.Nodes.Count);

            Assert.AreEqual("CD", model.Nodes[0].Description);
            Assert.AreEqual("CD", model.Nodes[1].Description);
            Assert.AreEqual("Book", model.Nodes[2].Description);
            Assert.AreEqual("Book", model.Nodes[3].Description);
            Assert.AreEqual("Book", model.Nodes[4].Description);
        }


        [Test]
        public void NestedNodesAndValues()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());

            LayoutNode layoutRoot = new LayoutNode("Library Root", "/Library", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
            LayoutNode disksLayoutNode = new LayoutNode("My Disks", "Disks", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());
            LayoutNode cdLayoutNode = new LayoutNode("CD", "CD", true, null,
                new List<LayoutNode>(), new List<LayoutValue>(), new List<LayoutDataTable>());

            LayoutValue artistLayoutValue = new LayoutValue("The Artist", "Artist");

            layout.Nodes.Add(layoutRoot);
            layoutRoot.ChildNodes.Add(disksLayoutNode);
            disksLayoutNode.ChildNodes.Add(cdLayoutNode);
            cdLayoutNode.Values.Add(artistLayoutValue);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            // Validate root level node
            Assert.AreEqual(1, model.Nodes.Count, "Checking root level node");
            XmlModelNode libraryRoot = model.Nodes[0];
            Assert.AreEqual("Library Root", libraryRoot.Description);

            // Second level node
            Assert.AreEqual(1, libraryRoot.ChildNodes.Count, "Checking second level node");
            XmlModelNode disksLevel = libraryRoot.ChildNodes[0];
            Assert.AreEqual("My Disks", disksLevel.Description);

            // Third level node (two instances of CD)
            Assert.AreEqual(2, disksLevel.ChildNodes.Count);
            XmlModelNode cd1 = disksLevel.ChildNodes[0];
            XmlModelNode cd2 = disksLevel.ChildNodes[1];
            Assert.AreEqual("CD", cd1.Description);
            Assert.AreEqual("CD", cd2.Description);

            // Values under the third level node
            Assert.AreEqual(1, cd1.Values.Count);
            Assert.AreEqual("The Artist", cd1.Values[0].Description);
            Assert.AreEqual("Michael Jackson", cd1.Values[0].Value);

            Assert.AreEqual(1, cd2.Values.Count);
            Assert.AreEqual("The Artist", cd2.Values[0].Description);
            Assert.AreEqual("Nirvana", cd2.Values[0].Value);
        }

        [Test]
        public void DataTable()
        {
            Layout layout = new Layout("Test Layout", "/Library", new List<LayoutNode>());

			LayoutDataTable layoutdataTable = new LayoutDataTable("Book List", "Book", new List<LayoutDataTable.Column>());
			layoutdataTable.Columns.Add(new LayoutDataTable.Column("The Author", "Author"));
			layoutdataTable.Columns.Add(new LayoutDataTable.Column("The Title", "Title"));

            List<LayoutDataTable> dataTables = new List<LayoutDataTable>();
            dataTables.Add(layoutdataTable);

            LayoutNode node = new LayoutNode("Books", "/Library/Books", true, null, 
                new List<LayoutNode>(), new List<LayoutValue>(), dataTables);
			layout.Nodes.Add (node);

            ModelParser parser = new ModelParser(layout);
            XmlModel model = parser.parseXmlString(XML_INPUT);

            Assert.AreEqual(1, model.Nodes.Count);
            Assert.AreEqual(1, model.Nodes[0].DataTables.Count);

            XmlModelDataTable dataTable = model.Nodes[0].DataTables[0];

            Assert.AreEqual("Book List", dataTable.Title);
            Assert.AreEqual(3, dataTable.Records.Count);

            Assert.AreEqual("Bram Stoker", dataTable.Records[0]["The Author"]);
            Assert.AreEqual("Dracula", dataTable.Records[0]["The Title"]);

            Assert.AreEqual("Miguel de Cervantes", dataTable.Records[1]["The Author"]);
            Assert.AreEqual("Dom Quixote", dataTable.Records[1]["The Title"]);

            Assert.AreEqual("George Orwell", dataTable.Records[2]["The Author"]);
            Assert.AreEqual("1984", dataTable.Records[2]["The Title"]);
        }
    }
}
