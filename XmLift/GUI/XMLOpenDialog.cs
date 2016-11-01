using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using XmLift.ModelLayout;

namespace XmLift.GUI
{
    public partial class XMLOpenDialog : Form
    {
        public string SelectedXmlPath { get; private set; }
        public Layout SelectedLayout { get; private set; }

        private LayoutManager layoutManager;
        private OpenFileDialog xmlOpenDialog = new OpenFileDialog();

        public XMLOpenDialog()
        {
            InitializeComponent();
        }

        public XMLOpenDialog(string defaultXml, Layout defaultLayout)
        {
            InitializeComponent();
            txtXmlPath.Text = Path.GetFullPath(defaultXml);
            xmlOpenDialog.InitialDirectory = Directory.GetParent(defaultXml).FullName;
            cboLayouts.Items.Add(defaultLayout);
            cboLayouts.SelectedItem = defaultLayout;
        }

        public void LoadLayouts() {
            layoutManager = new LayoutManager();

            try
            {
                layoutManager.LoadLayouts();
            }
            catch (Exception ex)
            {
                //TODO Improve this error handling
                MessageBox.Show(ex.Message, "Error loading layouts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public LayoutManager LayoutManager { 
            get { return layoutManager; } 
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtXmlPath.Text))
            {
                labelError.Text = "Inform the XML path";
                labelError.Visible = true;
                this.DialogResult = DialogResult.None;
                return;
            }

            if (File.Exists(txtXmlPath.Text))
            {
                SelectedXmlPath = txtXmlPath.Text;
            }
            else
            {
                labelError.Text = "The selected file does not exist";
                labelError.Visible = true;
                this.DialogResult = DialogResult.None;
                return;
            }

            if (cboLayouts.SelectedIndex >= 0)
            {
                SelectedLayout = (Layout)cboLayouts.SelectedItem;
            }
            else
            {
                labelError.Text = "A document layout was not selected";
                labelError.Visible = true;
                this.DialogResult = DialogResult.None;
                return;
            }
        }

        private void btnXmlBrowse_Click(object sender, EventArgs e)
        {
            labelError.Visible = false;

            xmlOpenDialog.CheckFileExists = true;
            if (xmlOpenDialog.ShowDialog() == DialogResult.OK)
            {
                txtXmlPath.Text = xmlOpenDialog.FileName;
                validateXmlPath();                
            }

        }

		private void btnLayoutHelp_Click(object sender, EventArgs e)
		{
			string msg = "Layout files can be read from the following locations:\n";
			foreach (string dir in layoutManager.getLayoutDirs()) {
				msg = msg + "-> " + dir + "\n";
			}
			MessageBox.Show (this, msg);
		}

        private void txtXmlPath_Enter(object sender, EventArgs e)
        {
            labelError.Visible = false;
        }

        private void txtXmlPath_Leave(object sender, EventArgs e)
        {
            validateXmlPath();
        }    

        private void cboLayouts_Enter(object sender, EventArgs e)
        {
            labelError.Visible = false;
        }

        private void validateXmlPath()
        {
            List<Layout> layouts = layoutManager.listCompatibleLayouts(txtXmlPath.Text);

            Layout previouslySelected = cboLayouts.SelectedIndex >= 0 ? (Layout)cboLayouts.SelectedItem : null;
            Boolean reselectPrevious = false;

            cboLayouts.Items.Clear();
            if (layouts.Count > 0)
            {
                foreach (Layout l in layouts) {
                    cboLayouts.Items.Add(l);
                    if (previouslySelected == l)
                    {
                        // If the previously selected item is still there, select it again by default
                        reselectPrevious = true;
                    }
                }
                              
                cboLayouts.Text = "Select the layout:";

                if (reselectPrevious) {
                    cboLayouts.SelectedItem = previouslySelected;
                }
                
                cboLayouts.Enabled = true;
            }
            else
            {
                cboLayouts.Text = "-- No compatible layout --";
                cboLayouts.Enabled = false;
            }
        }

    }
}
