using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lintianwen.Selection
{
    public partial class attributeEditForm : Form
    {
        #region def vars
        IFeature pFeature = null;
        private DataTable pFeatTable = null;
        private Hashtable _FeatHashtable = null;

        private IEngineEditor _gisEdit;
        public IEngineEditor GisEdit
        {
            get { return _gisEdit; }
            set { _gisEdit = value; }
        }

        private List<IFeature> _lstFeature;
        public List<IFeature> LstFeature
        {
            get { return _lstFeature; }
            set { _lstFeature = value; }
        }

        private IFeatureLayer _featLyr;
        public IFeatureLayer FeatLyr
        {
            get { return _featLyr; }
            set { _featLyr = value; }
        }
        #endregion

        public attributeEditForm()
        {
            InitializeComponent();
            pFeatTable = new DataTable();
            InitTable(pFeatTable);
        }

        private void InitTable(DataTable pTable)
        {
            DataColumn pDataColumn = new DataColumn();
            pDataColumn.ColumnName = "字段名称";
            pDataColumn.DataType = System.Type.GetType("System.String");
            pTable.Columns.Add(pDataColumn);

            pDataColumn = new DataColumn();
            pDataColumn.ColumnName = "字段值";
            pDataColumn.DataType = System.Type.GetType("System.Object");
            pTable.Columns.Add(pDataColumn);
        }

        public void InitTreeView()
        {
            try
            {
                int pDisPlayFieldIndex = -1;
                _FeatHashtable = new Hashtable();
                string sFieldValue = string.Empty;
                string sDisPlayName = string.Empty;
                TreeNode pRootNode = new TreeNode();
                TreeNode pNode = null; IFeature pFeat;
                if (_featLyr == null) return;
                pFeatTable.Rows.Clear();
                tvLayer.Nodes.Clear();
                tvLayer.ExpandAll();
                pRootNode.Text = _featLyr.Name.ToString();
                pRootNode.ExpandAll();
                tvLayer.Nodes.Add(pRootNode);

                sDisPlayName = _featLyr.DisplayField;
                pDisPlayFieldIndex = _featLyr.FeatureClass.Fields.FindField(sDisPlayName);
                for (int i = 0; i < _lstFeature.Count; i++)
                {
                    pNode = new TreeNode();
                    pFeat = _lstFeature[i];
                    sFieldValue = pFeat.get_Value(pDisPlayFieldIndex).ToString();
                    pNode.Text = sFieldValue;
                    pNode.Expand();
                    pRootNode.Nodes.Add(pNode);

                    if (!_FeatHashtable.Contains(pFeat))
                    {
                        _FeatHashtable.Add(pFeat, sFieldValue);
                    }
                }
                tvLayer.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitGridView(TreeNode pNode)
        {
            try
            {
                DataRow pDataRow = null;
                pFeatTable.Rows.Clear();
                string sNodeValue = pNode.Text.ToString();
                foreach (DictionaryEntry de in _FeatHashtable)
                {
                    if (de.Value.ToString().ToUpper() == sNodeValue.ToUpper())
                    {
                        pFeature = de.Key as IFeature;
                        break;
                    }
                }
                if (pFeature != null)
                {
                    for (int i = 0; i < pFeature.Fields.FieldCount; i++)
                    {
                        string strFieldName = pFeature.Fields.get_Field(i).Name;
                        if (strFieldName.ToUpper() == "SHAPE") continue;
                        string strFieldValue = pFeature.get_Value(i).ToString();
                        pDataRow = pFeatTable.NewRow();
                        pDataRow["字段名称"] = strFieldName;
                        pDataRow["字段值"] = strFieldValue;
                        pFeatTable.Rows.Add(pDataRow);
                    }
                }
                dgvAttributes.DataSource = pFeatTable;
                for (int i = 0; i < dgvAttributes.Columns.Count; i++)
                {
                    dgvAttributes.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                dgvAttributes.Columns[0].ReadOnly = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void tvLayer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                TreeNode pFocusedNode = e.Node;
                if (pFocusedNode.Nodes.Count == 0)
                    InitGridView(pFocusedNode);
            }
            catch (Exception ex)
            {


            }
        }

        private void dgvAttributes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (_gisEdit == null) return;
                if (pFeature == null) return;
                _gisEdit.StartOperation();
                int pIndex = dgvAttributes.CurrentCell.RowIndex;
                object sFieldValue = dgvAttributes.Rows[pIndex].Cells[1].Value;
                string sFieldName = dgvAttributes.Rows[pIndex].Cells[0].Value.ToString();
                pFeature.set_Value(pFeature.Fields.FindField(sFieldName), sFieldValue);
                pFeature.Store();
                _gisEdit.StopOperation("属性编辑");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            IEngineEditTask pEngineEditTask = GisEdit.GetTaskByUniqueName("ControlToolsEditing_CreateNewFeatureTask");
            GisEdit.CurrentTask = pEngineEditTask;
            this.Close();
        }
    }
}
