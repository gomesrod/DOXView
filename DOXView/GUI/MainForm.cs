using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DOXView.Model;
using DOXView.ModelLayout;

namespace DOXView.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void openXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LayoutParser layoutParser = new LayoutParser();

            // The IDE is supposed to run this program from [DoxViewTests directory]/bin/[Debug|Release]
            // We go back three levels to find the sample files in the root of the project
            Layout layout = layoutParser.parseXmlFile("../../../DemoFiles/SampleLayout.xml");

            ModelParser parser = new ModelParser(layout);
            XmlModel model;

            try
            {
                model = parser.parseXmlFile("../../../DemoFiles/SampleData.xml");
            } catch (Exception ex) {
                MessageBox.Show(this, ex.Message + "|" + ex.StackTrace);
                return;
            }

            addXmlNodesToTree(model.Nodes, documentTreeView.Nodes);
        }

        private void addXmlNodesToTree(IList<XmlModelNode> modelNodes, TreeNodeCollection treeNodeCollection)
        {
            foreach (XmlModelNode modelNode in modelNodes)
            {
                TreeNode treeNode = new TreeNode(modelNode.Description);
                treeNode.Tag = modelNode;               

                // Proceed adding child nodes
                addXmlNodesToTree(modelNode.ChildNodes, treeNode.Nodes);

                treeNodeCollection.Add(treeNode);
            }
        }

        private void documentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            XmlModelNode modelNode = (XmlModelNode)e.Node.Tag;
            dataGridView.DataSource = modelNode.Values;
        }

    }
}
