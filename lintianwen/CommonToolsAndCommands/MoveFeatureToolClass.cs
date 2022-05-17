using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using System;

namespace lintianwen.CommonToolsAndCommands
{
    public class MoveFeatureToolClass : ICommand, ITool
    {
        private IMap m_Map = null;
        private bool bEnable = true;
        private IPoint m_startPt = null;
        private IPoint m_EndPoint = null;
        private IHookHelper m_hookHelper = null;
        private IActiveView m_activeView = null;
        private IEngineEditor m_EngineEditor = null;
        private IEngineEditLayers m_EngineEditLayers = null;
        private IMoveGeometryFeedback m_moveGeoFeedBack = null;

        #region ICommand members

        public int Bitmap
        {
            get { return -1; }
        }

        public string Caption
        {
            get { return "移动要素"; }
        }

        public string Category
        {
            get { return "编辑工具"; }
        }

        public bool Checked
        {
            get { return false; }
        }

        public bool Enabled
        {
            get { return bEnable; }
        }

        public int HelpContextID
        {
            get { return -1; }
        }

        public string HelpFile
        {
            get { return ""; }
        }

        public string Message
        {
            get { return "整体移动要素"; }
        }

        public string Name
        {
            get { return "MoveTool"; }
        }

        public void OnClick()
        {
            m_Map = m_hookHelper.FocusMap;
            m_activeView = m_Map as IActiveView;
            m_EngineEditor = MapAlgo.EngineEditor;
            m_EngineEditLayers = MapAlgo.EngineEditor as IEngineEditLayers;

            EditVertexClass.ClearResource();
        }

        public void OnCreate(object Hook)
        {
            if (Hook == null) return;
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = Hook;
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                bEnable = false;
            else
                bEnable = true;
        }

        public string Tooltip
        {
            get { return "整体移动要素"; }
        }

        #endregion

        #region ITool members

        public int Cursor
        {
            get { return -1; }
        }

        public bool Deactivate()
        {
            return true;
        }

        public bool OnContextMenu(int x, int y)
        {
            return false;
        }

        public void OnDblClick()
        {
        }

        public void OnKeyDown(int keyCode, int shift)
        {

        }

        public void OnKeyUp(int keyCode, int shift)
        {

        }

        public void OnMouseDown(int button, int shift, int x, int y)
        {
            try
            {
                m_startPt = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (m_EngineEditor == null) return;
                if (m_EngineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
                if (m_EngineEditLayers == null) return;
                IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
                if (pFeatLyr == null) return;
                IFeatureCursor pFeatCur = MapAlgo.GetSelectedFeatures(pFeatLyr);
                if (pFeatCur == null)
                {
                    MessageBox.Show("请选择要移动要素！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                IFeature pFeature = pFeatCur.NextFeature();
                if (m_moveGeoFeedBack == null)
                    m_moveGeoFeedBack = new MoveGeometryFeedbackClass();
                m_moveGeoFeedBack.Display = m_activeView.ScreenDisplay;
                while (pFeature != null)
                {
                    m_moveGeoFeedBack.AddGeometry(pFeature.Shape);
                    pFeature = pFeatCur.NextFeature();
                }
                m_moveGeoFeedBack.Start(m_startPt);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);
            }
            catch (Exception ex)
            {
            }
        }

        public void OnMouseMove(int button, int shift, int x, int y)
        {
            try
            {
                IPoint pMovePt = null;
                pMovePt = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (m_moveGeoFeedBack == null) return;
                m_moveGeoFeedBack.MoveTo(pMovePt);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void OnMouseUp(int button, int shift, int x, int y)
        {
            try
            {
                m_EndPoint = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (m_moveGeoFeedBack == null) return;
                m_moveGeoFeedBack.MoveTo(m_EndPoint);
                MoveFeatures(m_EndPoint, m_startPt);
                m_moveGeoFeedBack.ClearGeometry();
                m_moveGeoFeedBack = null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void Refresh(int hdc)
        {

        }
        #endregion

        #region functions

        private void MoveFeatures(IPoint lastpoint, IPoint firstpoint)
        {
            if (m_EngineEditor == null) return;
            m_EngineEditor.StartOperation();
            if (m_EngineEditLayers == null) return;
            IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
            if (pFeatLyr == null) return;
            IFeatureCursor pFeatCur = MapAlgo.GetSelectedFeatures(pFeatLyr);
            IFeature pFeature = pFeatCur.NextFeature();
            while (pFeature != null)
            {
                MoveFeature(pFeature, lastpoint, firstpoint);
                pFeature = pFeatCur.NextFeature();
            }
            m_EngineEditor.StopOperation("MoveTool");
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection | esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void MoveFeature(IFeature pFeature, IPoint lastpoint, IPoint firstpoint)
        {
            double deltax; double deltay;
            IGeoDataset pGeoDataSet;
            ITransform2D transform;
            IGeometry pGeometry;
            IFeatureClass pClass = pFeature.Class as IFeatureClass;
            pGeoDataSet = pClass as IGeoDataset;

            pGeometry = pFeature.Shape;
            if (pGeometry.GeometryType == esriGeometryType.esriGeometryMultiPatch
                || pGeometry.GeometryType == esriGeometryType.esriGeometryPoint
                || pGeometry.GeometryType == esriGeometryType.esriGeometryPolyline
                || pGeometry.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                pGeometry = pFeature.Shape;
                transform = pGeometry as ITransform2D;
                if (!MapAlgo.Distance(lastpoint, firstpoint, out deltax, out deltay))
                {
                    MessageBox.Show("计算距离出现错误", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                transform.Move(deltax, deltay);
                pGeometry = (IGeometry)transform;
                if (pGeoDataSet.SpatialReference != null)
                {
                    pGeometry.Project(pGeoDataSet.SpatialReference);
                }
                pFeature.Shape = SupportZMFeatureClass.ModifyGeomtryZMValue(pClass, pGeometry);
                pFeature.Store();
            }
        }
        #endregion
    }
}
