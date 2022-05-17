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
    public partial class selectBySpatialRelationshipForm : Form
    {
        IMap pMap;
        private ToolStripStatusLabel lblWorking;
        private ToolStripProgressBar bar;

        public selectBySpatialRelationshipForm(IMap mainMap, ToolStripStatusLabel labelWorking, ToolStripProgressBar progressBar)
        {
            InitializeComponent();
            pMap = mainMap;
            lblWorking = labelWorking;
            bar = progressBar;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region form events
        //init form and the elems...
        private void selectBySpatialRelationshipForm_Load(object sender, EventArgs e)
        {
            //reset the lists
            clTargetLayer.Items.Clear();
            cobSourceLayer.Items.Clear();

            for (int i = 0; i < pMap.LayerCount; i++)
            {
                var pLayer = pMap.get_Layer(i);

                //add feature layers name to checkList
                if (pLayer is IRasterLayer)
                    continue;
                else if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
                {
                    ICompositeLayer pCompositeLayer = pLayer as ICompositeLayer;

                    for (int j = 0; j < pCompositeLayer.Count; j++)
                    {
                        string pLayerName = pCompositeLayer.get_Layer(j).Name;
                        clTargetLayer.Items.Add(pLayerName);
                        cobSourceLayer.Items.Add(pLayerName);
                    }
                }
                else
                {
                    clTargetLayer.Items.Add(pLayer.Name);
                    cobSourceLayer.Items.Add(pLayer.Name);
                }
            }

            //default choice in comboBox
            if (clTargetLayer.Items.Count > 0)
            {
                cobSourceLayer.SelectedIndex = 0;
                cobSpatialMethod.SelectedIndex = 0;
            }
        }

        //trigger for query
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                lblWorking.Visible = true;
                bar.Visible = true;
                bar.Value = 10;
                SpatialQueryRunner();
                this.Close();
                bar.Value = 100;
                lblWorking.Visible = false;
                bar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出错\n" + ex.Message);
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
                SpatialQueryRunner();
                bar.Value = 100;
                lblWorking.Visible = false;
                bar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出错\n" + ex.Message);
            }
        }

        //reset everything
        private void btnReset_Click(object sender, EventArgs e)
        {
            pMap.ClearSelection();
            IActiveView pActivate = pMap as IActiveView;
            pActivate.Refresh();
            selectBySpatialRelationshipForm_Load(sender, e);
        }
        #endregion

        #region query runner
        

        /// <summary>
        /// union the feature classes and return as IGeometry
        /// </summary>
        /// <param name="pFeatureLayer">input feature layer</param>
        /// <returns>IGeometry</returns>
        private IGeometry UnionFeatureClasses(IFeatureLayer pFeatureLayer)
        {
            IGeometry pGeometry = null;
            IFeatureCursor pCursor = pFeatureLayer.Search(null, false);
            IFeature pThisFeatrue = pCursor.NextFeature();
            while (pThisFeatrue != null)
            {
                if (pGeometry == null)
                    pGeometry = pThisFeatrue.Shape;
                else
                {
                    ITopologicalOperator pTopoOpra = pGeometry as ITopologicalOperator;
                    pGeometry = pTopoOpra.Union(pThisFeatrue.Shape);
                }
                pThisFeatrue = pCursor.NextFeature();
            }
            return pGeometry;
        }

        //query runner
        private void SpatialQueryRunner()
        {
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = UnionFeatureClasses(MapAlgo.GetLayerFromName(pMap, cobSourceLayer.SelectedItem.ToString()));

            //choice a spaitial relationship to do spaitial query
            bar.Value = 20;
            switch (cobSpatialMethod.SelectedIndex)
            {
                case 0:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
                case 1:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    break;
                case 2:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case 3:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    break;
                case 4:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                    break;
                case 5:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                default:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }

            //querying...
            bar.Value = 30;
            int cntSelectedLayers = clTargetLayer.CheckedItems.Count;
            int batch = (int)((90.0 - 30.0) / cntSelectedLayers);
            for (int i = 0; i < cntSelectedLayers; i++)
            {
                IFeatureLayer pFeatureLayer = MapAlgo.GetLayerFromName(pMap, (clTargetLayer.CheckedItems[i]) as string);
                IFeatureSelection pSelectionLayer = pFeatureLayer as IFeatureSelection;
                pSelectionLayer.SelectFeatures(pSpatialFilter as IQueryFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);
                bar.Value += batch;
            }

            //refresh mapControl viewing
            IActiveView pActivate = pMap as IActiveView;
            pActivate.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActivate.Extent);
        }
        #endregion
    }
}
