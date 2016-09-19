using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DOXView.ModelLayout;

namespace DOXView.GUI
{
    public partial class XMLOpenDialog : Form
    {
        public string SelectedXmlPath { get; private set; }
        public Layout SelectedLayout { get; private set; }

        private LayoutManager layoutManager;

        public XMLOpenDialog(LayoutManager lm)
        {
            InitializeComponent();
            layoutManager = lm;
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

            OpenFileDialog fd = new OpenFileDialog();
            fd.CheckFileExists = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtXmlPath.Text = fd.FileName;
                validateXmlPath();                
            }

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
            List<Layout> layouts = layoutManager.listCompatibleLayouts(txtXmlPath);

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
                              
                cboLayouts.DisplayMember = "Description";
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
