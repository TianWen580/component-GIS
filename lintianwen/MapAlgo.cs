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
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lintianwen
{
    static class MapAlgo
    {
        /* mapAlgo用于编写某些功能的辅助函数
         * - 静态类无需实例化
        */

        private static IEngineEditor _engineEditor;
        public static IEngineEditor EngineEditor
        {
            get { return MapAlgo._engineEditor; }
            set { MapAlgo._engineEditor = value; }
        }

        public static Form ToolPlatForm = null;

        #region unit opt
        /// <summary>
        /// get the unit string from esri map units
        /// </summary>
        /// <param name="_esriMapUnit">esri map units</param>
        /// <returns>string of unit</returns>
        public static string GetMapUnit(esriUnits _esriMapUnit)
        {
            string sMapUnits = string.Empty;
            switch (_esriMapUnit)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "厘米";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "十进制";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "分米";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "尺";
                    break;
                case esriUnits.esriInches:
                    sMapUnits = "英寸";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "千米";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "米";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "英里";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "毫米";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "海里";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "点";
                    break;
                case esriUnits.esriUnitsLast:
                    sMapUnits = "UnitsLast";
                    break;
                case esriUnits.esriUnknownUnits:
                    sMapUnits = "未知单位";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "码";
                    break;
                default:
                    break;
            }
            return sMapUnits;
        }

        /// <summary>
        /// unit trnasformation
        /// </summary>
        /// <param name="pixelUnits"></param>
        /// <returns></returns>
        public static double ConvertPixelsToMapUnits(IActiveView activeView, double pixelUnits)
        {
            int pixelExtent = activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().right
                - activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().left;

            double realWorldDisplayExtent = activeView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;

            return pixelUnits * sizeOfOnePixel;
        }
        #endregion

        #region get path with string
        public static string GetPath(string path)
        {
            int t;
            for (t = 0; t < path.Length; t++)
            {
                if (path.Substring(t, 5) == "chp02")
                {
                    break;
                }
            }
            string name = path.Substring(0, t + 5);
            return name;
        }
        #endregion

        #region get feature class
        /// <summary>
        /// get feature class from layer
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="_lstFeatCls"></param>
        public static void GetFeatureClassInLayer(ILayer pLayer, ref List<IFeatureClass> _lstFeatCls)
        {
            try
            {
                ILayer pLyr = null;
                ICompositeLayer pComLyr = pLayer as ICompositeLayer;
                if (pComLyr == null)
                {
                    IFeatureLayer pFeatLyr = pLayer as IFeatureLayer;
                    if (!_lstFeatCls.Contains(pFeatLyr.FeatureClass))
                    {
                        _lstFeatCls.Add(pFeatLyr.FeatureClass);
                    }
                }
                else
                {
                    for (int i = 0; i < pComLyr.Count; i++)
                    {
                        pLyr = pComLyr.get_Layer(i);
                        GetFeatureClassInLayer(pLyr, ref _lstFeatCls);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// get feature class from map
        /// </summary>
        /// <param name="pMap"></param>
        /// <returns></returns>
        public static List<IFeatureClass> GetFeatureClass(IMap pMap)
        {
            List<IFeatureClass> _lstFeatCls = null;
            try
            {
                ILayer pLayer = null;
                IFeatureLayer pFeatLyr = null;
                _lstFeatCls = new List<IFeatureClass>();
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    pLayer = pMap.get_Layer(i);
                    pFeatLyr = pLayer as IFeatureLayer;
                    GetFeatureClassInLayer(pLayer, ref _lstFeatCls);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return _lstFeatCls;
        }
        #endregion

        #region get color with RGB and transparency
        public static IRgbColor ColorRGBT(int R, int G, int B, byte transparency=255)
        {
            if (R < 0 || R > 255 || G < 0 || G > 255 || B < 0 || B > 255 || transparency < 0 || transparency > 255)
                return null;
            else
            {
                IRgbColor pColor = new RgbColor();
                pColor.Red = R;
                pColor.Green = G;
                pColor.Blue = B;
                pColor.Transparency = transparency;
                return pColor;
            }
        }
        #endregion

        #region reset data in map
        /// <summary>
        /// reset mapControl
        /// </summary>
        public static void CleanAllInMap(IMap pMap, string docFileName, int lyrCount)
        {
            if (pMap != null && lyrCount > 0)
            {
                //reset the map and name
                IMap newMap = new MapClass();
                newMap.Name = "New map";
                docFileName = string.Empty;
                pMap = newMap;

                //the same in the eagle map
                IMap newEagleMap = new MapClass();
                newEagleMap.Name = "New eagle map";
                docFileName = string.Empty;
                pMap = newEagleMap;
            }
        }
        #endregion

        #region helper for map data
        /// <summary>
        /// help for transforming feature layer
        /// </summary>
        /// <param name="pWorkspace">workspace</param>
        /// <param name="featureName">shape file name</param>
        /// <returns></returns>
        public static ILayer FeatureHelper(IWorkspace pWorkspace, string featureName)
        {
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(featureName);
            pFeatureLayer.Name = featureName;
            return pFeatureLayer;
        }

        /// <summary>
        /// help for transforming raster layer
        /// </summary>
        /// <param name="pWorkspace">workspace</param>
        /// <param name="rasterName">raster file name</param>
        /// <returns></returns>
        public static ILayer RasterHelper(IWorkspace pWorkspace, string rasterName)
        {
            IRasterWorkspace pRasterWorkspace = pWorkspace as IRasterWorkspace;
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(rasterName);
            IRasterPyramid pPyramid = pRasterDataset as IRasterPyramid; //construct raster pyramid
            if (!pPyramid.Present)
            {
                pPyramid.Create(); //create pyramid when it didn`t exist
            }
            IRaster pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterLayer pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            return pRasterLayer as ILayer;
        }

        /// <summary>
        /// help for reading the layers in personal mdb
        /// </summary>
        /// <param name="pWorkspace">workspace loaded</param>
        public static List<IFeatureLayer> DatabaseHelper(IWorkspace pWorkspace)
        {
            List<IFeatureLayer> pFLayers = new List<IFeatureLayer>();
            IEnumDataset pEnumdataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTAny);
            pEnumdataset.Reset();
            IDataset pDataset = pEnumdataset.Next();
            while (pDataset != null)
            {
                //feature dataset
                if (pDataset is IFeatureDataset)
                {
                    IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                    IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(pDataset.Name);
                    IEnumDataset pEnumDataset = pFeatureDataset.Subsets;
                    pEnumDataset.Reset();
                    IGroupLayer pGroupLayer = new GroupLayerClass();
                    pGroupLayer.Name = pFeatureDataset.Name;
                    IDataset pDatasetNext = pEnumDataset.Next();
                    while (pDatasetNext != null)
                    {
                        if (pDatasetNext is IFeatureClass)
                        {
                            IFeatureLayer pFlayer = new FeatureLayerClass();
                            pFlayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(pDatasetNext.Name);
                            if (pFlayer.FeatureClass != null)
                            {
                                pFlayer.Name = pFlayer.FeatureClass.AliasName;
                                pGroupLayer.Add(pFlayer);
                                pFLayers.Add(pFlayer);
                            }
                        }
                        pDatasetNext = pEnumDataset.Next();
                    }
                }
                //feature class
                else if (pDataset is IFeatureClass)
                {
                    ILayer pLayer = MapAlgo.FeatureHelper(pWorkspace, pDataset.Name);
                    pFLayers.Add(pLayer as IFeatureLayer);
                }
                //raster data
                else if (pDataset is IRasterDataset)
                {
                    ILayer pLayer = MapAlgo.RasterHelper(pWorkspace, pDataset.Name);
                    pFLayers.Add(pLayer as IFeatureLayer);
                }
                pDataset = pEnumdataset.Next();
            }

            return pFLayers;
        }
        #endregion

        #region get layer or layers
        /// <summary>
        /// get a layer with the layer name
        /// </summary>
        /// <param name="name">layer name</param>
        /// <returns>IFeatureLayer</returns>
        public static IFeatureLayer GetLayerFromName(IMap pMap, string name)
        {
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                if (pMap.get_Layer(i) is GroupLayer)
                {
                    ICompositeLayer compositeLayer = pMap.get_Layer(i) as ICompositeLayer;
                    for (int j = 0; j < compositeLayer.Count; j++)
                    {
                        if (compositeLayer.get_Layer(j).Name == name)
                            return compositeLayer.get_Layer(j) as IFeatureLayer;
                    }
                }
                else
                {
                    if (pMap.get_Layer(i).Name == name)
                        return pMap.get_Layer(i) as IFeatureLayer;
                }
            }
            return null;
        }

        /// <summary>
        /// get the layers of map
        /// </summary>
        /// <param name="pMap"></param>
        /// <returns></returns>
        public static List<ILayer> GetLayers(IMap pMap)
        {
            ILayer plyr = null;
            List<ILayer> pLstLayers = null;
            try
            {
                pLstLayers = new List<ILayer>();
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    plyr = pMap.get_Layer(i);
                    if (!pLstLayers.Contains(plyr))
                    {
                        pLstLayers.Add(plyr);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return pLstLayers;
        }
        #endregion

        #region feature to selection
        /// <summary>
        /// select the features
        /// </summary>
        /// <param name="pFeatLyr">feature targets</param>
        /// <returns></returns>
        public static IFeatureCursor GetSelectedFeatures(IFeatureLayer pFeatLyr)
        {
            ICursor pCursor = null;
            IFeatureCursor pFeatCur = null;
            if (pFeatLyr == null) return null;

            IFeatureSelection pFeatSel = pFeatLyr as IFeatureSelection;
            ISelectionSet pSelSet = pFeatSel.SelectionSet;
            if (pSelSet.Count == 0) return null;
            pSelSet.Search(null, false, out pCursor);
            pFeatCur = pCursor as IFeatureCursor;
            return pFeatCur;
        }
        #endregion

        #region distance
        /// <summary>
        /// 计算两点之间X轴方向和Y轴方向上的距离
        /// </summary>
        /// <param name="lastpoint"></param>
        /// <param name="firstpoint"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <returns></returns>
        public static bool Distance(IPoint lastpoint, IPoint firstpoint, out double deltaX, out double deltaY)
        {
            deltaX = 0; deltaY = 0;
            if (lastpoint == null || firstpoint == null)
                return false;
            deltaX = lastpoint.X - firstpoint.X;
            deltaY = lastpoint.Y - firstpoint.Y;
            return true;
        }
        #endregion
    }
}
