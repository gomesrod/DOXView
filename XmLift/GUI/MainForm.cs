using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XmLift.Model;
using XmLift.ModelLayout;
using XmLift.GUI.DictionaryToGridAdapter;
using System.IO;

namespace XmLift.GUI
{
    public partial class MainForm : Form
    {
        private List<XmlModelValue> currentValuesList;
        private XMLOpenDialog xmlOpenDialog;
        private XmlModel preloadedModel = null;

        public MainForm()
        {
            InitializeComponent();
            xmlOpenDialog = new XMLOpenDialog();
        }

        public MainForm(XmlModel preloadedModel_, string defaultXml, Layout defaultLayout)
        {
            InitializeComponent();
            preloadedModel = preloadedModel_;
            xmlOpenDialog = new XMLOpenDialog(defaultXml, defaultLayout);
        }
                
        private void openXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (xmlOpenDialog.LayoutManager == null)
            {
                xmlOpenDialog.LoadLayouts();
            }

            if (xmlOpenDialog.ShowDialog() == DialogResult.OK)
            {
                ModelParser parser = new ModelParser(xmlOpenDialog.SelectedLayout);
                XmlModel model;
                try
                {
                    model = parser.parseXmlFile(xmlOpenDialog.SelectedXmlPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message + "|" + ex.StackTrace);
                    return;
                }

                documentTreeView.Nodes.Clear();
                addXmlNodesToTree(model.Nodes, documentTreeView.Nodes);
                documentTreeView.SelectedNode = documentTreeView.Nodes[0];
            }
        }

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close ();
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (preloadedModel != null) {
                documentTreeView.Nodes.Clear();
                addXmlNodesToTree(preloadedModel.Nodes, documentTreeView.Nodes);
                documentTreeView.SelectedNode = documentTreeView.Nodes[0];
            }            
        }

        private DataGridView prepareValuesGridView()
        {
            DataGridView valueGridView = new DataGridView();

            //create the column programatically
            DataGridViewCell defaultValueCell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn()
            {
                CellTemplate = defaultValueCell,
                Name = "ValueDescription",
                HeaderText = "Description",
                DataPropertyName = "Description" // Tell the column which property of FileName it should use
            };

            DataGridViewTextBoxColumn colValue = new DataGridViewTextBoxColumn()
            {
                CellTemplate = defaultValueCell,
                Name = "ValueContents",
                HeaderText = "Value",
                DataPropertyName = "Value" // Tell the column which property of FileName it should use
            };

            valueGridView.AllowUserToAddRows = false;
            valueGridView.AllowUserToDeleteRows = false;
            valueGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            valueGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            valueGridView.ColumnHeadersVisible = false;
            valueGridView.Location = new System.Drawing.Point(3, 3);
            valueGridView.Name = "valueGridView";
            valueGridView.ReadOnly = true;
            valueGridView.RowHeadersVisible = false;
            valueGridView.Size = new System.Drawing.Size(486, 67);
            valueGridView.TabIndex = 0;
            valueGridView.BorderStyle = BorderStyle.None;
            valueGridView.BackgroundColor = Color.FromKnownColor(KnownColor.Window);
            valueGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.valuesGridView_CellFormatting);
            valueGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridView_resize_on_DataBindingComplete);
            
            valueGridView.AutoGenerateColumns = false;
            valueGridView.Columns.Add(colDescription);
            valueGridView.Columns.Add(colValue);

            this.gridContainerPanel.Controls.Add(valueGridView);

            return valueGridView;
        }

        private void addXmlNodesToTree(IList<XmlModelNode> modelNodes, TreeNodeCollection treeNodeCollection)
        {
            foreach (XmlModelNode modelNode in modelNodes)
            {
                TreeNode treeNode = new TreeNode(modelNode.Description);
                treeNode.Tag = modelNode;

                if (modelNode.IsError)
                {
                    treeNode.BackColor = Color.Red;
                }
                else
                {
                    // Proceed adding child nodes
                    addXmlNodesToTree(modelNode.ChildNodes, treeNode.Nodes);
                }                

                treeNodeCollection.Add(treeNode);
            }
        }

        private void documentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            XmlModelNode modelNode = (XmlModelNode)e.Node.Tag;

            this.gridContainerPanel.Controls.Clear();
            
            if (!modelNode.IsError)
            {                
                currentValuesList = modelNode.Values;
                if (currentValuesList.Count > 0)
                {
                    DataGridView valuesGridView = prepareValuesGridView();
                    valuesGridView.Visible = true;
                    valuesGridView.DataSource = currentValuesList;
                }                

                createGridForDataTables(modelNode.DataTables);
            }
        }

        private void createGridForDataTables(List<XmlModelDataTable> dataTables)
        {
            // Cleanup existing controls
            foreach (Control ctl in this.gridContainerPanel.Controls)
            {
                if (ctl.Name.StartsWith("dataTableLabel_") || ctl.Name.StartsWith("dataTableGrid_"))
                {
                    this.gridContainerPanel.Controls.Remove(ctl);
                }
            }
            this.gridContainerPanel.Refresh();

            int index = 0;
            foreach (XmlModelDataTable dataTable in dataTables)
            {
                Label titleLabel = new Label();
                titleLabel.Name = "dataTableLabel_" + index;
                titleLabel.Text = dataTable.Title;                
                titleLabel.Margin = new Padding (0, 10, 0, 0);
                titleLabel.Font = new Font(titleLabel.Font.FontFamily, titleLabel.Font.Size + 2);
                this.gridContainerPanel.Controls.Add(titleLabel);

                DataGridView dataTableGrid = new DataGridView();
                dataTableGrid.Name = "dataTableGrid_" + index;               
                dataTableGrid.AutoGenerateColumns = true;
                dataTableGrid.AllowUserToAddRows = false;
                dataTableGrid.AllowUserToDeleteRows = false;
                dataTableGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));

                dataTableGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
                dataTableGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                
                dataTableGrid.ReadOnly = true;
                dataTableGrid.RowHeadersVisible = false;
                dataTableGrid.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridView_resize_on_DataBindingComplete);
                dataTableGrid.DataSource = new DictionaryTypedList(dataTable.Records);

                this.gridContainerPanel.Controls.Add(dataTableGrid);

                index++;
            }         
        }
        
        private void valuesGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (currentValuesList == null) return;
            if (e.ColumnIndex == 0) return; // Format only the second column (value)

            if (currentValuesList[e.RowIndex].IsError)
            {
                e.CellStyle.BackColor = Color.Red;
            }
        }

        private void gridView_resize_on_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Resize component to fit all the rows
            // (with some extra pixels to avoid the scroll bar)
            DataGridView grid = (DataGridView)sender;
            int height = grid.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            int width = grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);

            Size clientSize = new Size(width, height);
            Size componentMinSize = new Size(width + 20, height + 20);
            grid.ClientSize = clientSize;
            grid.MinimumSize = componentMinSize;
        }


    }
}
