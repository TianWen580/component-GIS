﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using lintianwen.CommonToolsAndCommands;



namespace lintianwen.CommonToolsAndCommands
{
    public class CreateFeatureToolClass : ICommand, ITool
    {
        private IMap m_Map = null;
        private bool bEnable = true;
        private IHookHelper m_hookHelper = null;
        private IActiveView m_activeView = null;
        private IEngineEditor m_EngineEditor = null;
        private IEngineEditLayers m_EngineEditLayers = null;
        private IPointCollection m_pointCollection;
        private INewLineFeedback m_newLineFeedBack;
        private INewPolygonFeedback m_newPolyFeedBack;
        private INewMultiPointFeedback m_newMultPtFeedBack;


        #region ICommand members

        public int Bitmap
        {
            get { return -1; }
        }

        public string Caption
        {
            get { return "添加要素"; }
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
            get { return "添加要素"; }
        }

        public string Name
        {
            get { return "SketchTool"; }
        }

        public void OnClick()
        {
            m_Map = m_hookHelper.FocusMap;
            m_activeView = m_Map as IActiveView;
            m_EngineEditor = MapAlgo.EngineEditor;
            m_EngineEditLayers = MapAlgo.EngineEditor as IEngineEditLayers;
        }

        public void OnCreate(object Hook)
        {
            EditVertexClass.ClearResource();

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
            get { return "添加要素"; }
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
            IGeometry pResultGeometry = null;
            if (m_EngineEditLayers == null) return;
            IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
            if (pFeatLyr == null) return;
            IFeatureClass pFeatCls = pFeatLyr.FeatureClass;
            if (pFeatCls == null) return;

            switch (pFeatCls.ShapeType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                    m_newMultPtFeedBack.Stop();
                    pResultGeometry = m_pointCollection as IGeometry;
                    m_newMultPtFeedBack = null;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    IPolyline pPolyline = null;
                    pPolyline = m_newLineFeedBack.Stop();
                    pResultGeometry = pPolyline as IGeometry;
                    m_newLineFeedBack = null;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    IPolygon pPolygon = null;
                    pPolygon = m_newPolyFeedBack.Stop();
                    pResultGeometry = pPolygon as IGeometry;
                    m_newPolyFeedBack = null;
                    break;
            }

            IZAware pZAware = pResultGeometry as IZAware;
            pZAware.ZAware = true;
            CreateFeature(pResultGeometry);
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
                IPoint pPt = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (m_EngineEditor == null) return;
                if (m_EngineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
                if (m_EngineEditLayers == null) return;
                IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
                if (pFeatLyr == null) return;
                IFeatureClass pFeatCls = pFeatLyr.FeatureClass;
                if (pFeatCls == null) return;

                IZAware pZAware = pPt as IZAware;
                pZAware.ZAware = true;
                pPt.Z = 0;

                object missing = Type.Missing;

                m_Map.ClearSelection();

                switch (pFeatCls.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        CreateFeature(pPt as IGeometry);
                        break;
                    case esriGeometryType.esriGeometryMultipoint:
                        if (m_pointCollection == null)
                        {
                            m_pointCollection = new MultipointClass();
                        }
                        else
                        {
                            m_pointCollection.AddPoint(pPt, ref missing, ref missing);
                        }
                        if (m_newMultPtFeedBack == null)
                        {
                            m_newMultPtFeedBack = new NewMultiPointFeedbackClass();
                            m_newMultPtFeedBack.Display = m_activeView.ScreenDisplay;
                            m_newMultPtFeedBack.Start(m_pointCollection, pPt);
                        }
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        if (m_newLineFeedBack == null)
                        {
                            m_newLineFeedBack = new NewLineFeedbackClass();
                            m_newLineFeedBack.Display = m_activeView.ScreenDisplay;
                            m_newLineFeedBack.Start(pPt);
                        }
                        else
                        {
                            m_newLineFeedBack.AddPoint(pPt);
                        }
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        if (m_newPolyFeedBack == null)
                        {
                            m_newPolyFeedBack = new NewPolygonFeedbackClass();
                            m_newPolyFeedBack.Display = m_activeView.ScreenDisplay;
                            m_newPolyFeedBack.Start(pPt);
                        }
                        else
                        {
                            m_newPolyFeedBack.AddPoint(pPt);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void OnMouseMove(int button, int shift, int x, int y)
        {
            IPoint pPt = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
            if (m_EngineEditLayers == null) return;
            IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
            if (pFeatLyr == null) return;
            IFeatureClass pFeatCls = pFeatLyr.FeatureClass;
            if (pFeatCls == null) return;
            switch (pFeatCls.ShapeType)
            {
                case esriGeometryType.esriGeometryPolyline:
                    if (m_newLineFeedBack != null)
                        m_newLineFeedBack.MoveTo(pPt);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    if (m_newPolyFeedBack != null)
                        m_newPolyFeedBack.MoveTo(pPt);
                    break;
            }
        }

        public void OnMouseUp(int button, int shift, int x, int y)
        {

        }

        public void Refresh(int hdc)
        {

        }

        #endregion

        #region functions

        /// <summary>
        /// create features
        /// </summary>
        /// <param name="pGeometry"></param>
        private void CreateFeature(IGeometry pGeometry)
        {
            try
            {
                if (m_EngineEditLayers == null) return;
                IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
                if (pFeatLyr == null) return;
                IFeatureClass pFeatCls = pFeatLyr.FeatureClass;
                if (pFeatCls == null) return;
                if (m_EngineEditor == null) return;
                if (pGeometry == null) return;
                ITopologicalOperator pTop = pGeometry as ITopologicalOperator;
                pTop.Simplify();
                IGeoDataset pGeoDataset = pFeatCls as IGeoDataset;
                if (pGeoDataset.SpatialReference != null)
                {
                    pGeometry.Project(pGeoDataset.SpatialReference);
                }
                m_EngineEditor.StartOperation();
                IFeature pFeature = null;
                pFeature = pFeatCls.CreateFeature();
                pFeature.Shape = SupportZMFeatureClass.ModifyGeomtryZMValue(pFeatCls, pGeometry);
                pFeature.Store();
                m_EngineEditor.StopOperation("添加要素");
                m_Map.SelectFeature(pFeatLyr, pFeature);
                m_activeView.Refresh();
            }
            catch (Exception ex)
            {

            }
        }



        #endregion


    }
}
