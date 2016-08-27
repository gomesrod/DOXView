using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DOXViewer.Model;
using DOXViewer.ModelLayout;

namespace DOXViewer.GUI
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
            
            String xmlFile = "D:\\rodrigo\\projects\\DOXViewer\\Sample\\INV0010437370-L-C0.xml";

            Layout layout = new Layout();

            ModelParser parser = new ModelParser(layout);
            XmlModel model;

            try
            {
                model = parser.parseXmlFile(xmlFile);
            } catch (Exception ex) {
                MessageBox.Show(this, ex.Message + "|" + ex.StackTrace);
                return;
            }
            
            foreach(XmlModelNode modelNode in model.Nodes) {
                documentTreeView.Nodes.Add(modelNode.Name);
            }
        }

    }
}
