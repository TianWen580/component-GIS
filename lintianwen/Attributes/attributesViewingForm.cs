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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lintianwen.Selection
{
    public partial class attributesViewingForm : Form
    {
        #region constuct row col structure
        public struct RowAndCol
        {
            private int row;
            private int column;
            private string _value;

            public int Row
            {
                get
                {
                    return row;
                }
                set
                {
                    row = value;
                }
            }
            public int Column
            {
                get
                {
                    return column;
                }
                set
                {
                    column = value;
                }
            }
            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
        }
        #endregion

        private IActiveView m_activeView;
        private IFeatureLayer _curFeatureLayer = null;
        public IFeatureLayer CurFeatureLayer
        {
            get { return _curFeatureLayer; }
            set { _curFeatureLayer = value; }
        }
        private RowAndCol[] pRowAndCol = new RowAndCol[10000];
        int count = 0;
        int row_index;

        public attributesViewingForm(IActiveView pActivateView)
        {
            InitializeComponent();
            m_activeView = pActivateView;
        }

        private void attributesViewingForm_Load(object sender, EventArgs e)
        {
            dgvAttributes.ReadOnly = true;
            btnFinish.Enabled = false;

            if (_curFeatureLayer == null) return;

            IFeature pFeature = null;
            DataTable pFeatDT = new DataTable();
            DataRow pDataRow = null;
            DataColumn pDataCol = null;
            IField pField = null;
            for (int i = 0; i < _curFeatureLayer.FeatureClass.Fields.FieldCount; i++)
            {
                pDataCol = new DataColumn();
                pField = _curFeatureLayer.FeatureClass.Fields.get_Field(i);
                pDataCol.ColumnName = pField.AliasName;
                pDataCol.DataType = Type.GetType("System.Object");
                pFeatDT.Columns.Add(pDataCol); 
            }

            IFeatureCursor pFeatureCursor = _curFeatureLayer.Search(null, true);
            while ((pFeature = pFeatureCursor.NextFeature()) != null)
            {
                pDataRow = pFeatDT.NewRow();
                for (int k = 0; k < pFeatDT.Columns.Count; k++)
                {
                    pDataRow[k] = pFeature.get_Value(k);
                }

                pFeatDT.Rows.Add(pDataRow); 
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
            dgvAttributes.DataSource = pFeatDT;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            dgvAttributes.ReadOnly = false;
            btnFinish.Enabled = true;
            this.dgvAttributes.CurrentCell = this.dgvAttributes.Rows[this.dgvAttributes.Rows.Count - 2].Cells[0];
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            dgvAttributes.ReadOnly = true;
            IFeatureClass pFeatureClass = _curFeatureLayer.FeatureClass;
            ITable pTable;
            pTable = _curFeatureLayer as ITable;

            try
            {
                int i = 0;
                while (pRowAndCol[i].Column != 0 || pRowAndCol[i].Row != 0)
                {
                    IRow pRow;
                    pRow = pTable.GetRow(pRowAndCol[i].Row);
                    pRow.set_Value(pRowAndCol[i].Column, pRowAndCol[i].Value);
                    pRow.Store();
                    i++;
                }
                count = 0;
                for (int j = 0; j < i; j++)
                {
                    pRowAndCol[j].Row = 0;
                    pRowAndCol[j].Column = 0;
                    pRowAndCol[j].Value = null;
                }
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存出现问题\n" + ex.Message);
            }
            finally
            {
                btnFinish.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (((MessageBox.Show("确定要删除吗", "警告", MessageBoxButtons.YesNo)) == DialogResult.Yes))
            {
                try
                {
                    ITable pTable = _curFeatureLayer as ITable;
                    IRow pRow = pTable.GetRow(row_index);
                    pRow.Delete();
                    MessageBox.Show("删除成功！", "提示", MessageBoxButtons.OK);
                    m_activeView.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除出现问题\n" + ex.Message);
                }
            }

        }

        private void dgvAttributes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            row_index = e.RowIndex;
        }

        private void dgvAttributes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            pRowAndCol[count].Row = dgvAttributes.CurrentCell.RowIndex;
            pRowAndCol[count].Column = dgvAttributes.CurrentCell.ColumnIndex;
            pRowAndCol[count].Value = dgvAttributes.Rows[dgvAttributes.CurrentCell.RowIndex].Cells[dgvAttributes.CurrentCell.ColumnIndex].Value.ToString();
            count++;
        }

        private void btnLocate_Click(object sender, EventArgs e)
        {
            IQueryFilter pQuery = new QueryFilterClass();
            int count = this.dgvAttributes.SelectedRows.Count;
            string val;
            string col;
            col = this.dgvAttributes.Columns[0].Name;

            try
            {
                if (count == 1)
                {
                    val = this.dgvAttributes.SelectedRows[0].Cells[col].Value.ToString();
                    pQuery.WhereClause = col + "=" + val;
                }
                else
                {
                    int i;
                    string str;
                    for (i = 0; i < count - 1; i++)
                    {
                        val = this.dgvAttributes.SelectedRows[i].Cells[col].Value.ToString();
                        str = col + "=" + val + " OR ";
                        pQuery.WhereClause += str;
                    }
                    val = this.dgvAttributes.SelectedRows[i].Cells[col].Value.ToString();
                    str = col + "=" + val;
                    pQuery.WhereClause += str;
                }
                IFeatureSelection pFeatSelection;
                pFeatSelection = _curFeatureLayer as IFeatureSelection;
                pFeatSelection.SelectFeatures(pQuery, esriSelectionResultEnum.esriSelectionResultNew, false);
                m_activeView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("请按行选择要定位的元素\n" + ex.Message);
            }
        }
    }
}
