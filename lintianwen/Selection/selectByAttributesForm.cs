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
    public partial class selectByAttributesForm : Form
    {
        private IMap pMap; //the map in main mapControl
        private IFeatureLayer pSelectedFeatureLayer;
        private string pSelectedFeildName;
        private ToolStripStatusLabel lblWorking;
        private ToolStripProgressBar bar;

        public selectByAttributesForm(IMap mainMap, ToolStripStatusLabel labelWorking, ToolStripProgressBar progressBar)
        {
            InitializeComponent();
            pMap = mainMap;
            btnGetValuesUnique.Enabled = false;
            lblWorking = labelWorking;
            bar = progressBar;
        }

        #region form events
        //init form and the elems...
        private void selectByAttributesForm_Load(object sender, EventArgs e)
        {
            //reset the cobSelectedLayer
            cobSelectedLayer.Items.Clear();

            try
            {
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    //is composite layer?
                    var pLayer = pMap.get_Layer(i);
                    if (pLayer is IRasterLayer)
                        continue;
                    else if (pLayer is IGroupLayer)
                    {
                        ICompositeLayer pCompositeLayer = pLayer as ICompositeLayer;
                        for (int j = 0; j < pCompositeLayer.Count; j++)
                        {
                            if (i == 0)
                                pSelectedFeatureLayer = pLayer as IFeatureLayer;
                            cobSelectedLayer.Items.Add(pCompositeLayer.get_Layer(j).Name);
                        }
                    }
                    else
                    {
                        if (i == 0)
                            pSelectedFeatureLayer = pLayer as IFeatureLayer;
                        cobSelectedLayer.Items.Add(pLayer.Name);
                    }
                }
                if (cobSelectedLayer.Items.Count > 0)
                    cobSelectedLayer.SelectedIndex = 0;
                cobSelectedFunctions.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化失败\n" + ex.Message);
            }
        }

        //keyowrds keyboard buttons event
        private void btn_cnt_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                rtbSQL.Text += " " + (sender as Button).Text;
            }
        }

        //cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //clea
        private void btnClean_Click(object sender, EventArgs e)
        {
            rtbSQL.Text = "";
        }

        //change the layer selection
        private void cobSelectedLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //reset two lists about last layer
            listFields.Items.Clear();
            listValues.Items.Clear();

            //change the selectes layer
            pSelectedFeatureLayer = MapAlgo.GetLayerFromName(pMap, cobSelectedLayer.SelectedItem.ToString());

            //change the content of fields list
            for (int i = 0; i < pSelectedFeatureLayer.FeatureClass.Fields.FieldCount; i++)
            {
                IField field = pSelectedFeatureLayer.FeatureClass.Fields.get_Field(i);
                //except for "SHAPE" field
                if (field.Name.ToUpper() != "SHAPE")
                    listFields.Items.Add("\"" + field.Name + "\"");
            }

            //reset the WHERE statement
            rtbSQL.Text = "";
        }

        //change the field selection
        private void listFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            //reset the list of values
            listValues.Items.Clear();

            //change the selected field name
            btnGetValuesUnique.Enabled = true;
            pSelectedFeildName = listFields.SelectedItem.ToString();
            pSelectedFeildName = pSelectedFeildName.Substring(1);
            pSelectedFeildName = pSelectedFeildName.Substring(0, pSelectedFeildName.Length - 1);
        }

        //get unique value of a field to show
        private void btnGetValuesUnique_Click(object sender, EventArgs e)
        {
            try
            {
                IDataset pDataset = pSelectedFeatureLayer.FeatureClass as IDataset;
                IQueryDef pQueryDef = (pDataset.Workspace as IFeatureWorkspace).CreateQueryDef(); //define a query searcher
                pQueryDef.Tables = pDataset.Name; //set the searched form name
                pQueryDef.SubFields = "DISTINCT (" + pSelectedFeildName + ")";
                ICursor cursor = pQueryDef.Evaluate(); //store result with cursor

                //transform to field
                IFields fields = pSelectedFeatureLayer.FeatureClass.Fields;
                IField field = fields.get_Field(fields.FindField(pSelectedFeildName));

                //add to values list
                IRow row = cursor.NextRow();
                while (row != null)
                {
                    if (field.Type == esriFieldType.esriFieldTypeString)
                    {
                        listValues.Items.Add("\'" + row.get_Value(0).ToString() + "\'");
                    }
                    else
                    {
                        listValues.Items.Add(row.get_Value(0).ToString());
                    }
                    row = cursor.NextRow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("似乎无法获取该字段的唯一值\n" + ex.Message);
            }
        }

        //add text to ...
        private void listFields_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            rtbSQL.Text += " " + listFields.SelectedItem.ToString();
        }
        private void listValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            rtbSQL.Text += " " + listValues.SelectedItem.ToString();
        }

        //trigger for querying
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                lblWorking.Visible = true;
                bar.Visible = true;
                bar.Value = 10;
                SQLRunner();
                bar.Value = 100;
                this.Close();
                lblWorking.Visible = false;
                bar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL语句出错\n" + ex.Message);
            }
        }

        //trigger for query without closing
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                lblWorking.Visible = true;
                bar.Visible = true;
                bar.Value = 10;
                SQLRunner();
                bar.Value = 100;
                lblWorking.Visible = false;
                bar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL语句出错\n" + ex.Message);
            }
        }
        #endregion

        #region query runner
        private void SQLRunner()
        {
            IFeatureSelection featureSelection = pSelectedFeatureLayer as IFeatureSelection;
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = rtbSQL.Text;
            IActiveView activeView = pMap as IActiveView;

            //querying...
            bar.Value = 20;
            switch (cobSelectedFunctions.SelectedIndex)
            {
                case 0:
                    pMap.ClearSelection();
                    featureSelection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    break;
                case 1:
                    featureSelection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);
                    break;
                case 2:
                    featureSelection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultXOR, false);
                    break;
                case 3:
                    featureSelection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultAnd, false);
                    break;
                default:
                    pMap.ClearSelection();
                    featureSelection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    break;
            }

            //partial refresh main mapControl
            bar.Value = 30;
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, activeView.Extent);
            bar.Value = 70;
        }
        #endregion
    }
}
