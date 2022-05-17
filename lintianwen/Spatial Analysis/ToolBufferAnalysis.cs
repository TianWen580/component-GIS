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

    /// <summary>
    /// Summary description for ToolBufferAnalysis.
    /// </summary>
    public sealed class ToolBufferAnalysis : BaseTool
    {
        #region COM Registration Function(s)
        static void RegisterFunction(Type registerType)
        {
            ArcGISCategoryRegistration(registerType);
        }

        static void UnregisterFunction(Type registerType)
        {
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;

        public ToolBufferAnalysis()
        {
            base.m_category = "矢量分析"; //localizable text 
            base.m_caption = "缓冲区分析";  //localizable text 
            base.m_message = "点击选择要素生成缓冲区";  //localizable text
            base.m_toolTip = "点击选择要素生成缓冲区";  //localizable text
            base.m_name = "ToolBufferAnalysis";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
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

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
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

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
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
            IActiveView pActiveView = m_hookHelper.ActiveView;
            IGraphicsContainer pGraCont = (IGraphicsContainer)pActiveView;
            pGraCont.DeleteAllElements();
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
                IGeometry pGeometry = pFeature.Shape as IGeometry;
                ITopologicalOperator pTopoOpe = (ITopologicalOperator)pGeometry;
                pTopoOpe.Simplify();
                IGeometry pBufferGeo = pTopoOpe.Buffer(5000);
                IScreenDisplay pdisplay = pActiveView.ScreenDisplay;
                ISimpleFillSymbol pSymbol = new SimpleFillSymbolClass();
                pSymbol.Style = esriSimpleFillStyle.esriSFSCross;
                RgbColor pColor = new RgbColorClass();
                pColor.Blue = 200;
                pColor.Red = 211;
                pColor.Green = 100;
                pSymbol.Color = (IColor)pColor;
                IFillShapeElement pFillShapeElm = new PolygonElementClass();
                IElement pElm = (IElement)pFillShapeElm;
                pElm.Geometry = pBufferGeo;
                pFillShapeElm.Symbol = pSymbol;
                pGraCont.AddElement((IElement)pFillShapeElm, 0);
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
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
