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
using DOXView.GUI.DictionaryToGridAdapter;

namespace DOXView.GUI
{
    public partial class MainForm : Form
    {
        private List<XmlModelValue> currentValuesList;

        public MainForm()
        {
            InitializeComponent();
        }
        
        private void openXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.ShowDialog(this);


            // * * * Temporary * * * 
            // * * * A file will not be open during startup
            LayoutParser layoutParser = new LayoutParser();

            // The IDE is supposed to run this program from [DoxViewTests directory]/bin/[Debug|Release]
            // We go back three levels to find the sample files in the root of the project
            Layout layout = layoutParser.parseXmlFile("../../../DemoFiles/SampleLayout.xml");

            ModelParser parser = new ModelParser(layout);
            XmlModel model;

            try
            {
                model = parser.parseXmlFile("../../../DemoFiles/SampleData.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "|" + ex.StackTrace);
                return;
            }

            addXmlNodesToTree(model.Nodes, documentTreeView.Nodes);
            documentTreeView.SelectedNode = documentTreeView.Nodes[0];
        }

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close ();
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
            prepareValuesGridView();
        }

        private void prepareValuesGridView()
        {

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

            valuesGridView.AutoGenerateColumns = false;
            valuesGridView.Columns.Add(colDescription);
            valuesGridView.Columns.Add(colValue);
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
            currentValuesList = modelNode.Values;
            if (currentValuesList.Count > 0)
            {
                valuesGridView.Visible = true;
                valuesGridView.DataSource = currentValuesList;
            }
            else
            {
                valuesGridView.Visible = false;
            }            

            createGridForDataTables(modelNode.DataTables);
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

            int index = 0;
            foreach (XmlModelDataTable dataTable in dataTables)
            {
                Label titleLabel = new Label();
                titleLabel.Name = "dataTableLabel_" + index;
                titleLabel.Text = dataTable.Title;
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

        private void cleanupDatatableGrids()
        {
            throw new NotImplementedException();
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
