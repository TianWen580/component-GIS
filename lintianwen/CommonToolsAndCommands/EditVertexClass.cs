using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;

namespace lintianwen.CommonToolsAndCommands
{
    internal class EditVertexClass
    {
        public static IActiveView m_activeView = null;
        public static IMap m_Map = null;

        public static bool m_inUse;
        public static ISimpleMarkerSymbol m_vertexSym;
        public static ISimpleMarkerSymbol m_endPointSym;
        public static ISimpleMarkerSymbol m_selPointSym;
        public static ISimpleMarkerSymbol m_markerSym;
        public static ISimpleLineSymbol m_lineSym;
        public static ISimpleFillSymbol m_fillSym;
        public static ISimpleLineSymbol m_tracklineSym;

        public static IGeometryCollection m_vertexGeoBag;
        public static IArray m_featArray;

        public static IPoint pHitPnt = null;


        public static int GetVertexIndex(IPoint pPoint, IGeometry pGeo)
        {
            int functionReturnValue = -2;

            IPointCollection pPointCollection = pGeo as IPointCollection;
            if (pPointCollection == null) return functionReturnValue;

            ITopologicalOperator pTopoOpt = pPointCollection as ITopologicalOperator;
            IRelationalOperator pRelationalOperator = pPoint as IRelationalOperator;
            IProximityOperator pProx = pPoint as IProximityOperator;

            bool pIsEqual = false;
            for (int i = 0; i < pPointCollection.PointCount; i++)
            {
                pIsEqual = pRelationalOperator.Equals(pPointCollection.get_Point(i));
                if (pIsEqual)
                {
                    functionReturnValue = i;
                    break;
                }
            }
            return functionReturnValue;
        }

        public static void SelectByShapeTop(IFeatureLayer pFeaturelayer, IGeometry pGeo, esriSpatialRelEnum SpatialRel, bool blnShow, esriSelectionResultEnum Method)
        {
            ITopologicalOperator pTopo = null;
            m_inUse = blnShow;
            IGeometry pGeometry = default(IGeometry);
            IClone pClone = pGeo as IClone;
            pGeometry = pClone.Clone() as IGeometry;
            IFeatureSelection pFeatureSelection = pFeaturelayer as IFeatureSelection;
            bool pBlnExit = false;
            if (pGeometry == null) pBlnExit = true;
            if (pGeometry.IsEmpty) pBlnExit = true;
            switch (pGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryEnvelope:
                    IEnvelope pEnve = null;
                    pEnve = pGeometry as IEnvelope;
                    if (pEnve.Height == 0 | pEnve.Width == 0)
                        pBlnExit = true;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    IPolygon pPolygon = null;
                    pPolygon = pGeometry as IPolygon;
                    if (pPolygon.Length == 0)
                        pBlnExit = true;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    IPolyline pPolyLine = null;
                    pPolyLine = pGeometry as IPolyline;
                    if (pPolyLine.Length == 0)
                        pBlnExit = true;
                    break;
            }
            if (pBlnExit == true)
            {
                if (blnShow == true)
                {
                    if (Method == esriSelectionResultEnum.esriSelectionResultNew)
                    {
                        pFeatureSelection.Clear();
                    }
                }
                else
                {
                    if (Method == esriSelectionResultEnum.esriSelectionResultNew)
                    {
                        m_featArray = new ESRI.ArcGIS.esriSystem.Array();
                    }
                }
                return;
            }
            if (pGeometry is ITopologicalOperator)
            {
                pTopo = pGeometry as ITopologicalOperator;
                pTopo.Simplify();
            }
            ISpatialFilter pSpatialfilter = null;
            pSpatialfilter = new SpatialFilter();
            pSpatialfilter.Geometry = pGeometry;
            pSpatialfilter.GeometryField = pFeaturelayer.FeatureClass.ShapeFieldName;
            pSpatialfilter.SpatialRel = SpatialRel;
            IFeatureCursor pFeatCursor = null;
            if (blnShow)
            {
                pFeatureSelection.SelectFeatures(pSpatialfilter, Method, false);
            }
            else
            {
                pFeatCursor = pFeaturelayer.Search(pSpatialfilter, false);
                IFeature pFeature = null;
                pFeature = pFeatCursor.NextFeature();
                int i = 0;
                IArray pTempArray = null;
                switch (Method)
                {
                    case esriSelectionResultEnum.esriSelectionResultNew:
                        m_featArray = new ESRI.ArcGIS.esriSystem.Array();
                        while (pFeature != null)
                        {
                            m_featArray.Add(pFeature);
                            pFeature = pFeatCursor.NextFeature();
                        }
                        break;
                    case esriSelectionResultEnum.esriSelectionResultAdd:
                        while (pFeature != null)
                        {
                            for (i = 0; i <= m_featArray.Count - 1; i++)
                            {
                                if (object.ReferenceEquals(m_featArray.get_Element(i), pFeature))
                                    break;
                            }
                            if (i == m_featArray.Count)
                            {
                                m_featArray.Add(pFeature);
                            }
                            pFeature = pFeatCursor.NextFeature();
                        }
                        break;
                    case esriSelectionResultEnum.esriSelectionResultSubtract:
                        while ((pFeature != null))
                        {
                            for (i = 0; i <= m_featArray.Count - 1; i++)
                            {
                                if (object.ReferenceEquals(m_featArray.get_Element(i), pFeature))
                                    break;
                            }
                            if (i < m_featArray.Count)
                            {
                                m_featArray.Remove(i);
                            }
                            pFeature = pFeatCursor.NextFeature();
                        }
                        break;
                    case esriSelectionResultEnum.esriSelectionResultAnd:
                        pTempArray = new ESRI.ArcGIS.esriSystem.Array();
                        while ((pFeature != null))
                        {
                            for (i = 0; i <= m_featArray.Count - 1; i++)
                            {
                                if (object.ReferenceEquals(m_featArray.get_Element(i), pFeature))
                                {
                                    pTempArray.Add(pFeature);
                                    break;
                                }
                            }
                            pFeature = pFeatCursor.NextFeature();
                        }
                        m_featArray = pTempArray;
                        break;
                    case esriSelectionResultEnum.esriSelectionResultXOR:
                        while ((pFeature != null))
                        {
                            for (i = 0; i <= m_featArray.Count - 1; i++)
                            {
                                if (object.ReferenceEquals(m_featArray.get_Element(i), pFeature))
                                    break;
                            }
                            if (i == m_featArray.Count)
                            {
                                m_featArray.Add(pFeature);
                            }
                            else
                            {
                                m_featArray.Remove(i);
                            }
                            pFeature = pFeatCursor.NextFeature();
                        }
                        break;
                    default:
                        m_featArray = new ESRI.ArcGIS.esriSystem.Array();
                        break;
                }
            }
        }
        private static void SymbolInit()
        {
            IRgbColor pFcolor;
            IRgbColor pOcolor;
            IRgbColor pTrackcolor;
            IRgbColor pVcolor;
            IRgbColor pSColor;
            pFcolor = new RgbColor();
            pOcolor = new RgbColor();
            pTrackcolor = new RgbColor();
            pVcolor = new RgbColor();
            pSColor = new RgbColor();
            pFcolor = MapAlgo.ColorRGBT(255, 0, 0);
            pOcolor = MapAlgo.ColorRGBT(0, 0, 255);
            pTrackcolor = MapAlgo.ColorRGBT(0, 255, 255);
            pVcolor = MapAlgo.ColorRGBT(0, 255, 0);
            pSColor = MapAlgo.ColorRGBT(0, 0, 0);

            m_markerSym = new SimpleMarkerSymbol();
            m_markerSym.Style = esriSimpleMarkerStyle.esriSMSCircle;
            m_markerSym.Color = pFcolor;
            m_markerSym.Outline = true;
            m_markerSym.OutlineColor = pOcolor;
            m_markerSym.OutlineSize = 1;
            m_vertexSym = new SimpleMarkerSymbol();
            m_vertexSym.Style = esriSimpleMarkerStyle.esriSMSSquare;
            m_vertexSym.Color = pVcolor;
            m_vertexSym.Size = 4;
            m_selPointSym = new SimpleMarkerSymbol();
            m_selPointSym.Style = esriSimpleMarkerStyle.esriSMSSquare;
            m_selPointSym.Color = pSColor;
            m_selPointSym.Size = 4;
            m_endPointSym = new SimpleMarkerSymbol();
            m_endPointSym.Style = esriSimpleMarkerStyle.esriSMSSquare;
            m_endPointSym.Color = pFcolor;
            m_endPointSym.Size = 4;
            m_lineSym = new SimpleLineSymbol();
            m_lineSym.Color = pFcolor;
            m_lineSym.Width = 1;
            m_tracklineSym = new SimpleLineSymbol();
            m_tracklineSym.Color = pTrackcolor;
            m_tracklineSym.Width = 1;
            ISimpleLineSymbol pOsym = default(ISimpleLineSymbol);
            pOsym = new SimpleLineSymbol();
            pOsym.Color = pOcolor;
            pOsym.Width = 1;
            m_fillSym = new SimpleFillSymbol();
            m_fillSym.Color = pFcolor;
            m_fillSym.Style = esriSimpleFillStyle.esriSFSVertical;
            m_fillSym.Outline = pOsym;
        }
        public static void DisplayGraphic(IGeometry pGeometry, IColor pColor, ISymbol pSymbol)
        {
            ISimpleMarkerSymbol pMSym = new SimpleMarkerSymbol();
            ISimpleLineSymbol pLSym = new SimpleLineSymbol();
            ISimpleFillSymbol pSFSym = new SimpleFillSymbol();
            if (pColor != null)
            {
                pMSym.Style = esriSimpleMarkerStyle.esriSMSCircle;
                pMSym.Color = pColor;

                pLSym.Color = pColor;
                pSFSym.Color = pColor;
                pSFSym.Style = esriSimpleFillStyle.esriSFSSolid;
            }
            //开始绘制图形
            m_activeView.ScreenDisplay.StartDrawing(m_activeView.ScreenDisplay.hDC, -1);
            switch (pGeometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:
                case esriGeometryType.esriGeometryMultipoint:
                    //绘制点
                    if ((pColor != null))
                    {
                        m_activeView.ScreenDisplay.SetSymbol(pMSym as ISymbol);
                    }
                    else if ((pSymbol != null))
                    {
                        m_activeView.ScreenDisplay.SetSymbol(pSymbol);
                    }
                    else
                    {
                        m_activeView.ScreenDisplay.SetSymbol(m_markerSym as ISymbol);
                    }
                    m_activeView.ScreenDisplay.DrawPoint(pGeometry);
                    break;
                case esriGeometryType.esriGeometryLine:
                case esriGeometryType.esriGeometryPolyline:
                    //绘制线
                    if (pColor != null)
                    {
                        m_activeView.ScreenDisplay.SetSymbol(pLSym as ISymbol);
                    }
                    else if (pSymbol != null)
                    {
                        m_activeView.ScreenDisplay.SetSymbol(pSymbol);
                    }
                    else
                    {
                        m_activeView.ScreenDisplay.SetSymbol(m_lineSym as ISymbol);
                    }
                    m_activeView.ScreenDisplay.DrawPolyline(pGeometry);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                case esriGeometryType.esriGeometryEnvelope:
                    //绘制面
                    if (pColor != null)
                    {
                        m_activeView.ScreenDisplay.SetSymbol(pSFSym as ISymbol);
                    }
                    else if (pSymbol != null)
                    {
                        m_activeView.ScreenDisplay.SetSymbol(pSymbol as ISymbol);
                    }
                    else
                    {
                        m_activeView.ScreenDisplay.SetSymbol(m_fillSym as ISymbol);
                    }
                    m_activeView.ScreenDisplay.DrawPolygon(pGeometry);

                    break;
            }
            m_activeView.ScreenDisplay.FinishDrawing();
        }
        public static void ShowAllVertex(IFeatureLayer pFeatLyr)
        {
            m_vertexGeoBag = null;
            if (pFeatLyr == null) return;
            IFeatureCursor pFeatureCursor = MapAlgo.GetSelectedFeatures(pFeatLyr);
            if (pFeatureCursor == null) return;
            IFeature pTFeature = null;
            pTFeature = pFeatureCursor.NextFeature();
            if (pTFeature == null) return;
            m_Map.ClearSelection();
            m_Map.SelectFeature(pFeatLyr as ILayer, pTFeature);
            m_activeView.Refresh();
            if (pTFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                return;
            IArray pFeatureArray = null;
            pFeatureArray = new ESRI.ArcGIS.esriSystem.Array();
            pFeatureArray.Add(pTFeature);
            SymbolInit();
            IFeature pFeature = default(IFeature);
            IPointCollection pPointCol = default(IPointCollection);
            IPoint pPoint = default(IPoint);
            int i = 0; int j = 0;
            m_vertexGeoBag = new GeometryBagClass();
            for (i = 0; i <= pFeatureArray.Count - 1; i++)
            {
                pFeature = pFeatureArray.get_Element(i) as IFeature;
                pPointCol = pFeature.ShapeCopy as IPointCollection;
                for (j = 0; j <= pPointCol.PointCount - 1; j++)
                {
                    pPoint = pPointCol.get_Point(j);
                    if (j == 0 | j == pPointCol.PointCount - 1)
                    {
                        pPoint.ID = 10;
                    }
                    else
                    {
                        pPoint.ID = 100;
                    }
                    IColor pColor = null;
                    object obj = Type.Missing;
                    if (pPoint == pHitPnt)
                    {
                        DisplayGraphic(pPoint, pColor, m_selPointSym as ISymbol);
                    }
                    if (j == 0 || j == pPointCol.PointCount - 1)
                    {
                        DisplayGraphic(pPoint, pColor, m_endPointSym as ISymbol);
                    }
                    else
                    {
                        DisplayGraphic(pPoint, pColor, m_vertexSym as ISymbol);
                    }

                    m_vertexGeoBag.AddGeometry(pPoint, ref obj, ref obj);
                }
            }
        }
        public static IFeature GetSelectedFeature(IFeatureLayer pFeatLyr)
        {
            IFeature functionReturnValue = null;
            IFeatureCursor pFeatureCursor = null;
            IFeature pFeature = null;
            pFeatureCursor = MapAlgo.GetSelectedFeatures(pFeatLyr);
            if (pFeatureCursor == null)
            {
                functionReturnValue = null;
                return functionReturnValue;
            }
            pFeature = pFeatureCursor.NextFeature();
            functionReturnValue = pFeature;
            return functionReturnValue;
        }
        public static IPolyline GetBoundary(IFeatureLayer pFeatLyr)
        {
            IPolyline functionReturnValue = null;
            ITopologicalOperator pTopologicOpr = null;
            IGeometry pGeometry = null;
            IFeature pFeature = null;
            pFeature = GetSelectedFeature(pFeatLyr);
            if (pFeature == null)
            {
                functionReturnValue = null;
                MessageBox.Show("请选择要编辑的多边形!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return functionReturnValue;
            }
            else
            {
                pGeometry = pFeature.Shape;
                pTopologicOpr = pGeometry as ITopologicalOperator;
                functionReturnValue = pTopologicOpr.Boundary as IPolyline;
            }
            return functionReturnValue;
        }
        public static void ClearResource()
        {
            m_vertexGeoBag = null;
            if (m_activeView != null) m_activeView.Refresh();
        }
    }
}
