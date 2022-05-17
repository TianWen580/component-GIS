using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;


namespace lintianwen.Spatial_Analysis
{
    public sealed class ToolGetNearFeature : BaseTool
    {
        private IHookHelper m_hookHelper = null;

        public ToolGetNearFeature()
        {
            base.m_category = "矢量分析"; //localizable text 
            base.m_caption = "获取邻接要素";  //localizable text 
            base.m_message = "查找任一多边形要素的所有邻接要素";  //localizable text
            base.m_toolTip = "查找任一多边形要素的所有邻接要素";  //localizable text
            base.m_name = "ToolGetNearFeature";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        public override void OnCreate(object hook)
        {
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;
        }

        public override void OnClick()
        {
            if (m_hookHelper.FocusMap.LayerCount <= 0)
            {
                MessageBox.Show("请先加载图层数据！");
                return;
            }
            (this.m_hookHelper.Hook as MapControl).MousePointer = esriControlsMousePointer.esriPointerHand;
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1 || m_hookHelper.FocusMap.LayerCount <= 0)
                return;
            IMap pMap = m_hookHelper.FocusMap;
            IActiveView pActiveView = m_hookHelper.ActiveView;
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            IFeatureLayer pFeatureLayer = m_hookHelper.FocusMap.get_Layer(0) as IFeatureLayer;
            if (pFeatureLayer == null)
            {
                return;
            }
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pPoint;
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor featureCursor = pFeatureClass.Search(pSpatialFilter, false);
            IFeature pFeature = featureCursor.NextFeature();
            if (pFeature != null && pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                pMap.ClearSelection();
                IRelationalOperator pRelationalOperator = pFeature.Shape as IRelationalOperator;
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 255;
                IFeatureSelection pFeatueSelection = pFeatureLayer as IFeatureSelection;
                pFeatueSelection.SelectionColor = pColor;
                IFeatureCursor pCompFeaureCursor = pFeatureLayer.Search(null, false);
                IFeature pCompFeature = pCompFeaureCursor.NextFeature();
                while (pCompFeature != null)
                {
                    if (pRelationalOperator.Touches(pCompFeature.Shape))
                    {
                        pMap.SelectFeature(pFeatureLayer, pCompFeature);
                    }
                    pCompFeature = pCompFeaureCursor.NextFeature();
                }
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
        }
        #endregion
    }
}
