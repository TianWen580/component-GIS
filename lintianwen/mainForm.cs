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
using stdole;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//my functions folders
using lintianwen.Selection;
using lintianwen.Bookmark;
using lintianwen.Measure;
using lintianwen.Labeling;
using lintianwen.Cartography;
using lintianwen.CommonToolsAndCommands;
using lintianwen.Spatial_Analysis;

namespace lintianwen
{
    public partial class mainForm : Form
    {
        //app info
        private string author = "林天文";
        private string version = "1.0";

        //init var
        private Dictionary<int, string> mouseStateDict = new Dictionary<int, string>();
        private string mouseState = null;
        private IEnvelope pEnvelope;                                            //extent of mainMapControl
        private INewLineFeedback pNewLineFeedback = null;                       //track the line
        private IPointCollection pLinePointCollection = new MultipointClass();  //line key point collection
        private IPoint pMouseHitPt = null;                                      //point when mouse click
        private IPoint pMouseMovingPt = null;                                   //point when mouse move
        private double pToltalLength = 0;
        private double dToltalLength = 0;                                       //length measured
        private double dSubLength = 0;                                          //sub length measured
        private string mapUnitString = "未知单位";                              //unit of map
        private IFeatureLayer pFocusFeatureLyr;                                 //overall focusing feature layer

        //edit features
        private string sMxdPath = Application.StartupPath;
        private IMap m_Map = null;
        private IActiveView pActiveView = null;
        private List<ILayer> plstLayers = null;
        private IFeatureLayer pCurrentLyr = null;
        private IEngineEditor pEngineEditor = null;
        private IEngineEditTask pEngineEditTask = null;
        private IEngineEditLayers pEngineEditLayers = null;

        //cartography
        private INewEnvelopeFeedback pNewEnvelopeFeedback;
        private EnumMapSurroundType _enumMapSurType = EnumMapSurroundType.None;
        private IStyleGalleryItem pStyleGalleryItem;
        private System.Drawing.Printing.PrintDocument document = new System.Drawing.Printing.PrintDocument();
        private IPoint m_MovePt = null;
        private IPoint m_PointPt = null;

        //overall form
        measureResultForm pMeasureResultForm = null;
        attributesViewingForm pAttributesViewingForm = null;
        textElementForm pTextElementForm = null;
        symSingleForm pSymSingleForm = null;
        uniqueValueRenForm pUniqueValueRenForm = null;
        uniqueValueDualFields pUniqueValueDualFields = null;
        graduatedcolorsForm pGraduatedcolorsForm = null;
        graduatedSymbolsForm pGraduatedSymbolsForm = null;
        proportionalForm pProportionalForm = null;
        dotDensityForm pDotDensityForm = null;
        symbolForm pSymbolForm = null;

        public mainForm()
        {
            InitializeComponent();
        }

        #region init object 
        private void InitObject()
        {
            try
            {
                ChangeButtonState(false);
                pEngineEditor = new EngineEditorClass();
                MapAlgo.EngineEditor = pEngineEditor;
                pEngineEditTask = pEngineEditor as IEngineEditTask;
                pEngineEditLayers = pEngineEditor as IEngineEditLayers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region view toggling
        /// <summary>
        /// view toggling function
        /// </summary>
        /// <param name="flag">flag marks the showing of item</param>
        /// <returns>flag</returns>
        private bool view_toggle(bool flag)
        {
            if (flag == true) return false;
            else return true;
        }

        private void 状态栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = view_toggle(statusStrip1.Visible);
            statusStrip1.Visible = flag;
            状态栏ToolStripMenuItem.Checked = flag;
        }

        private void toolsbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = view_toggle(toolStrip.Visible);
            toolStrip.Visible = flag;
            tabControl2.Visible = flag;
            toolsbarToolStripMenuItem.Checked = flag;
        }

        private void tocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = view_toggle(panel1.Visible);
            panel1.Visible = flag;
            tocToolStripMenuItem.Checked = flag;
        }

        private void mapAndPageLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = view_toggle(tabControl1.Visible);
            tabControl1.Visible = flag;
            mapAndPageLayoutToolStripMenuItem.Checked = flag;
        }
        #endregion

        #region winform events
        //form loading...
        private void mainForm_Load(object sender, EventArgs e)
        {
            //viewing init setting
            statusStrip1.Visible = true;
            toolStrip.Visible = true;
            axTOCControl1.Visible = true;
            sLabelWorking.Visible = false;
            sProgressBar.Visible = false;

            //winform init setting
            this.WindowState = FormWindowState.Maximized;
            axTOCControl1.SetBuddyControl(axMapControlMain.Object);

            //init mouse state dict
            mouseStateDict.Add(0, "panMouse");              //pan
            mouseStateDict.Add(1, "measureDisMouse");       //measure distance
            mouseStateDict.Add(2, "zoomInMouse");           //zoom in map
            mouseStateDict.Add(3, "zoomOutMouse");          //zoom out map
            mouseStateDict.Add(4, "selFeatureMouse");       //selection
            mouseStateDict.Add(5, "none");                  //nothing happen

            //init mouse state
            mouseState = mouseStateDict[0];

            //init object
            InitObject();
        }

        private void menuExitApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 4)
            {
                tabControl1.SelectedIndex = 1;
            }
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "此软件由" + author + "开发\n" +
                "Power by ArcGIS Engine10.2\n" +
                "version：" + version
                );
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                tabControl2.SelectedIndex = 4;
                IObjectCopy pObjectCopy = new ObjectCopy();
                object copyFromMap = axMapControlMain.Map;
                object copiedMap = pObjectCopy.Copy(copyFromMap);
                object copyToMap = axPageLayoutControl1.ActiveView.FocusMap;
                pObjectCopy.Overwrite(copiedMap, ref copyToMap);
                axPageLayoutControl1.ActiveView.Refresh();
            }
            else
            {
                tabControl2.SelectedIndex = 0;
            }
        }

        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCleanSelection_Click(object sender, EventArgs e)
        {
            IActiveView pActiveView = axMapControlMain.ActiveView;
            pActiveView.FocusMap.ClearSelection();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            btnHelp_Click(sender, e);
        }

        private void btnOpenShp_Click(object sender, EventArgs e)
        {
            menuShapeFile_Click(sender, e);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            menuLoadMxFile_Click(sender, e);
        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            menuShapeFile_Click(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            menuLoadMxFile_Click(sender, e);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mouseState = mouseStateDict[2];
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            mouseState = mouseStateDict[0];
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mouseState = mouseStateDict[3];
        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            mouseState = mouseStateDict[4];
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("努力开发中...");
        }
        #endregion

        #region eagle map
        private IPoint pRectMousePositionPoint; //point location of mouse in the rect
        private bool isRectDragable = false; //mark accessibility of draging

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            pEnvelope = e.newEnvelope as IEnvelope;
            DrawRectangle(pEnvelope);
        }

        private void axEagleViewMap_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (axMapControlMain.LayerCount > 0)
            {
                //different mouse btns
                switch (e.button)
                {
                    case 1: //left btn click to move rect
                        if (e.mapX > pEnvelope.XMax || e.mapX < pEnvelope.XMin || e.mapY > pEnvelope.YMax || e.mapY < pEnvelope.YMin)
                            isRectDragable = false;
                        else
                            isRectDragable = true;

                        //mark the location point
                        pRectMousePositionPoint = new ESRI.ArcGIS.Geometry.Point();
                        pRectMousePositionPoint.PutCoords(e.mapX, e.mapY);
                        break;
                    case 2: //right btn click to draw new rect
                        IEnvelope pDrawEnvelope = axEagleViewMap.TrackRectangle(); //track the rect drawed by mouse
                        axMapControlMain.Extent = pDrawEnvelope; //reset the extent
                        IPoint pCenterPoint = new ESRI.ArcGIS.Geometry.Point();
                        pCenterPoint.PutCoords(pDrawEnvelope.XMax - pDrawEnvelope.Width / 2, pDrawEnvelope.YMax - pDrawEnvelope.Height / 2);
                        axMapControlMain.CenterAt(pCenterPoint); //reset the center position of mapControl
                        break;
                }
            }
            else
                return;
        }

        private void axEagleViewMap_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (pEnvelope == null)
                return;
            //setting mouse style
            if (e.mapX > pEnvelope.XMax || e.mapX < pEnvelope.XMin || e.mapY > pEnvelope.YMax || e.mapY < pEnvelope.YMin)
                axEagleViewMap.MousePointer = esriControlsMousePointer.esriPointerDefault; //default style out of rect
            else
            {
                //hand style inside rect
                switch (e.button)
                {
                    case 1:
                        axEagleViewMap.MousePointer = esriControlsMousePointer.esriPointerPan;
                        break;
                    case 2:
                        axEagleViewMap.MousePointer = esriControlsMousePointer.esriPointerDefault;
                        break;
                }
            }

            //changing extent with hand styled mouse
            if (isRectDragable)
            {
                //set the offset in eagle map
                double deltaX = e.mapX - pRectMousePositionPoint.X; //position right now - position when start
                double deltaY = e.mapY - pRectMousePositionPoint.Y;
                pEnvelope.Offset(deltaX, deltaY);
                pRectMousePositionPoint.PutCoords(e.mapX, e.mapY);
                DrawRectangle(pEnvelope);

                //set the extent when rect changed
                axMapControlMain.Extent = pEnvelope;
            }
        }

        private void axEagleViewMap_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (e.button == 1 && pRectMousePositionPoint != null)
            {
                if (e.mapX == pRectMousePositionPoint.X && e.mapY == pRectMousePositionPoint.Y)
                {
                    axMapControlMain.CenterAt(pRectMousePositionPoint);
                }
                isRectDragable = false;
            }
        }

        /// <summary>
        /// add layer to eagle map
        /// </summary>
        /// <param name="mapLayers">layers in mapControl</param>
        /// <returns>feature</returns>
        private void AddLayersToEagleMap(ILayer mapLayers)
        {
            //group layer and composite layer need to be separated
            if (mapLayers is IGroupLayer || mapLayers is ICompositeLayer)
            {
                ICompositeLayer pCompositeLayer = mapLayers as ICompositeLayer;
                for (int i = 0; i < pCompositeLayer.Count; i++)
                {
                    ILayer pSubLayer = pCompositeLayer.get_Layer(i);
                    IFeatureLayer pFeatureLayer = pSubLayer as IFeatureLayer;
                    if (pFeatureLayer != null)
                    {
                        //skip for points layer
                        if (pFeatureLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPoint
                            && pFeatureLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryMultipoint)
                            axEagleViewMap.AddLayer(pFeatureLayer as ILayer);
                    }
                }
            }
            else axEagleViewMap.AddLayer(mapLayers as ILayer); //single feature layer
        }

        //draw rectangle on eagle map
        private void DrawRectangle(IEnvelope pEnvelope)
        {
            //create elements container and activeView
            IGraphicsContainer pGraphicsContainer = axEagleViewMap.Map as IGraphicsContainer;
            IActiveView pActiveView = pGraphicsContainer as IActiveView;

            //if left last rectangle, delete it
            pGraphicsContainer.DeleteAllElements();

            //init the extent of new rectangle
            IRectangleElement pRectElem = new RectangleElement() as IRectangleElement;
            IElement pElem = pRectElem as IElement;
            pElem.Geometry = pEnvelope; //set the extent of new rectangle element

            //draw it
            ILineSymbol pOutline = new SimpleLineSymbol();
            pOutline.Width = 2.25;
            pOutline.Color = MapAlgo.ColorRGBT(255, 0, 0, 255);
            IFillSymbol pFillRect = new SimpleFillSymbol();
            pFillRect.Outline = pOutline;
            pFillRect.Color = MapAlgo.ColorRGBT(0, 0, 0, 0);

            //insert element to eagle map
            IFillShapeElement pFillRectElem = pElem as IFillShapeElement;
            pFillRectElem.Symbol = pFillRect;
            pGraphicsContainer.AddElement(pFillRectElem as IElement, 0);

            //partial refresh
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        //refresh eagle map
        private void RefreshEagleMap()
        {
            axEagleViewMap.Extent = axMapControlMain.FullExtent; //fully vision
            axEagleViewMap.SpatialReference = axMapControlMain.SpatialReference; //sanme spatial reference with mapControl
            for (int i = 0; i < axMapControlMain.LayerCount; i++)
            {
                ILayer pLayer = axMapControlMain.get_Layer(i);
                AddLayersToEagleMap(pLayer);
            }
            pEnvelope = axMapControlMain.Extent as IEnvelope;
            DrawRectangle(pEnvelope);
            axEagleViewMap.ActiveView.Refresh();
        }
        #endregion

        #region main map events
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            //get the point when mouse hit
            pMouseHitPt = (axMapControlMain.Map as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            pLinePointCollection.AddPoint(pMouseHitPt);

            //diferent mouse btn
            if (e.button == 1)
            {
                IActiveView pActiveView = axMapControlMain.ActiveView;

                //judging what mouse needing...
                switch (mouseState)
                {
                    #region zoom in map

                    case "zoomInMouse":
                        pEnvelope = axMapControlMain.TrackRectangle();
                        //如果拉框范围为空则返回
                        if (pEnvelope == null || pEnvelope.IsEmpty || pEnvelope.Height == 0 || pEnvelope.Width == 0)
                        {
                            return;
                        }
                        //如果有拉框范围，则放大到拉框范围
                        pActiveView.Extent = pEnvelope;
                        pActiveView.Refresh();
                        break;

                    #endregion

                    #region zoom out map

                    case "zoomOutMouse":
                        pEnvelope = axMapControlMain.TrackRectangle();

                        //如果拉框范围为空则退出
                        if (pEnvelope == null || pEnvelope.IsEmpty || pEnvelope.Height == 0 || pEnvelope.Width == 0)
                        {
                            return;
                        }
                        //如果有拉框范围，则以拉框范围为中心，缩小倍数为：当前视图范围/拉框范围
                        else
                        {
                            double dWidth = pActiveView.Extent.Width * pActiveView.Extent.Width / pEnvelope.Width;
                            double dHeight = pActiveView.Extent.Height * pActiveView.Extent.Height / pEnvelope.Height;
                            double dXmin = pActiveView.Extent.XMin -
                                           ((pEnvelope.XMin - pActiveView.Extent.XMin) * pActiveView.Extent.Width /
                                            pEnvelope.Width);
                            double dYmin = pActiveView.Extent.YMin -
                                           ((pEnvelope.YMin - pActiveView.Extent.YMin) * pActiveView.Extent.Height /
                                            pEnvelope.Height);
                            double dXmax = dXmin + dWidth;
                            double dYmax = dYmin + dHeight;
                            pEnvelope.PutCoords(dXmin, dYmin, dXmax, dYmax);
                        }
                        pActiveView.Extent = pEnvelope;
                        pActiveView.Refresh();
                        break;

                    #endregion

                    #region selection

                    case "selFeatureMouse":
                        IEnvelope pEnv = axMapControlMain.TrackRectangle();
                        IGeometry pGeo = pEnv as IGeometry;
                        //矩形框若为空，即为点选时，对点范围进行扩展
                        if (pEnv.IsEmpty == true)
                        {
                            tagRECT r;
                            r.left = e.x - 5;
                            r.top = e.y - 5;
                            r.right = e.x + 5;
                            r.bottom = e.y + 5;
                            pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnv, ref r, 4);
                            pEnv.SpatialReference = pActiveView.FocusMap.SpatialReference;
                        }
                        pGeo = pEnv as IGeometry;
                        axMapControlMain.Map.SelectByShape(pGeo, null, false);
                        axMapControlMain.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        break;

                    #endregion

                    #region measure distance
                    case "measureDisMouse":
                        //first hit or after first hit
                        if (pNewLineFeedback == null)
                        {
                            //first hit
                            pNewLineFeedback = new NewLineFeedbackClass();
                            pNewLineFeedback.Display = pActiveView.ScreenDisplay;
                            pNewLineFeedback.Start(pMouseHitPt);
                            dToltalLength = 0;
                            pToltalLength = dToltalLength;
                        }
                        else
                        {
                            //after first hit
                            IPoint lastPoint = pLinePointCollection.get_Point(pLinePointCollection.PointCount - 1); //get the last point
                            pNewLineFeedback.AddPoint(pMouseHitPt); //collect the point mouse hit
                            double deltaX = pMouseHitPt.X - lastPoint.X;
                            double deltaY = pMouseHitPt.Y - lastPoint.Y;
                            dSubLength = Math.Round(Math.Sqrt(deltaX * deltaX + deltaY * deltaY), 3);

                            //sum up total distance
                            pToltalLength = dToltalLength;
                            dToltalLength += dSubLength;
                        }
                        break;
                    #endregion

                    #region map pan
                    case "panMouse":
                        if (axMapControlMain.LayerCount > 0)
                            axMapControlMain.Pan();
                        break;
                    #endregion

                    default:
                        break;
                }
            }
        }

        private void axMapControlMain_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (mouseState == mouseStateDict[1])
            {
                //summary and init all items
                if (pMeasureResultForm != null)
                    pMeasureResultForm.lblMeasureRes.Text = "最终计算总长：" + dToltalLength + mapUnitString;
                if (pNewLineFeedback != null)
                {
                    pNewLineFeedback.Stop();
                    pNewLineFeedback = null;
                    axMapControlMain.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                }
                dToltalLength = 0;
                dSubLength = 0;
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (axMapControlMain.Map.LayerCount <= 0)
                return;
            if (mouseState == mouseStateDict[1])
            {
                if (pNewLineFeedback != null)
                {
                    //refresh location label in statusBar
                    mapUnitString = MapAlgo.GetMapUnit(axMapControlMain.Map.MapUnits);
                    sLabelLocation.Text = "Location:" + String.Format("X:{0:#.###} Y:{1:#.###}{2}", e.mapX, e.mapY, mapUnitString);

                    //get map location point with screen position
                    IActiveView pActiveView = axMapControlMain.ActiveView;
                    pMouseMovingPt = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

                    //refresh the line viewing
                    pNewLineFeedback.MoveTo(pMouseMovingPt);
                    double deltaX = pMouseMovingPt.X - pMouseHitPt.X;
                    double deltaY = pMouseMovingPt.Y - pMouseHitPt.Y;
                    dSubLength = Math.Round(Math.Sqrt(deltaX * deltaX + deltaY * deltaY), 3);
                    if (dSubLength != 0)
                        dToltalLength = pToltalLength + dSubLength;

                    //refresh the describtion
                    if (pMeasureResultForm != null)
                        pMeasureResultForm.lblMeasureRes.Text = String.Format("总长：{0:#.###}{2} | 当前线段长度：{1:#.###}{2}", dToltalLength, dSubLength, mapUnitString);
                }
            }
            else
            {
                mapUnitString = MapAlgo.GetMapUnit(axMapControlMain.Map.MapUnits);
                sLabelLocation.Text = "Location:" + String.Format("X:{0:#.###} Y:{1:#.###}{2}", e.mapX, e.mapY, mapUnitString);
            }
        }

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
        }
        #endregion

        #region save mx document
        private void menuSave_Click(object sender, EventArgs e)
        {
            try
            {
                string mxdFileName = axMapControlMain.DocumentFilename;
                IMapDocument newMapDocument = new MapDocumentClass();

                //if exist map document?
                if (mxdFileName == null)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "保存工作空间";
                    sfd.Filter = "ArcMap 文档（*.mxd)|*.mxd|ArcMap模板（*.mxt)|*.mst";
                    sfd.OverwritePrompt = true;
                    sfd.RestoreDirectory = true;
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        mxdFileName = sfd.FileName;
                    else
                        return;
                }
                else if (newMapDocument.get_IsReadOnly(mxdFileName) && axMapControlMain.CheckMxFile(mxdFileName))
                {
                    MessageBox.Show("当前文档不可编辑！");
                    return;
                }

                //update the map document
                newMapDocument.New(mxdFileName);
                newMapDocument.Save(newMapDocument.UsesRelativePaths, true);
                newMapDocument.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存出现问题\n" + ex.Message);
            }
        }

        private void btnSaveMXD_Click(object sender, EventArgs e)
        {
            menuSave_Click(sender, e);
        }

        private void btnSaveTool_Click(object sender, EventArgs e)
        {
            menuSave_Click(sender, e);
        }
        #endregion

        #region loading mx document
        //mapControl.LoadMxFile
        private void menuLoadMxFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Title = "选择文件...";
            ofd.Filter = "ArcMap文档(*.mxd)|*.mxd|ArcMap文档(*.mxt)|*.mxt|发布地图文件(*.pmf)|*.pmf|所有地图格式(*.mxd;*.mxt;*.pmf)|*.mxd;*.mxt;*.pmf";
            ofd.Multiselect = false; //mx document have no support to muti files & I don't want to load them in muti forms
            ofd.RestoreDirectory = true; //remember last dir
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = ofd.FileName;
                if (fileName == "" || !axMapControlMain.CheckMxFile(fileName))
                    return;
                else
                {
                    try
                    {
                        //clean existed files
                        MapAlgo.CleanAllInMap(axMapControlMain.Map, axMapControlMain.DocumentFilename, axMapControlMain.LayerCount);

                        //loads file
                        axMapControlMain.LoadMxFile(fileName);
                        RefreshEagleMap();

                        //init the layers in editor
                        m_Map = axMapControlMain.Map;
                        pActiveView = m_Map as IActiveView;
                        plstLayers = MapAlgo.GetLayers(m_Map);

                        sLabelMap.Text = "Map:" + axMapControlMain.Map.Name;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("地图载入失败，请检查后重试\n" + ex.Message);
                        return;
                    }
                }
            }
        }

        //load with ImapDocument interface        
        private void menuIMapDocument_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Title = "选择文件...";
            ofd.Filter = "ArcMap文档(*.mxd)|*.mxd|ArcMap文档(*.mxt)|*.mxt|发布地图文件(*.pmf)|*.pmf|所有地图格式(*.mxd;*.mxt;*.pmf)|*.mxd;*.mxt;*.pmf";
            ofd.Multiselect = false; //mx document have no support to muti files & I don't want to load them in muti forms
            ofd.RestoreDirectory = true; //remember last dir
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = ofd.FileName;
                if (fileName == "" || !axMapControlMain.CheckMxFile(fileName))
                    return;
                else
                {
                    //check mxd file with mapControl
                    if (axMapControlMain.CheckMxFile(fileName))
                    {
                        try
                        {
                            //clean existed files
                            MapAlgo.CleanAllInMap(axMapControlMain.Map, axMapControlMain.DocumentFilename, axMapControlMain.LayerCount);

                            IMapDocument md = new MapDocument();
                            md.Open(fileName);
                            axMapControlMain.Map = md.ActiveView.FocusMap;
                            RefreshEagleMap();
                            axMapControlMain.ActiveView.Refresh();

                            //init the layers in editor
                            m_Map = axMapControlMain.Map;
                            pActiveView = m_Map as IActiveView;
                            plstLayers = MapAlgo.GetLayers(m_Map);

                            sLabelMap.Text = "Map:" + axMapControlMain.Map.Name;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("地图载入失败，请检查后重试\n" + ex.Message);
                            return;
                        }
                    }
                }
            }
        }

        //load with CODCC
        private void menuCODCC_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsOpenDocCommandClass();
            pCommand.OnCreate(axMapControlMain.Object);
            pCommand.OnClick();

            //init the layers in editor
            m_Map = axMapControlMain.Map;
            pActiveView = m_Map as IActiveView;
            plstLayers = MapAlgo.GetLayers(m_Map);
        }
        #endregion

        #region loading file
        private void menuShapeFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择shape file";
            ofd.Filter = "Shape file(*.shp)|*.shp";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string fileName = ofd.FileName;
                    int idx = fileName.LastIndexOf("\\");
                    string shpPath = fileName.Substring(0, idx);//get path
                    string shpName = fileName.Substring(idx + 1);//get single shape file name

                    //transform to a vector layer
                    IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
                    IWorkspace pWorkSpace = pWorkspaceFactory.OpenFromFile(shpPath, 0);
                    ILayer pLayer = MapAlgo.FeatureHelper(pWorkSpace, shpName);

                    //add feature class to mapControl and eagle map
                    axMapControlMain.AddLayer(pLayer as ILayer);
                    axMapControlMain.Map.Name = shpName;
                    axMapControlMain.ActiveView.Refresh();
                    RefreshEagleMap();

                    //init the layers in editor
                    m_Map = axMapControlMain.Map;
                    pActiveView = m_Map as IActiveView;
                    plstLayers = MapAlgo.GetLayers(m_Map);

                    sLabelMap.Text = "Map:" + axMapControlMain.Map.Name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法加载shape file\n" + ex.Message);
                }
            }
        }

        private void menuAccessMdb_Click(object sender, EventArgs e)
        {
            List<IFeatureLayer> pFLayers = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择个人地理数据库mdb";
            ofd.Filter = "个人地理数据库(*.mdb)|*.mdb";
            ofd.Multiselect = false;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fullPath = ofd.FileName;
                if (fullPath != "")
                {
                    IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactory();
                    IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(fullPath, 0);
                    MapAlgo.CleanAllInMap(axMapControlMain.Map, axMapControlMain.DocumentFilename, axMapControlMain.LayerCount);
                    pFLayers = MapAlgo.DatabaseHelper(pWorkspace); //helper for personal mdb
                    foreach (var Flayer in pFLayers)
                        axMapControlMain.AddLayer(Flayer);

                    //refreshing...
                    axMapControlMain.Map.Name = fullPath.Substring(fullPath.LastIndexOf('\\') + 1);
                    axMapControlMain.ActiveView.Refresh();
                    RefreshEagleMap();

                    //init the layers in editor
                    m_Map = axMapControlMain.Map;
                    pActiveView = m_Map as IActiveView;
                    plstLayers = MapAlgo.GetLayers(m_Map);

                    sLabelMap.Text = "Map:" + axMapControlMain.Map.Name;
                }
            }
        }

        private void menuAutoCAD_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择CAD文件";
            ofd.Filter = "CAD(*.dwg)|*.dwg";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string fileName = ofd.FileName;
                    int idx = fileName.LastIndexOf("\\");
                    string pCADPath = fileName.Substring(0, idx); //get path
                    string pCADName = fileName.Substring(idx + 1); //get the single CAD file name

                    //transform to a feture layer
                    IWorkspaceFactory pWorkspaceFactory = new CadWorkspaceFactory();
                    IFeatureWorkspace pFeaWS = pWorkspaceFactory.OpenFromFile(pCADPath, 0) as IFeatureWorkspace;
                    IFeatureDataset pFeatureDataset = pFeaWS.OpenFeatureDataset(pCADName);
                    //Ifeatureclassscontainer can manage all feature classes
                    IFeatureClassContainer pContainer = pFeatureDataset as IFeatureClassContainer;

                    //add layer in different ways
                    MapAlgo.CleanAllInMap(axMapControlMain.Map, axMapControlMain.DocumentFilename, axMapControlMain.LayerCount);
                    for (int i = 0; i < pContainer.ClassCount; i++)
                    {
                        IFeatureClass pFeaClass = pContainer.get_Class(i);
                        //annotation layer or normal layer
                        if (pFeaClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                        {
                            IFeatureLayer pFeaLayer = new CadAnnotationLayerClass();
                            pFeaLayer.Name = pFeaClass.AliasName;
                            pFeaLayer.FeatureClass = pFeaClass;
                            axMapControlMain.Map.AddLayer(pFeaLayer);
                        }
                        else
                        {
                            IFeatureLayer pFeaLayer = new FeatureLayerClass();
                            pFeaLayer.Name = pFeaClass.AliasName;
                            pFeaLayer.FeatureClass = pFeaClass;
                            axMapControlMain.Map.AddLayer(pFeaLayer);
                        }
                        axMapControlMain.Map.Name = pCADName;
                        axMapControlMain.ActiveView.Refresh();
                    }
                    RefreshEagleMap();

                    //init the layers in editor
                    m_Map = axMapControlMain.Map;
                    pActiveView = m_Map as IActiveView;
                    plstLayers = MapAlgo.GetLayers(m_Map);

                    sLabelMap.Text = "Map:" + axMapControlMain.Map.Name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法打开Auto CAD文件\n" + ex.Message);
                }
            }
        }

        private void menuLocationTxt_Click(object sender, EventArgs e)
        {
            loadingTxtDialogForm theLoadingTxtDialogFrom = new loadingTxtDialogForm(axMapControlMain, sLabelWorking, sProgressBar, sLabelMap);
            theLoadingTxtDialogFrom.Show();

            //init the layers in editor
            m_Map = axMapControlMain.Map;
            pActiveView = m_Map as IActiveView;
            plstLayers = MapAlgo.GetLayers(m_Map);
        }

        private void menuRasterFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择栅格文件";
            ofd.Filter = "栅格文件(*.*)|*.bmp;*.tif;*.jpg;*.img|(*.bmp)|*.bmp|(*.tif)|*.tif|(*.jpg)|*.jpg|(*.img)|*img";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = ofd.FileName;
                int idx = fileName.LastIndexOf("\\");
                string rasterPath = fileName.Substring(0, idx);//get path
                string rasterName = fileName.Substring(idx + 1);//get single raster file name

                //transform to a raster layer
                IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactory();
                IWorkspace pWorkSpace = pWorkspaceFactory.OpenFromFile(rasterPath, 0);
                ILayer pLayer = MapAlgo.RasterHelper(pWorkSpace, rasterName);

                //add to mapControl and eagle map
                axMapControlMain.AddLayer(pLayer);
                axMapControlMain.Map.Name = rasterName;
                RefreshEagleMap();

                //init the layers in editor
                m_Map = axMapControlMain.Map;
                pActiveView = m_Map as IActiveView;
                plstLayers = MapAlgo.GetLayers(m_Map);

                sLabelMap.Text = "Map:" + axMapControlMain.Map.Name;
            }
        }

        private void menuFileGeodatabase_click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;
            string fullPath = fbd.SelectedPath;
            if (fullPath != "")
            {
                List<IFeatureLayer> pFLayers = null;
                IWorkspaceFactory pFileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                MapAlgo.CleanAllInMap(axMapControlMain.Map, axMapControlMain.DocumentFilename, axMapControlMain.LayerCount);
                IWorkspace pWorkspace = pFileGDBWorkspaceFactory.OpenFromFile(fullPath, 0);
                pFLayers = MapAlgo.DatabaseHelper(pWorkspace);
                foreach (var Flayer in pFLayers)
                    axMapControlMain.AddLayer(Flayer);

                //refreshing...
                axMapControlMain.Map.Name = fullPath.Substring(fullPath.LastIndexOf('\\') + 1);
                axMapControlMain.ActiveView.Refresh();
                RefreshEagleMap();

                //init the layers in editor
                m_Map = axMapControlMain.Map;
                pActiveView = m_Map as IActiveView;
                plstLayers = MapAlgo.GetLayers(m_Map);

                sLabelMap.Text = axMapControlMain.Map.Name;
            }
        }
        #endregion

        #region query and statics
        //select by attributes
        private void btnSelectByAttributes_Click(object sender, EventArgs e)
        {
            if (axMapControlMain.Map.LayerCount == 0)
            {
                MessageBox.Show("未发现图层");
                return;
            }
            selectByAttributesForm pSelectByAttributesForm = new selectByAttributesForm(axMapControlMain.Map, sLabelWorking, sProgressBar);
            pSelectByAttributesForm.Show();
        }

        //select by spatial relationship
        private void btnSelectBySpatialRelationship_Click(object sender, EventArgs e)
        {
            if (axMapControlMain.Map.LayerCount == 0)
            {
                MessageBox.Show("未发现图层");
                return;
            }
            selectBySpatialRelationshipForm pSelectBySpatialRelationshipForm = new selectBySpatialRelationshipForm(axMapControlMain.Map, sLabelWorking, sProgressBar);
            pSelectBySpatialRelationshipForm.Show();
        }
        #endregion

        #region bookmark
        //adding bookmark
        private void btnAddBookmark_Click(object sender, EventArgs e)
        {
            if (axMapControlMain.Map.LayerCount == 0)
            {
                MessageBox.Show("未发现图层");
                return;
            }
            AddBookmarkForm pAddBookmarkForm = new AddBookmarkForm();
            pAddBookmarkForm.ShowDialog();

            //if continue?
            if (!pAddBookmarkForm.IsAdding)
                return;

            //renaming juding...
            IMapBookmarks pMapBookmarks = axMapControlMain.Map as IMapBookmarks;
            IEnumSpatialBookmark pEnum = pMapBookmarks.Bookmarks;
            pEnum.Reset();
            ISpatialBookmark pBookmark; //temp!
            while ((pBookmark = pEnum.Next()) != null)
            {
                if (pBookmark.Name == pAddBookmarkForm.ReadBookmarkName)
                {
                    //show the dialog with yes|no|cancel
                    DialogResult dr = MessageBox.Show("标签名已重复，是否要替换？", "提示", MessageBoxButtons.YesNoCancel);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        pMapBookmarks.RemoveBookmark(pBookmark);
                    }
                    else if (dr == System.Windows.Forms.DialogResult.No)
                    {
                        pAddBookmarkForm.Show(); //retry
                    }
                    else
                        return; //end the process
                }
                else
                    continue;
            }

            //adding bookmark...
            IAOIBookmark pBookmarkAdding = new AOIBookmarkClass();
            pBookmarkAdding.Location = axMapControlMain.Extent; //setting extent
            pBookmarkAdding.Name = pAddBookmarkForm.ReadBookmarkName; //setting name
            IMapBookmarks pBookmarks = axMapControlMain.Map as IMapBookmarks;
            pBookmarks.AddBookmark(pBookmarkAdding);
        }

        //bookmarks manager
        private void btnBookmarkManager_Click(object sender, EventArgs e)
        {
            bookmarkManagerForm pBookmarkManagerForm = new bookmarkManagerForm(axMapControlMain.Map);
            pBookmarkManagerForm.Show();
        }
        #endregion

        #region front and post view
        private void btnFrontView_Click(object sender, EventArgs e)
        {
            IExtentStack pExtentStack = axMapControlMain.ActiveView.ExtentStack;
            if (pExtentStack.CanUndo())
            {
                pExtentStack.Undo();
                btnPostView.Enabled = true;
                if (!pExtentStack.CanUndo())
                {
                    btnFrontView.Enabled = false;
                }
            }
            axMapControlMain.ActiveView.Refresh();
        }

        private void btnPostView_Click(object sender, EventArgs e)
        {
            IExtentStack pExtentStack = axMapControlMain.ActiveView.ExtentStack;
            if (pExtentStack.CanRedo())
            {
                pExtentStack.Redo();
                btnFrontView.Enabled = true;
                if (!pExtentStack.CanRedo())
                {
                    btnPostView.Enabled = false;
                }
            }
            axMapControlMain.ActiveView.Refresh();
        }
        #endregion

        #region measure distance
        //function happened when form closed
        public void pMeasureResultForm_frmColsed()
        {
            //reset the line tracked
            if (pNewLineFeedback != null)
            {
                pNewLineFeedback.Stop();
                pNewLineFeedback = null;
            }

            //clean all marks
            axMapControlMain.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);

            //end the measure
            mouseState = mouseStateDict[0];
            axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerDefault;
        }

        private void btnMeasureDis_Click(object sender, EventArgs e)
        {
            axMapControlMain.CurrentTool = null;
            mouseState = mouseStateDict[1];
            axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            if (pMeasureResultForm == null || pMeasureResultForm.IsDisposed)
            {
                pMeasureResultForm = new measureResultForm();
                pMeasureResultForm.frmClosed += new measureResultForm.FormClosedEventHandler(pMeasureResultForm_frmColsed);
                pMeasureResultForm.lblMeasureRes.Text = "";
                pMeasureResultForm.lblTitle.Text = "距离量测";
                pMeasureResultForm.Show();
            }
            else
            {
                pMeasureResultForm.Activate();
            }
        }
        #endregion

        #region TOC right click...
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            try
            {
                esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap pMap = null;
                ILayer pLayer = null;
                object unk = null;
                object data = null;

                switch (e.button)
                {
                    case 1:
                        axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref unk, ref data);
                        pFocusFeatureLyr = pLayer as IFeatureLayer;
                        break;
                    case 2:
                        axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref unk, ref data);
                        pFocusFeatureLyr = pLayer as IFeatureLayer;
                        if (pItem == esriTOCControlItem.esriTOCControlItemLayer && pFocusFeatureLyr != null)
                        {
                            contextMenuStrip1.Show(Control.MousePosition);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region attributes manager
        //attribute static
        private void btnAttributeStatistic_Click(object sender, EventArgs e)
        {
            attributesStatisticForm pAttributesStatisticForm = new attributesStatisticForm(axMapControlMain.Map);
            pAttributesStatisticForm.Show();
        }

        //attributes viewing, deleted and editing
        private void btnAttributesViewing_Click(object sender, EventArgs e)
        {
            浏览属性表ToolStripMenuItem_Click(sender, e);
        }

        private void 浏览属性表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pFocusFeatureLyr == null)
            {
                MessageBox.Show("没有明确的图层,请在图层管理器选中浏览的图层！");
                return;
            }
            if (pAttributesViewingForm == null || pAttributesViewingForm.IsDisposed)
            {
                pAttributesViewingForm = new attributesViewingForm(axMapControlMain.ActiveView);
            }
            else
            {
                pAttributesViewingForm.Close();
                pAttributesViewingForm = new attributesViewingForm(axMapControlMain.ActiveView);
            }
            pAttributesViewingForm.lblSelectedClass.Text = pFocusFeatureLyr.Name;
            pAttributesViewingForm.CurFeatureLayer = pFocusFeatureLyr;
            pAttributesViewingForm.Show();
        }

        //edit attributes
        private void btnOprAttributes_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                ICommand m_AtrributeCom = new EditAtrributeToolClass();
                m_AtrributeCom.OnCreate(axMapControlMain.Object);
                m_AtrributeCom.OnClick();
                axMapControlMain.CurrentTool = m_AtrributeCom as ITool;
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerArrow;
            }
            catch (Exception ex)
            {
                MessageBox.Show("工具初始化失败\n" + ex.Message);
            }
        }
        #endregion

        #region symbolization
        private void btnPntSimple_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;
                ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbol();
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
                IRgbColor pRgbColor = new RgbColor();
                pRgbColor = MapAlgo.ColorRGBT(255, 100, 100);
                pMarkerSymbol.Color = pRgbColor;
                ISymbol pSymbol = (ISymbol)pMarkerSymbol;
                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pSymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch
            {
                MessageBox.Show("请输入有效图层!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
               
        private void btnPntArrow_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;
                IArrowMarkerSymbol pArrowMarkerSymbol = new ArrowMarkerSymbolClass();
                IRgbColor pRgbColor = new RgbColor();
                pRgbColor = MapAlgo.ColorRGBT(255, 100, 0);
                pArrowMarkerSymbol.Angle = 90;
                pArrowMarkerSymbol.Color = pRgbColor;
                pArrowMarkerSymbol.Length = 20;
                pArrowMarkerSymbol.Width = 10;
                pArrowMarkerSymbol.XOffset = 0;
                pArrowMarkerSymbol.YOffset = 0;
                pArrowMarkerSymbol.Style = esriArrowMarkerStyle.esriAMSPlain;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pArrowMarkerSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch
            {
                MessageBox.Show("请输入有效图层", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPntCharactor_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;
                ICharacterMarkerSymbol pCharacterMarkerSymbol = new CharacterMarkerSymbol();
                stdole.IFontDisp pFontDisp = (stdole.IFontDisp)(new stdole.StdFontClass());
                IRgbColor pRgbColor = new RgbColor();
                pRgbColor = MapAlgo.ColorRGBT(255, 0, 0);
                pFontDisp.Name = "arial";       
                pFontDisp.Italic = true;

                pCharacterMarkerSymbol.Angle = 0;
                pCharacterMarkerSymbol.CharacterIndex = 65;
                pCharacterMarkerSymbol.Color = pRgbColor;
                pCharacterMarkerSymbol.Font = pFontDisp;
                pCharacterMarkerSymbol.Size = 10;
                pCharacterMarkerSymbol.XOffset = 3;
                pCharacterMarkerSymbol.YOffset = 3;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pCharacterMarkerSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch
            {
                MessageBox.Show("请输入有效图层", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPntPiled_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;
                IMultiLayerMarkerSymbol pMultiLayerMarkerSymbol = new MultiLayerMarkerSymbolClass();
                IPictureMarkerSymbol pPictureMarkerSymbol = new PictureMarkerSymbolClass();
                ICharacterMarkerSymbol pCharacterMarkerSymbol = new CharacterMarkerSymbol();
                stdole.IFontDisp fontDisp = (stdole.IFontDisp)(new stdole.StdFontClass());
                IRgbColor pGgbColor = new RgbColor();
                pGgbColor = MapAlgo.ColorRGBT(0, 0, 0);
                fontDisp.Name = "arial";
                fontDisp.Size = 12;
                fontDisp.Italic = true;
                pCharacterMarkerSymbol.Angle = 0;
                pCharacterMarkerSymbol.CharacterIndex = 97;//字母a
                pCharacterMarkerSymbol.Color = pGgbColor;
                pCharacterMarkerSymbol.Font = fontDisp;
                pCharacterMarkerSymbol.Size = 24;
                string fileName = "\\Symbols\\city.bmp"; ;
                pPictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureBitmap, fileName);
                pPictureMarkerSymbol.Angle = 0;
                pPictureMarkerSymbol.BitmapTransparencyColor = pGgbColor;
                pPictureMarkerSymbol.Size = 10;
                pMultiLayerMarkerSymbol.AddLayer(pCharacterMarkerSymbol);
                pMultiLayerMarkerSymbol.AddLayer(pPictureMarkerSymbol);
                pMultiLayerMarkerSymbol.Angle = 0;
                pMultiLayerMarkerSymbol.Size = 20;
                pMultiLayerMarkerSymbol.XOffset = 5;
                pMultiLayerMarkerSymbol.YOffset = 5;
                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pMultiLayerMarkerSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch
            {
                MessageBox.Show("请输入有效图层", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLineSimple_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Width = 0;
                simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSInsideFrame;   
                simpleLineSymbol.Color = MapAlgo.ColorRGBT(255, 100, 0);
                ISymbol symbol = simpleLineSymbol as ISymbol;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = symbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLineCarto_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                ICartographicLineSymbol pCartographicLineSymbol = new CartographicLineSymbolClass();
                pCartographicLineSymbol.Cap = esriLineCapStyle.esriLCSRound;
                pCartographicLineSymbol.Join = esriLineJoinStyle.esriLJSRound; 
                pCartographicLineSymbol.Width = 2;

                ILineProperties pLineProperties;
                pLineProperties = pCartographicLineSymbol as ILineProperties;
                pLineProperties.Offset = 0;
                double[] dob = new double[6];
                dob[0] = 0;
                dob[1] = 1;
                dob[2] = 2;
                dob[3] = 3;
                dob[4] = 4;
                dob[5] = 5;
                ITemplate pTemplate = new TemplateClass();
                pTemplate.Interval = 1;
                for (int i = 0; i < dob.Length; i += 2)
                {
                    pTemplate.AddPatternElement(dob[i], dob[i + 1]);
                }
                pLineProperties.Template = pTemplate;
                IRgbColor pRgbColor = MapAlgo.ColorRGBT(0, 255, 0);
                pCartographicLineSymbol.Color = pRgbColor;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pCartographicLineSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLineLyrs_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;
                IMultiLayerLineSymbol pMultiLayerLineSymbol = new MultiLayerLineSymbolClass();
                ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSDashDotDot;
                pSimpleLineSymbol.Width = 2;
                IRgbColor pRgbColor = MapAlgo.ColorRGBT(255, 0, 0);
                pSimpleLineSymbol.Color = pRgbColor;
                ICartographicLineSymbol pCartographicLineSymbol = new CartographicLineSymbolClass();
                pCartographicLineSymbol.Cap = esriLineCapStyle.esriLCSRound;
                pCartographicLineSymbol.Join = esriLineJoinStyle.esriLJSRound;
                pCartographicLineSymbol.Width = 2;
                ILineProperties pLineProperties;
                pLineProperties = pCartographicLineSymbol as ILineProperties;
                pLineProperties.Offset = 0;
                double[] dob = new double[6];
                dob[0] = 0;
                dob[1] = 1;
                dob[2] = 2;
                dob[3] = 3;
                dob[4] = 4;
                dob[5] = 5;
                ITemplate pTemplate = new TemplateClass();
                pTemplate.Interval = 1;
                for (int i = 0; i < dob.Length; i += 2)
                {
                    pTemplate.AddPatternElement(dob[i], dob[i + 1]);
                }
                pLineProperties.Template = pTemplate;

                pRgbColor = MapAlgo.ColorRGBT(0, 255, 0);
                pCartographicLineSymbol.Color = pRgbColor;
                pMultiLayerLineSymbol.AddLayer(pSimpleLineSymbol);
                pMultiLayerLineSymbol.AddLayer(pCartographicLineSymbol);
                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pMultiLayerLineSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlgSimple_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSVertical;
                pSimpleFillSymbol.Color = MapAlgo.ColorRGBT(150, 150, 150);

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pSimpleFillSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlgLines_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSDashDotDot; 
                pSimpleLineSymbol.Width = 2;
                IRgbColor pRgbColor = MapAlgo.ColorRGBT(255, 0, 0);
                pSimpleLineSymbol.Color = pRgbColor;
                ILineFillSymbol pLineFillSymbol = new LineFillSymbol();
                pLineFillSymbol.Angle = 45;
                pLineFillSymbol.Separation = 10;
                pLineFillSymbol.Offset = 5;
                pLineFillSymbol.LineSymbol = pSimpleLineSymbol;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pLineFillSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlgPoints_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                IArrowMarkerSymbol pArrowMarkerSymbol = new ArrowMarkerSymbolClass();
                IRgbColor pRgbColor = MapAlgo.ColorRGBT(255, 0, 0);
                pArrowMarkerSymbol.Color = pRgbColor as IColor;
                pArrowMarkerSymbol.Length = 2;
                pArrowMarkerSymbol.Width = 2;
                pArrowMarkerSymbol.Style = esriArrowMarkerStyle.esriAMSPlain;

                IMarkerFillSymbol pMarkerFillSymbol = new MarkerFillSymbolClass();
                pMarkerFillSymbol.MarkerSymbol = pArrowMarkerSymbol;
                pRgbColor = MapAlgo.ColorRGBT(255, 0, 0);
                pMarkerFillSymbol.Color = pRgbColor;
                pMarkerFillSymbol.Style = esriMarkerFillStyle.esriMFSGrid;

                IFillProperties pFillProperties = pMarkerFillSymbol as IFillProperties;
                pFillProperties.XOffset = 2;
                pFillProperties.YOffset = 2;
                pFillProperties.XSeparation = 5;
                pFillProperties.YSeparation = 5;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pFillProperties as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlgGradian_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                IGradientFillSymbol pGradientFillSymbol = new GradientFillSymbolClass();
                IAlgorithmicColorRamp pAlgorithcColorRamp = new AlgorithmicColorRampClass();
                pAlgorithcColorRamp.FromColor = MapAlgo.ColorRGBT(255, 0, 0);
                pAlgorithcColorRamp.ToColor = MapAlgo.ColorRGBT(0, 255, 0);
                pAlgorithcColorRamp.Algorithm = esriColorRampAlgorithm.esriHSVAlgorithm;
                pGradientFillSymbol.ColorRamp = pAlgorithcColorRamp;
                pGradientFillSymbol.GradientAngle = 130;
                pGradientFillSymbol.GradientPercentage = 1;
                pGradientFillSymbol.IntervalCount = 5;
                pGradientFillSymbol.Style = esriGradientFillStyle.esriGFSLinear;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pGradientFillSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlgLyrs_Click(object sender, EventArgs e)
        {
            try
            {
                IGeoFeatureLayer pGeoFeatLyr = pFocusFeatureLyr as IGeoFeatureLayer;

                IMultiLayerFillSymbol pMultiLayerFillSymbol = new MultiLayerFillSymbolClass();
                IGradientFillSymbol pGradientFillSymbol = new GradientFillSymbolClass();
                IAlgorithmicColorRamp pAlgorithcColorRamp = new AlgorithmicColorRampClass();
                pAlgorithcColorRamp.FromColor = MapAlgo.ColorRGBT(255, 0, 0);
                pAlgorithcColorRamp.ToColor = MapAlgo.ColorRGBT(0, 255, 0);
                pAlgorithcColorRamp.Algorithm = esriColorRampAlgorithm.esriHSVAlgorithm;
                pGradientFillSymbol.ColorRamp = pAlgorithcColorRamp;
                pGradientFillSymbol.GradientAngle = 45;
                pGradientFillSymbol.GradientPercentage = 0.9;
                pGradientFillSymbol.Style = esriGradientFillStyle.esriGFSLinear;

                ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSDashDotDot;
                pSimpleLineSymbol.Width = 2;
                IRgbColor pRgbColor = MapAlgo.ColorRGBT(255, 0, 0);
                pSimpleLineSymbol.Color = pRgbColor;
                ILineFillSymbol pLineFillSymbol = new LineFillSymbol();
                pLineFillSymbol.Angle = 45;
                pLineFillSymbol.Separation = 10;
                pLineFillSymbol.Offset = 5;
                pLineFillSymbol.LineSymbol = pSimpleLineSymbol;

                pMultiLayerFillSymbol.AddLayer(pGradientFillSymbol);
                pMultiLayerFillSymbol.AddLayer(pLineFillSymbol);

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
                pSimpleRenderer.Symbol = pMultiLayerFillSymbol as ISymbol;
                pGeoFeatLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLblText_Click(object sender, EventArgs e)
        {
            try
            {
                if (pTextElementForm == null || pTextElementForm.IsDisposed)
                {
                    pTextElementForm = new textElementForm();
                    pTextElementForm.TextElement += new textElementForm.TextElementLabelEventHandler(frmTextElement_TextElement);
                }
                pTextElementForm.Map = axMapControlMain.Map;
                pTextElementForm.InitUI();
                pTextElementForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmTextElement_TextElement(string sFeatClsName, string sFieldName)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            TextElementLabel(pFeatLyr, sFieldName);
        }

        private void TextElementLabel(IFeatureLayer pFeatLyr, string sFieldName)
        {
            try
            {
                IMap pMap = axMapControlMain.Map;
                IFeatureClass pFeatureClass = pFeatLyr.FeatureClass;
                IFeatureCursor pFeatCursor = pFeatureClass.Search(null, true);
                IFeature pFeature = pFeatCursor.NextFeature();
                while (pFeature != null)
                {
                    IFields pFields = pFeature.Fields;
                    int index = pFields.FindField(sFieldName);
                    IEnvelope pEnve = pFeature.Extent;
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(pEnve.XMin + pEnve.Width / 2, pEnve.YMin + pEnve.Height / 2);
                    stdole.IFontDisp pFont;
                    pFont = new stdole.StdFontClass() as stdole.IFontDisp;
                    pFont.Name = "arial";
                    ITextSymbol pTextSymbol = new TextSymbolClass();
                    pTextSymbol.Size = 20;
                    pTextSymbol.Font = pFont;
                    pTextSymbol.Color = MapAlgo.ColorRGBT(255, 0, 0);
                    ITextElement pTextElement = new TextElementClass();
                    pTextElement.Text = pFeature.get_Value(index).ToString();
                    pTextElement.ScaleText = true;
                    pTextElement.Symbol = pTextSymbol;
                    IElement pElement = pTextElement as IElement;
                    pElement.Geometry = pPoint;
                    IActiveView pActiveView = pMap as IActiveView;
                    IGraphicsContainer pGraphicsContainer = pMap as IGraphicsContainer;
                    pGraphicsContainer.AddElement(pElement, 0);
                    pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    pPoint = null;
                    pElement = null;
                    pFeature = pFeatCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region cartography
        private void btnSymSingle_Click(object sender, EventArgs e)
        {
            if (pSymSingleForm == null || pSymSingleForm.IsDisposed)
            {
                pSymSingleForm = new symSingleForm();
                pSymSingleForm.SimpleRender += new symSingleForm.SimpleRenderEventHandler(symSingleForm_SimpleRender);
            }
            pSymSingleForm.PMap = axMapControlMain.Map;
            pSymSingleForm.InitUI();
            pSymSingleForm.ShowDialog();
        }

        private void symSingleForm_SimpleRender(string sFeatClsName, IRgbColor pRgbColr)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            SimpleRenderer(pFeatLyr, pRgbColr);
        }

        /// <summary>
        /// sym single
        /// </summary>
        /// <param name="pFeatLyr">target layer</param>
        /// <param name="pRgbColor">color</param>
        private void SimpleRenderer(IFeatureLayer pFeatLyr, IRgbColor pRgbColor)
        {
            esriGeometryType types = pFeatLyr.FeatureClass.ShapeType;
            ISimpleRenderer pSimRender = new SimpleRendererClass();
            if (types == esriGeometryType.esriGeometryPolygon)
            {
                ISimpleFillSymbol pSimFillSym = new SimpleFillSymbolClass();
                pSimFillSym.Color = pRgbColor;
                pSimRender.Symbol = pSimFillSym as ISymbol;
            }
            else if (types == esriGeometryType.esriGeometryPoint)
            {
                ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                pSimpleMarkerSymbol.Color = pRgbColor;
                pSimRender.Symbol = pSimpleMarkerSymbol as ISymbol;
            }
            else if (types == esriGeometryType.esriGeometryPolyline)
            {
                ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                pSimpleLineSymbol.Color = pRgbColor;
                pSimRender.Symbol = pSimpleLineSymbol as ISymbol;
            }
            IGeoFeatureLayer pGeoFeatLyr = pFeatLyr as IGeoFeatureLayer;
            pGeoFeatLyr.Renderer = pSimRender as IFeatureRenderer;
            (axMapControlMain.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            axMapControlMain.Update();
        }

        private void btnUniqueValSingle_Click(object sender, EventArgs e)
        {
            if (pUniqueValueRenForm == null || pUniqueValueRenForm.IsDisposed)
            {
                pUniqueValueRenForm = new uniqueValueRenForm();
                pUniqueValueRenForm.UniqueValueRender += new uniqueValueRenForm.UniqueValueRenderEventHandler(uniqueValueRenForm_UniqueValueRender);
            }
            pUniqueValueRenForm.Map = axMapControlMain.Map;
            pUniqueValueRenForm.InitUI();
            pUniqueValueRenForm.ShowDialog();
        }

        void uniqueValueRenForm_UniqueValueRender(string sFeatClsName, string sFieldName)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            UniqueValueRenderer(pFeatLyr, sFieldName);
        }
        private void UniqueValueRenderer(IFeatureLayer pFeatLyr, string sFieldName)
        {
            IGeoFeatureLayer pGeoFeatLyr = pFeatLyr as IGeoFeatureLayer;
            ITable pTable = pFeatLyr as ITable;
            IUniqueValueRenderer pUniqueValueRender = new UniqueValueRendererClass();

            int intFieldNumber = pTable.FindField(sFieldName);
            pUniqueValueRender.FieldCount = 1;
            pUniqueValueRender.set_Field(0, sFieldName);

            IRandomColorRamp pRandColorRamp = new RandomColorRampClass();
            pRandColorRamp.StartHue = 0;
            pRandColorRamp.MinValue = 0;
            pRandColorRamp.MinSaturation = 15;
            pRandColorRamp.EndHue = 360;
            pRandColorRamp.MaxValue = 100;
            pRandColorRamp.MaxSaturation = 30;

            IQueryFilter pQueryFilter = new QueryFilterClass();
            pRandColorRamp.Size = pFeatLyr.FeatureClass.FeatureCount(pQueryFilter);
            bool bSuccess = false;
            pRandColorRamp.CreateRamp(out bSuccess);

            IEnumColors pEnumRamp = pRandColorRamp.Colors;
            IColor pNextUniqueColor = null;

            pQueryFilter = new QueryFilterClass();
            pQueryFilter.AddField(sFieldName);
            ICursor pCursor = pTable.Search(pQueryFilter, true);
            IRow pNextRow = pCursor.NextRow();
            object codeValue = null;
            IRowBuffer pNextRowBuffer = null;


            while (pNextRow != null)
            {
                pNextRowBuffer = pNextRow as IRowBuffer;
                codeValue = pNextRowBuffer.get_Value(intFieldNumber);

                pNextUniqueColor = pEnumRamp.Next();
                if (pNextUniqueColor == null)
                {
                    pEnumRamp.Reset();
                    pNextUniqueColor = pEnumRamp.Next();
                }
                IFillSymbol pFillSymbol = null;
                ILineSymbol pLineSymbol;
                IMarkerSymbol pMarkerSymbol;
                switch (pGeoFeatLyr.FeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPolygon:
                        pFillSymbol = new SimpleFillSymbolClass();
                        pFillSymbol.Color = pNextUniqueColor;
                        pUniqueValueRender.AddValue(codeValue.ToString(), "", pFillSymbol as ISymbol);
                        pNextRow = pCursor.NextRow();
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        pLineSymbol = new SimpleLineSymbolClass();
                        pLineSymbol.Color = pNextUniqueColor;
                        pUniqueValueRender.AddValue(codeValue.ToString(), "", pLineSymbol as ISymbol);
                        pNextRow = pCursor.NextRow();
                        break;
                    case esriGeometryType.esriGeometryPoint:
                        pMarkerSymbol = new SimpleMarkerSymbolClass();
                        pMarkerSymbol.Color = pNextUniqueColor;
                        pUniqueValueRender.AddValue(codeValue.ToString(), "", pMarkerSymbol as ISymbol);
                        pNextRow = pCursor.NextRow();
                        break;
                }
            }
            pGeoFeatLyr.Renderer = pUniqueValueRender as IFeatureRenderer;
            axMapControlMain.Refresh();
            axTOCControl1.Update();
        }

        private void btnUniqueValDual_Click(object sender, EventArgs e)
        {
            if (pUniqueValueDualFields == null || pUniqueValueDualFields.IsDisposed)
            {
                pUniqueValueDualFields = new uniqueValueDualFields();
                pUniqueValueDualFields.UniqueValueRender += new uniqueValueDualFields.UniqueValueRenderEventHandler(uniqueValueDualFields_UniqueValueRender);
            }

            pUniqueValueDualFields.Map = axMapControlMain.Map;
            pUniqueValueDualFields.InitUI();
            pUniqueValueDualFields.ShowDialog();
        }

        void uniqueValueDualFields_UniqueValueRender(string sFeatClsName, string[] sFieldName)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            UniqueValueMany_fieldsRenderer(pFeatLyr, sFieldName);
        }
        private void UniqueValueMany_fieldsRenderer(IFeatureLayer pFeatLyr, string[] sFieldName)
        {
            IUniqueValueRenderer pUniqueValueRender;
            IColor pNextUniqueColor;
            IEnumColors pEnumRamp;
            ITable pTable;
            IRow pNextRow;
            ICursor pCursor;
            IQueryFilter pQueryFilter;
            IRandomColorRamp pRandColorRamp = new RandomColorRampClass();
            pRandColorRamp.StartHue = 0;
            pRandColorRamp.MinValue = 0;
            pRandColorRamp.MinSaturation = 15;
            pRandColorRamp.EndHue = 360;
            pRandColorRamp.MaxValue = 100;
            pRandColorRamp.MaxSaturation = 30;

            IQueryFilter pQueryFilter1 = new QueryFilterClass();
            pRandColorRamp.Size = pFeatLyr.FeatureClass.FeatureCount(pQueryFilter1);
            bool bSuccess = false;
            pRandColorRamp.CreateRamp(out bSuccess);

            if (sFieldName.Length == 2)
            {
                string sFieldName1 = sFieldName[0];
                string sFieldName2 = sFieldName[1];
                IGeoFeatureLayer pGeoFeatureL = (IGeoFeatureLayer)pFeatLyr;
                pUniqueValueRender = new UniqueValueRendererClass();
                pTable = (ITable)pGeoFeatureL;
                int pFieldNumber = pTable.FindField(sFieldName1);
                int pFieldNumber2 = pTable.FindField(sFieldName2);
                pUniqueValueRender.FieldCount = 2;
                pUniqueValueRender.set_Field(0, sFieldName1);
                pUniqueValueRender.set_Field(1, sFieldName2);
                pEnumRamp = pRandColorRamp.Colors;
                pNextUniqueColor = null;

                pQueryFilter = new QueryFilterClass();
                pQueryFilter.AddField(sFieldName1);
                pQueryFilter.AddField(sFieldName2);
                pCursor = pTable.Search(pQueryFilter, true);
                pNextRow = pCursor.NextRow();
                string codeValue;
                while (pNextRow != null)
                {
                    codeValue = pNextRow.get_Value(pFieldNumber).ToString() + pUniqueValueRender.FieldDelimiter + pNextRow.get_Value(pFieldNumber2).ToString();
                    pNextUniqueColor = pEnumRamp.Next();
                    if (pNextUniqueColor == null)
                    {
                        pEnumRamp.Reset();
                        pNextUniqueColor = pEnumRamp.Next();
                    }
                    IFillSymbol pFillSymbol;
                    ILineSymbol pLineSymbol;
                    IMarkerSymbol pMarkerSymbol;
                    switch (pGeoFeatureL.FeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPolygon:
                            {
                                pFillSymbol = new SimpleFillSymbolClass();
                                pFillSymbol.Color = pNextUniqueColor;

                                pUniqueValueRender.AddValue(codeValue, sFieldName1 + " " + sFieldName2, (ISymbol)pFillSymbol);
                                break;
                            }
                        case esriGeometryType.esriGeometryPolyline:
                            {
                                pLineSymbol = new SimpleLineSymbolClass();
                                pLineSymbol.Color = pNextUniqueColor;

                                pUniqueValueRender.AddValue(codeValue, sFieldName1 + " " + sFieldName2, (ISymbol)pLineSymbol);
                                break;
                            }
                        case esriGeometryType.esriGeometryPoint:
                            {
                                pMarkerSymbol = new SimpleMarkerSymbolClass();
                                pMarkerSymbol.Color = pNextUniqueColor;

                                pUniqueValueRender.AddValue(codeValue, sFieldName1 + " " + sFieldName2, (ISymbol)pMarkerSymbol);
                                break;
                            }
                    }
                    pNextRow = pCursor.NextRow();
                }
                pGeoFeatureL.Renderer = (IFeatureRenderer)pUniqueValueRender;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
            else if (sFieldName.Length == 3)
            {
                string sFieldName1 = sFieldName[0];
                string sFieldName2 = sFieldName[1];
                string sFieldName3 = sFieldName[2];
                IGeoFeatureLayer pGeoFeatureL = (IGeoFeatureLayer)pFeatLyr;
                pUniqueValueRender = new UniqueValueRendererClass();
                pTable = (ITable)pGeoFeatureL;
                int pFieldNumber = pTable.FindField(sFieldName1);
                int pFieldNumber2 = pTable.FindField(sFieldName2);
                int pFieldNumber3 = pTable.FindField(sFieldName3);
                pUniqueValueRender.FieldCount = 3;
                pUniqueValueRender.set_Field(0, sFieldName1);
                pUniqueValueRender.set_Field(1, sFieldName2);
                pUniqueValueRender.set_Field(2, sFieldName3);
                pEnumRamp = pRandColorRamp.Colors;
                pNextUniqueColor = null;
                pQueryFilter = new QueryFilterClass();
                pQueryFilter.AddField(sFieldName1);
                pQueryFilter.AddField(sFieldName2);
                pQueryFilter.AddField(sFieldName3);
                pCursor = pTable.Search(pQueryFilter, true);
                pNextRow = pCursor.NextRow();
                string codeValue;
                while (pNextRow != null)
                {
                    codeValue = pNextRow.get_Value(pFieldNumber).ToString() + pUniqueValueRender.FieldDelimiter + pNextRow.get_Value(pFieldNumber2).ToString() + pUniqueValueRender.FieldDelimiter + pNextRow.get_Value(pFieldNumber3).ToString();
                    pNextUniqueColor = pEnumRamp.Next();
                    if (pNextUniqueColor == null)
                    {
                        pEnumRamp.Reset();
                        pNextUniqueColor = pEnumRamp.Next();
                    }
                    IFillSymbol pFillSymbol;
                    ILineSymbol pLineSymbol;
                    IMarkerSymbol pMarkerSymbol;
                    switch (pGeoFeatureL.FeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPolygon:
                            {
                                pFillSymbol = new SimpleFillSymbolClass();
                                pFillSymbol.Color = pNextUniqueColor;
                                pUniqueValueRender.AddValue(codeValue, sFieldName1 + " " + sFieldName2 + "" + sFieldName3, (ISymbol)pFillSymbol);
                                break;
                            }
                        case esriGeometryType.esriGeometryPolyline:
                            {
                                pLineSymbol = new SimpleLineSymbolClass();
                                pLineSymbol.Color = pNextUniqueColor;
                                pUniqueValueRender.AddValue(codeValue, sFieldName1 + " " + sFieldName2 + "" + sFieldName3, (ISymbol)pLineSymbol);
                                break;
                            }
                        case esriGeometryType.esriGeometryPoint:
                            {
                                pMarkerSymbol = new SimpleMarkerSymbolClass();
                                pMarkerSymbol.Color = pNextUniqueColor;
                                pUniqueValueRender.AddValue(codeValue, sFieldName1 + " " + sFieldName2 + "" + sFieldName3, (ISymbol)pMarkerSymbol);
                                break;
                            }
                    }
                    pNextRow = pCursor.NextRow();
                }
                pGeoFeatureL.Renderer = (IFeatureRenderer)pUniqueValueRender;
                axMapControlMain.Refresh();
                axMapControlMain.Update();
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (pGraduatedcolorsForm == null || pGraduatedcolorsForm.IsDisposed)
            {
                pGraduatedcolorsForm = new graduatedcolorsForm();
                pGraduatedcolorsForm.Graduatedcolors += new graduatedcolorsForm.GraduatedcolorsEventHandler(graduatedcolorsForm_Graduatedcolors);
            }
            pGraduatedcolorsForm.Map = axMapControlMain.Map;
            pGraduatedcolorsForm.InitUI();
            pGraduatedcolorsForm.ShowDialog();
        }

        void graduatedcolorsForm_Graduatedcolors(string sFeatClsName, string sFieldName, int numclasses)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            GraduatedColors(pFeatLyr, sFieldName, numclasses);
        }

        /// <summary>
        /// classified colors
        /// </summary>
        /// <param name="pFeatLyr">layer</param>
        /// <param name="sFieldName">fields/param>
        /// <param name="numclasses">classe num</param>
        public void GraduatedColors(IFeatureLayer pFeatLyr, string sFieldName, int numclasses)
        {
            IGeoFeatureLayer pGeoFeatureL = pFeatLyr as IGeoFeatureLayer;
            object dataFrequency;
            object dataValues;
            bool ok;
            int breakIndex;

            ITable pTable = pGeoFeatureL.FeatureClass as ITable;
            ITableHistogram pTableHistogram = new BasicTableHistogramClass();
            IBasicHistogram pBasicHistogram = (IBasicHistogram)pTableHistogram;
            pTableHistogram.Field = sFieldName;
            pTableHistogram.Table = pTable;
            pBasicHistogram.GetHistogram(out dataValues, out dataFrequency);
            IClassifyGEN pClassify = new EqualIntervalClass();
            pClassify.Classify(dataValues, dataFrequency, ref  numclasses);

            double[] Classes = pClassify.ClassBreaks as double[];
            int ClassesCount = Classes.GetUpperBound(0);
            IClassBreaksRenderer pClassBreaksRenderer = new ClassBreaksRendererClass();
            pClassBreaksRenderer.Field = sFieldName;
            pClassBreaksRenderer.BreakCount = ClassesCount;
            pClassBreaksRenderer.SortClassesAscending = true;

            IHsvColor pFromColor = new HsvColorClass();
            pFromColor.Hue = 0;
            pFromColor.Saturation = 50;
            pFromColor.Value = 96;
            IHsvColor pToColor = new HsvColorClass();
            pToColor.Hue = 80;
            pToColor.Saturation = 100;
            pToColor.Value = 96;

            IAlgorithmicColorRamp pAlgorithmicCR = new AlgorithmicColorRampClass();
            pAlgorithmicCR.Algorithm = esriColorRampAlgorithm.esriHSVAlgorithm;
            pAlgorithmicCR.FromColor = pFromColor;
            pAlgorithmicCR.ToColor = pToColor;
            pAlgorithmicCR.Size = ClassesCount;
            pAlgorithmicCR.CreateRamp(out ok);

            IEnumColors pEnumColors = pAlgorithmicCR.Colors;

            for (breakIndex = 0; breakIndex <= ClassesCount - 1; breakIndex++)
            {
                IColor pColor = pEnumColors.Next();
                switch (pGeoFeatureL.FeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPolygon:
                        {
                            ISimpleFillSymbol pSimpleFillS = new SimpleFillSymbolClass();
                            pSimpleFillS.Color = pColor;
                            pSimpleFillS.Style = esriSimpleFillStyle.esriSFSSolid;
                            pClassBreaksRenderer.set_Symbol(breakIndex, (ISymbol)pSimpleFillS);
                            pClassBreaksRenderer.set_Break(breakIndex, Classes[breakIndex + 1]);
                            break;
                        }
                    case esriGeometryType.esriGeometryPolyline:
                        {
                            ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                            pSimpleLineSymbol.Color = pColor;
                            pClassBreaksRenderer.set_Symbol(breakIndex, (ISymbol)pSimpleLineSymbol);
                            pClassBreaksRenderer.set_Break(breakIndex, Classes[breakIndex + 1]);
                            break;
                        }
                    case esriGeometryType.esriGeometryPoint:
                        {
                            ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                            pSimpleMarkerSymbol.Color = pColor;
                            pClassBreaksRenderer.set_Symbol(breakIndex, (ISymbol)pSimpleMarkerSymbol);
                            pClassBreaksRenderer.set_Break(breakIndex, Classes[breakIndex + 1]);
                            break;
                        }
                }
            }
            pGeoFeatureL.Renderer = (IFeatureRenderer)pClassBreaksRenderer;
            axMapControlMain.Refresh();
            axMapControlMain.Update();
        }

        private void btnSymbol_Click(object sender, EventArgs e)
        {
            if (pGraduatedSymbolsForm == null || pGraduatedSymbolsForm.IsDisposed)
            {
                pGraduatedSymbolsForm = new graduatedSymbolsForm();
                pGraduatedSymbolsForm.GraduatedSymbols += new graduatedSymbolsForm.GraduatedSymbolsEventHandler(graduatedSymbolsForm_GraduatedSymbols);
            }
            pGraduatedSymbolsForm.Map = axMapControlMain.Map;
            pGraduatedSymbolsForm.InitUI();
            pGraduatedSymbolsForm.ShowDialog();
        }

        void graduatedSymbolsForm_GraduatedSymbols(string sFeatClsName, string sFieldName, int numclasses)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            GraduatedSymbols(pFeatLyr, sFieldName, numclasses);
        }

        /// <summary>
        /// classes symbols
        /// </summary>
        /// <param name="pFeatLyr">layer</param>
        /// <param name="sFieldName">fields</param>
        /// <param name="numclasses">classes num</param>
        public void GraduatedSymbols(IFeatureLayer pFeatLyr, string sFieldName, int numclasses)
        {
            ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
            pSimpleMarkerSymbol.Color = MapAlgo.ColorRGBT(255, 100, 100);
            ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
            pSimpleLineSymbol.Color = MapAlgo.ColorRGBT(255, 100, 100);
            int IbreakIndex;
            object dataFrequency;
            object dataValues;

            IGeoFeatureLayer pGeoFeatureL = pFeatLyr as IGeoFeatureLayer;
            ITable pTable = pGeoFeatureL.FeatureClass as ITable;
            ITableHistogram pTableHistogram = new BasicTableHistogramClass();
            IBasicHistogram pBasicHistogram = (IBasicHistogram)pTableHistogram;
            pTableHistogram.Field = sFieldName;
            pTableHistogram.Table = pTable;
            pBasicHistogram.GetHistogram(out dataValues, out dataFrequency);              
            IClassifyGEN pClassify = new EqualIntervalClass();
            pClassify.Classify(dataValues, dataFrequency, ref numclasses); 

            double[] Classes = (double[])pClassify.ClassBreaks;
            int ClassesCount = Classes.GetUpperBound(0);
            IClassBreaksRenderer pClassBreakRenderer = new ClassBreaksRendererClass();
            pClassBreakRenderer.Field = sFieldName; 

            pClassBreakRenderer.BreakCount = ClassesCount; 
            pClassBreakRenderer.SortClassesAscending = true; 

            double symbolSizeOrigin = 5.0;
            if (ClassesCount <= 5)
            {
                symbolSizeOrigin = 8;
            }
            if (ClassesCount < 10 && ClassesCount > 5)
            {
                symbolSizeOrigin = 7;
            }
            IFillSymbol pBackgroundSymbol = new SimpleFillSymbolClass();
            pBackgroundSymbol.Color = MapAlgo.ColorRGBT(255, 255, 100);

            switch (pGeoFeatureL.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    {
                        for (IbreakIndex = 0; IbreakIndex <= ClassesCount - 1; IbreakIndex++)
                        {
                            pClassBreakRenderer.set_Break(IbreakIndex, Classes[IbreakIndex + 1]);
                            pClassBreakRenderer.BackgroundSymbol = pBackgroundSymbol;
                            pSimpleMarkerSymbol.Size = symbolSizeOrigin + IbreakIndex * symbolSizeOrigin / 3.0d;
                            pClassBreakRenderer.set_Symbol(IbreakIndex, (ISymbol)pSimpleMarkerSymbol);
                        }
                        break;
                    }
                case esriGeometryType.esriGeometryPolyline:
                    {
                        for (IbreakIndex = 0; IbreakIndex <= ClassesCount - 1; IbreakIndex++)
                        {
                            pClassBreakRenderer.set_Break(IbreakIndex, Classes[IbreakIndex + 1]);
                            pSimpleLineSymbol.Width = symbolSizeOrigin / 5 + IbreakIndex * (symbolSizeOrigin / 5) / 5.0d;
                            pClassBreakRenderer.set_Symbol(IbreakIndex, (ISymbol)pSimpleLineSymbol);
                        }
                        break;
                    }
                case esriGeometryType.esriGeometryPoint:
                    {
                        for (IbreakIndex = 0; IbreakIndex <= ClassesCount - 1; IbreakIndex++)
                        {
                            pClassBreakRenderer.set_Break(IbreakIndex, Classes[IbreakIndex + 1]);
                            pSimpleMarkerSymbol.Size = symbolSizeOrigin + IbreakIndex * symbolSizeOrigin / 3.0d;
                            pClassBreakRenderer.set_Symbol(IbreakIndex, (ISymbol)pSimpleMarkerSymbol);
                        }
                        break;
                    }
            }
            pGeoFeatureL.Renderer = pClassBreakRenderer as IFeatureRenderer;
            axMapControlMain.ActiveView.Refresh();
            axTOCControl1.Update();
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            if (pProportionalForm == null || pProportionalForm.IsDisposed)
            {
                pProportionalForm = new proportionalForm();
                pProportionalForm.Proportional += new proportionalForm.ProportionalEventHandler(proportionalForm_Proportional);
            }
            pProportionalForm.Map = axMapControlMain.Map;
            pProportionalForm.InitUI();
            pProportionalForm.ShowDialog();
        }

        void proportionalForm_Proportional(string sFeatClsName, string sFieldName)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            Proportional(pFeatLyr, sFieldName);
        }

        /// <summary>
        /// scale symbol
        /// </summary>
        /// <param name="sender">layer</param>
        /// <param name="e">fields</param>        
        private void Proportional(IFeatureLayer pFeatLyr, string sFieldName)
        {
            IGeoFeatureLayer pGeoFeatureLayer = pFeatLyr as IGeoFeatureLayer;
            ITable pTable = pFeatLyr as ITable;
            ICursor pCursor = pTable.Search(null, true);

            IDataStatistics pDataStatistics = new DataStatisticsClass();
            pDataStatistics.Cursor = pCursor;
            pDataStatistics.Field = sFieldName;
            IStatisticsResults pStatisticsResult = pDataStatistics.Statistics;
            if (pStatisticsResult != null)
            {
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = MapAlgo.ColorRGBT(155, 255, 0);

                ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                pSimpleMarkerSymbol.Size = 3;
                pSimpleMarkerSymbol.Color = MapAlgo.ColorRGBT(255, 90, 0);
                IProportionalSymbolRenderer pProportionalSymbolRenderer = new ProportionalSymbolRendererClass();
                pProportionalSymbolRenderer.ValueUnit = esriUnits.esriUnknownUnits; 
                pProportionalSymbolRenderer.Field = sFieldName; 
                pProportionalSymbolRenderer.FlanneryCompensation = false; 
                pProportionalSymbolRenderer.MinDataValue = pStatisticsResult.Minimum; 
                pProportionalSymbolRenderer.MaxDataValue = pStatisticsResult.Maximum; 
                pProportionalSymbolRenderer.BackgroundSymbol = pFillSymbol;
                pProportionalSymbolRenderer.MinSymbol = pSimpleMarkerSymbol as ISymbol; 
                pProportionalSymbolRenderer.LegendSymbolCount = 5; 
                pProportionalSymbolRenderer.CreateLegendSymbols(); 
                pGeoFeatureLayer.Renderer = pProportionalSymbolRenderer as IFeatureRenderer;
            }
            axMapControlMain.Refresh();
            axMapControlMain.Update();
        }

        private void btnDense_Click(object sender, EventArgs e)
        {
            if (pDotDensityForm == null || pDotDensityForm.IsDisposed)
            {
                pDotDensityForm = new dotDensityForm();
                pDotDensityForm.DotDensity += new dotDensityForm.DotDensityEventHandler(dotDensityForm_DotDensity);
            }
            pDotDensityForm.Map = axMapControlMain.Map;
            pDotDensityForm.InitUI();
            pDotDensityForm.ShowDialog();
        }

        void dotDensityForm_DotDensity(string sFeatClsName, string sFieldName, int intRendererDensity)
        {
            IFeatureLayer pFeatLyr = MapAlgo.GetLayerFromName(axMapControlMain.Map, sFeatClsName);
            DotDensity(pFeatLyr, sFieldName, intRendererDensity);
        }

        /// <summary>
        /// dot dense
        /// </summary>
        /// <param name="pFeatLyr">layer</param>
        /// <param name="sFieldName">fields</param>
        /// <param name="intRendererDensity">density of points</param>           
        private void DotDensity(IFeatureLayer pFeatLyr, string sFieldName, int intRendererDensity)
        {
            IGeoFeatureLayer pGeoFeatureLayer = pFeatLyr as IGeoFeatureLayer;
            IDotDensityRenderer pDotDensityRenderer = new DotDensityRendererClass();
            IRendererFields pRendererFields = pDotDensityRenderer as IRendererFields;

            pRendererFields.AddField(sFieldName);

            IDotDensityFillSymbol pDotDensityFillSymbol = new DotDensityFillSymbolClass();
            pDotDensityFillSymbol.DotSize = 3;
            pDotDensityFillSymbol.BackgroundColor = MapAlgo.ColorRGBT(0, 255, 0);

            ISymbolArray pSymbolArray = pDotDensityFillSymbol as ISymbolArray;
            ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
            pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            pSimpleMarkerSymbol.Color = MapAlgo.ColorRGBT(0, 0, 255);
            pSymbolArray.AddSymbol(pSimpleMarkerSymbol as ISymbol);
            pDotDensityRenderer.DotDensitySymbol = pDotDensityFillSymbol;

            pDotDensityRenderer.DotValue = intRendererDensity;
            pDotDensityRenderer.CreateLegend();

            pGeoFeatureLayer.Renderer = pDotDensityRenderer as IFeatureRenderer;
            axMapControlMain.Refresh();
            axTOCControl1.Update();
        }
        #endregion

        #region editor tools
        private void ChangeButtonState(bool bEnable)
        {
            btnStartEdit.Enabled = !bEnable;
            btnSaveEdit.Enabled = bEnable;
            btnStopEdit.Enabled = bEnable;

            cobSelectedLayer.Enabled = bEnable;

            btnMoveFeature.Enabled = bEnable;
            btnAddFeatureClass.Enabled = bEnable;
            btnDelFeatureClass.Enabled = bEnable;
            btnOprAttributes.Enabled = bEnable;
            btnUndo.Enabled = bEnable;
            btnRedo.Enabled = bEnable;
        }

        private void InitCobEditLyrs(List<ILayer> plstLyrs)
        {
            cobSelectedLayer.Items.Clear();
            for (int i = 0; i < plstLyrs.Count; i++)
            {
                if (!cobSelectedLayer.Items.Contains(plstLyrs[i].Name))
                {
                    cobSelectedLayer.Items.Add(plstLyrs[i].Name);
                }
            }
            if (cobSelectedLayer.Items.Count != 0) cobSelectedLayer.SelectedIndex = 0;
        }

        private void 开始编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (plstLayers == null || plstLayers.Count == 0)
                {
                    MessageBox.Show("请加载编辑图层！", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                m_Map.ClearSelection();
                pActiveView.Refresh();
                InitCobEditLyrs(plstLayers);
                ChangeButtonState(true);
                if (pEngineEditor.EditState != esriEngineEditState.esriEngineStateNotEditing)
                    return;
                if (pCurrentLyr == null) return;
                IDataset pDataSet = pCurrentLyr.FeatureClass as IDataset;
                IWorkspace pWs = pDataSet.Workspace;
                if (pWs.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    pEngineEditor.EditSessionMode = esriEngineEditSessionMode.esriEngineEditSessionModeVersioned;
                }
                else
                {
                    pEngineEditor.EditSessionMode = esriEngineEditSessionMode.esriEngineEditSessionModeNonVersioned;
                }
                pEngineEditTask = pEngineEditor.GetTaskByUniqueName("ControlToolsEditing_CreateNewFeatureTask");
                pEngineEditor.CurrentTask = pEngineEditTask;
                pEngineEditor.EnableUndoRedo(true);
                pEngineEditor.StartEditing(pWs, m_Map);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ICommand m_saveEditCom = new SaveEditCommandClass();
                m_saveEditCom.OnCreate(axMapControlMain.Object);
                m_saveEditCom.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStopEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ICommand m_stopEditCom = new StopEditCommandClass();
                m_stopEditCom.OnCreate(axMapControlMain.Object);
                m_stopEditCom.OnClick();
                ChangeButtonState(false);
                axMapControlMain.CurrentTool = null;
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cobSelectedLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sLyrName = cobSelectedLayer.SelectedItem.ToString();
                pCurrentLyr = MapAlgo.GetLayerFromName(m_Map, sLyrName) as IFeatureLayer;
                pEngineEditLayers.SetTargetLayer(pCurrentLyr, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 缩放到图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IEnvelope envelope = new EnvelopeClass();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = "";
            IFeatureCursor featureCursor = pFocusFeatureLyr.FeatureClass.Search(queryFilter, true);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                IGeometry geometry = feature.Shape;
                IEnvelope featureExtent = geometry.Envelope;
                envelope.Union(featureExtent);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(feature);
                feature = featureCursor.NextFeature();
            }
            axMapControlMain.ActiveView.FullExtent = envelope;
            axMapControlMain.ActiveView.Refresh();
        }

        private void 移除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControlMain.Map.DeleteLayer(pFocusFeatureLyr);
            axMapControlMain.ActiveView.Refresh();
        }

        //move features
        private void btnMoveFeature_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                ICommand m_moveTool = new MoveFeatureToolClass();
                m_moveTool.OnCreate(axMapControlMain.Object);
                m_moveTool.OnClick();
                axMapControlMain.CurrentTool = m_moveTool as ITool;
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerArrow;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //add feature class
        private void btnAddFeatureClass_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                ICommand m_CreateFeatTool = new CreateFeatureToolClass();
                m_CreateFeatTool.OnCreate(axMapControlMain.Object);
                m_CreateFeatTool.OnClick();
                axMapControlMain.CurrentTool = m_CreateFeatTool as ITool;
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //delete a feature class
        private void btnDelFeatureClass_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerArrow;
                ICommand m_delFeatCom = new DelFeatureCommandClass();
                m_delFeatCom.OnCreate(axMapControlMain.Object);
                m_delFeatCom.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //undo
        private void btnUndo_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerArrow;
                ICommand m_undoCommand = new UndoCommandClass();
                m_undoCommand.OnCreate(axMapControlMain.Object);
                m_undoCommand.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //redo
        private void btnRedo_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                axMapControlMain.MousePointer = esriControlsMousePointer.esriPointerArrow;
                ICommand m_redoCommand = new RedoCommandClass();
                m_redoCommand.OnCreate(axMapControlMain.Object);
                m_redoCommand.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region spatial analysis
        private void btnBufferAna_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                ICommand pCommand = new ToolBufferAnalysis();
                pCommand.OnCreate(this.axMapControlMain.Object);
                this.axMapControlMain.CurrentTool = pCommand as ITool;
                pCommand = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBoundaryAna_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                ICommand pCommand = new ToolGetBoundary();
                pCommand.OnCreate(this.axMapControlMain.Object);
                this.axMapControlMain.CurrentTool = pCommand as ITool;
                pCommand = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNearFeatures_Click(object sender, EventArgs e)
        {
            try
            {
                mouseState = mouseStateDict[5];
                ICommand pCommand = new ToolGetNearFeature();
                pCommand.OnCreate(this.axMapControlMain.Object);
                this.axMapControlMain.CurrentTool = pCommand as ITool;
                pCommand = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region page layout
        private void btnNorthArrow_Click(object sender, EventArgs e)
        {
            try
            {
                _enumMapSurType = EnumMapSurroundType.NorthArrow;
                if (pSymbolForm == null || pSymbolForm.IsDisposed)
                {
                    pSymbolForm = new symbolForm();
                    pSymbolForm.GetSelSymbolItem += new symbolForm.GetSelSymbolItemEventHandler(symbolForm_GetSelSymbolItem);
                }
                pSymbolForm.EnumMapSurType = _enumMapSurType;
                pSymbolForm.InitUI();
                pSymbolForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void symbolForm_GetSelSymbolItem(ref IStyleGalleryItem pStyleItem)
        {
            pStyleGalleryItem = pStyleItem;
        }

        private void btnLegend_Click(object sender, EventArgs e)
        {
            try
            {
                _enumMapSurType = EnumMapSurroundType.Legend;
            }
            catch (Exception ex) { }
        }

        private void btnScaleBar_Click(object sender, EventArgs e)
        {
            try
            {
                _enumMapSurType = EnumMapSurroundType.ScaleBar;
                if (pSymbolForm == null || pSymbolForm.IsDisposed)
                {
                    pSymbolForm = new symbolForm();
                    pSymbolForm.GetSelSymbolItem += new symbolForm.GetSelSymbolItemEventHandler(symbolForm_GetSelSymbolItem);
                }
                pSymbolForm.EnumMapSurType = _enumMapSurType;
                pSymbolForm.InitUI();
                pSymbolForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region page layout events
        private void axPageLayoutControl1_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            try
            {
                if (_enumMapSurType != EnumMapSurroundType.None)
                {
                    IActiveView pActiveView = null;
                    pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                    m_PointPt = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                    if (pNewEnvelopeFeedback == null)
                    {
                        pNewEnvelopeFeedback = new NewEnvelopeFeedbackClass();
                        pNewEnvelopeFeedback.Display = pActiveView.ScreenDisplay;
                        pNewEnvelopeFeedback.Start(m_PointPt);
                    }
                    else
                    {
                        pNewEnvelopeFeedback.MoveTo(m_PointPt);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            try
            {
                if (_enumMapSurType != EnumMapSurroundType.None)
                {
                    if (pNewEnvelopeFeedback != null)
                    {
                        m_MovePt = (axPageLayoutControl1.PageLayout as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                        pNewEnvelopeFeedback.MoveTo(m_MovePt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void axPageLayoutControl1_OnMouseUp(object sender, IPageLayoutControlEvents_OnMouseUpEvent e)
        {
            if (_enumMapSurType != EnumMapSurroundType.None)
            {
                if (pNewEnvelopeFeedback != null)
                {
                    IActiveView pActiveView = null;
                    pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                    IEnvelope pEnvelope = pNewEnvelopeFeedback.Stop();
                    AddMapSurround(pActiveView, _enumMapSurType, pEnvelope);
                    pNewEnvelopeFeedback = null;
                    _enumMapSurType = EnumMapSurroundType.None;
                }
            }
        }
        #endregion

        //trigger of adding events
        private void AddMapSurround(IActiveView pActiveView, EnumMapSurroundType _enumMapSurroundType, IEnvelope pEnvelope)
        {
            try
            {
                switch (_enumMapSurroundType)
                {
                    case EnumMapSurroundType.NorthArrow:
                        addNorthArrow(axPageLayoutControl1.PageLayout, pEnvelope, pActiveView);
                        break;
                    case EnumMapSurroundType.ScaleBar:
                        makeScaleBar(pActiveView, axPageLayoutControl1.PageLayout, pEnvelope);
                        break;
                    case EnumMapSurroundType.Legend:
                        MakeLegend(pActiveView, axPageLayoutControl1.PageLayout, pEnvelope);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region 图例封装
        private void MakeLegend(IActiveView pActiveView, IPageLayout pageLayout, IEnvelope pEnvelope)
        {
            UID pID = new UID();
            pID.Value = "esriCarto.Legend";
            IGraphicsContainer pGraphicsContainer = pageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pActiveView.FocusMap) as IMapFrame;
            IMapSurroundFrame pMapSurroundFrame = pMapFrame.CreateSurroundFrame(pID, null); 
            IElement pDeletElement = axPageLayoutControl1.FindElementByName("Legend"); 
            if (pDeletElement != null)
            {
                pGraphicsContainer.DeleteElement(pDeletElement); 
            }
            ISymbolBackground pSymbolBackground = new SymbolBackgroundClass();
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
            pLineSymbol.Color = MapAlgo.ColorRGBT(0, 0, 0);
            pFillSymbol.Color = MapAlgo.ColorRGBT(240, 240, 240);
            pFillSymbol.Outline = pLineSymbol;
            pSymbolBackground.FillSymbol = pFillSymbol;
            pMapSurroundFrame.Background = pSymbolBackground;

            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnvelope as IGeometry;
            IMapSurround pMapSurround = pMapSurroundFrame.MapSurround;
            ILegend pLegend = pMapSurround as ILegend;
            pLegend.ClearItems();
            pLegend.Title = "图例";
            for (int i = 0; i < pActiveView.FocusMap.LayerCount; i++)
            {
                ILegendItem pLegendItem = new HorizontalLegendItemClass();
                pLegendItem.Layer = pActiveView.FocusMap.get_Layer(i);              
                pLegendItem.ShowDescriptions = false;
                pLegendItem.Columns = 1;
                pLegendItem.ShowHeading = true;
                pLegendItem.ShowLabels = true;
                pLegend.AddItem(pLegendItem); 
            }
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        #endregion

        #region 比例尺封装
        private void makeScaleBar(IActiveView pActiveView, IPageLayout pageLayout, IEnvelope pEnvelope)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pStyleGalleryItem == null) return;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            pMapSurroundFrame.MapSurround = (IMapSurround)pStyleGalleryItem.Item;
            IElement pElement = axPageLayoutControl1.FindElementByName("ScaleBar");
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement);  
            }
            IElementProperties pElePro = null;
            pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = (IGeometry)pEnvelope;
            pElePro = pElement as IElementProperties;
            pElePro.Name = "ScaleBar";
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        #endregion

        #region add north arrow
        void addNorthArrow(IPageLayout pPageLayout, IEnvelope pEnv, IActiveView pActiveView)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pStyleGalleryItem == null) return;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            INorthArrow pNorthArrow = new MarkerNorthArrowClass();
            pNorthArrow = pStyleGalleryItem.Item as INorthArrow;
            pNorthArrow.Size = pEnv.Width * 50;
            pMapSurroundFrame.MapSurround = (IMapSurround)pNorthArrow;           
            IElement pElement = axPageLayoutControl1.FindElementByName("NorthArrows"); 
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement); 
            }
            IElementProperties pElePro = null;
            pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = (IGeometry)pEnv;
            pElePro = pElement as IElementProperties;
            pElePro.Name = "NorthArrows";
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        #endregion
        #endregion

        #region layout grid
        /// <summary>
        /// counting grid interv of different scales
        /// </summary>
        /// <param name="x">Max v - Min v</param>
        /// <returns></returns>
        private double scaleSwift(double x)
        {
            return -0.00000007398705 * x + 1.5000517909;
        }

        private void btnGraticuleGrid_Click(object sender, EventArgs e)
        {
            try
            {
                IActiveView pActiveView = axPageLayoutControl1.ActiveView;
                IPageLayout pPageLayout = axPageLayoutControl1.PageLayout;
                DeleteMapGrid(pActiveView, pPageLayout);
                CreateGraticuleMapGrid(pActiveView, pPageLayout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteMapGrid(IActiveView pActiveView, IPageLayout pPageLayout)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer graphicsContainer = pPageLayout as IGraphicsContainer;
            IFrameElement frameElement = graphicsContainer.FindFrame(pMap);
            IMapFrame mapFrame = frameElement as IMapFrame;
            IMapGrids mapGrids = null;
            mapGrids = mapFrame as IMapGrids;
            if (mapGrids.MapGridCount > 0)
            {
                IMapGrid pMapGrid = mapGrids.get_MapGrid(0);
                mapGrids.DeleteMapGrid(pMapGrid);
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        }

        public void CreateGraticuleMapGrid(IActiveView pActiveView, IPageLayout pPageLayout)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraticule pGraticule = new GraticuleClass(); 
            pGraticule.Name = "Map Grid";
            ICartographicLineSymbol pLineSymbol;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 1;
            pLineSymbol.Color = MapAlgo.ColorRGBT(166, 187, 208);
            pGraticule.LineSymbol = pLineSymbol;
            ISimpleMapGridBorder simpleMapGridBorder = new SimpleMapGridBorderClass();
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Color = MapAlgo.ColorRGBT(100, 255, 0);
            simpleLineSymbol.Width = 2;
            simpleMapGridBorder.LineSymbol = simpleLineSymbol as ILineSymbol;
            pGraticule.Border = simpleMapGridBorder as IMapGridBorder;
            pGraticule.SetTickVisibility(true, true, true, true);
            pGraticule.TickLength = 15;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 1;
            pLineSymbol.Color = MapAlgo.ColorRGBT(255, 187, 208);
            pGraticule.TickMarkSymbol = null;
            pGraticule.TickLineSymbol = pLineSymbol;
            pGraticule.SetTickVisibility(true, true, true, true);
            pGraticule.SubTickCount = 5;
            pGraticule.SubTickLength = 10;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 0.1;
            pLineSymbol.Color = MapAlgo.ColorRGBT(166, 187, 208);
            pGraticule.SubTickLineSymbol = pLineSymbol;
            pGraticule.SetSubTickVisibility(true, true, true, true);
            IGridLabel pGridLabel;
            pGridLabel = pGraticule.LabelFormat;
            pGridLabel.LabelOffset = 15;
            stdole.StdFont pFont = new stdole.StdFont();
            pFont.Name = "Arial";
            pFont.Size = 16;
            pGraticule.LabelFormat.Font = pFont as stdole.IFontDisp;
            pGraticule.Visible = true;
            IMeasuredGrid pMeasuredGrid = new MeasuredGridClass();
            IProjectedGrid pProjectedGrid = pMeasuredGrid as IProjectedGrid;
            pProjectedGrid.SpatialReference = pMap.SpatialReference;
            pMeasuredGrid = pGraticule as IMeasuredGrid;
            double MaxX, MaxY, MinX, MinY;
            pProjectedGrid.SpatialReference.GetDomain(out MinX, out MaxX, out MinY, out MaxY);
            pMeasuredGrid.FixedOrigin = true;
            pMeasuredGrid.Units = pMap.MapUnits;
            pMeasuredGrid.XIntervalSize = scaleSwift(MaxX - MinX); 
            pMeasuredGrid.XOrigin = MinX;
            pMeasuredGrid.YIntervalSize = scaleSwift(MaxY - MinY); 
            pMeasuredGrid.YOrigin = MinY;
            IGraphicsContainer pGraphicsContainer = pActiveView as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapGrids pMapGrids = pMapFrame as IMapGrids;
            pMapGrids.AddMapGrid(pGraticule);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        }
        #endregion
    }
}
