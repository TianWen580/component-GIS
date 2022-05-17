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
    public partial class attributesStatisticForm : Form
    {
        IMap pMap;
        IFeatureLayer pSelectedFeatureLyr;

        public attributesStatisticForm(IMap mainMap)
        {
            InitializeComponent();
            pMap = mainMap;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void attributesStatisticForm_Load(object sender, EventArgs e)
        {
            //clear for coblist
            cobSelectedLayer.Items.Clear();
            cobSelectedField.Items.Clear();
            
            //init list of selected layer
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                var pLayer = pMap.get_Layer(i);
                if (pLayer is IRasterLayer)
                    continue;
                if (pLayer is ICompositeLayer || pLayer is IGroupLayer)
                {
                    ICompositeLayer pCompositeLayer = pLayer as ICompositeLayer;
                    for (int j = 0; j < pCompositeLayer.Count; j++)
                    {
                        cobSelectedLayer.Items.Add(pCompositeLayer.get_Layer(j).Name);
                    }
                }
                else
                    cobSelectedLayer.Items.Add(pLayer.Name);
            }
        }

        private void cobSelectedLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //clear for selected feild
            cobSelectedField.Items.Clear();

            //init list of feild
            if(cobSelectedLayer.SelectedIndex == -1)
                return;
            pSelectedFeatureLyr = MapAlgo.GetLayerFromName(pMap, cobSelectedLayer.SelectedItem.ToString());
            for (int i = 0; i < pSelectedFeatureLyr.FeatureClass.Fields.FieldCount; i++)
            {
                IField pField = pSelectedFeatureLyr.FeatureClass.Fields.get_Field(i);
                if (pField.Type == esriFieldType.esriFieldTypeInteger || pField.Type == esriFieldType.esriFieldTypeDouble
               || pField.Type == esriFieldType.esriFieldTypeSingle || pField.Type == esriFieldType.esriFieldTypeSmallInteger)
                {
                    if (pField.Name.ToUpper() != "OBJECTID" && pField.Name.ToUpper() != "SHAPE")
                        cobSelectedField.Items.Add(pField.Name);
                }
            }

            //default selection
            if(cobSelectedField.Items.Count>0)
                cobSelectedField.SelectedIndex = 0;
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Field = cobSelectedField.SelectedItem.ToString();
            IFeatureSelection featureSelection = pSelectedFeatureLyr as IFeatureSelection;
            ICursor cursor = null;
            featureSelection.SelectionSet.Search(null, false, out cursor);
            dataStatistics.Cursor = cursor;
            IStatisticsResults statisticsResults = dataStatistics.Statistics;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("统计总数： " + statisticsResults.Count.ToString());
            stringBuilder.AppendLine("最小值：" + statisticsResults.Minimum.ToString());
            stringBuilder.AppendLine("最大值：" + statisticsResults.Maximum.ToString());
            stringBuilder.AppendLine("总计： " + statisticsResults.Sum.ToString());
            stringBuilder.AppendLine("平均值： " + statisticsResults.Mean.ToString());
            stringBuilder.AppendLine("标准差： " + statisticsResults.StandardDeviation.ToString());
            labelStatisticsResult.Text = stringBuilder.ToString();
        }
    }
}
