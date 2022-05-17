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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lintianwen
{
    public partial class loadingTxtDialogForm : Form
    {
        struct MyPoint
        {
            public string No;
            public double x;
            public double y;
        }
        private AxMapControl pMapControl;
        private ToolStripStatusLabel lblWorking;
        private ToolStripStatusLabel lblMap;
        private ToolStripProgressBar bar;

        public loadingTxtDialogForm(AxMapControl axMapControl, ToolStripStatusLabel labelWorking, ToolStripProgressBar progressBar, ToolStripStatusLabel labelMap)
        {
            InitializeComponent();
            pMapControl = axMapControl;
            lblWorking = labelWorking;
            lblMap = labelMap;
            bar = progressBar;
        }

        #region form events
        //cancel
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //confirm and start runner
        private void button4_Click(object sender, EventArgs e)
        {
            if (ValidateTxtbox())
            {
                //viewing progress
                lblWorking.Visible = true;
                bar.Visible = true;

                //transform to layer and add to mapControl
                bar.Value = 20;
                List<MyPoint> points = GetPoints(textTxtFile.Text);
                if (points != null)
                {
                    bar.Value = 60;
                    ILayer pLayer = CreateSHPWithPoints(points, textShpFile.Text);
                    pMapControl.AddLayer(pLayer);

                    lblMap.Text += pMapControl.Map.Name;
                    bar.Value = 100;
                }
                else
                {
                    lblWorking.Visible = false;
                    bar.Visible = false;
                    MessageBox.Show("未发现点信息，请检查并重选txt文件");
                }
                lblWorking.Visible = false;
                bar.Visible = false;
                this.Close();
            }
            else
            {
                return;
            }
        }

        //open dialog for choosing txt
        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择txt";
            ofd.Filter = "txt文本(*.txt)|*.txt";
            ofd.Multiselect = false;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textTxtFile.Text = ofd.FileName;
            }
        }

        //save for shp
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "shp另存为";
            sfd.Filter = "shp file(*.shp)|*.shp";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textShpFile.Text = sfd.FileName;
            }
        }
        #endregion

        #region runner
        //check for the selection
        private bool ValidateTxtbox()
        {
            if (textTxtFile.Text == "" || !File.Exists(textTxtFile.Text))
            {
                MessageBox.Show("测量数据无效，请重新选择！", "提示", MessageBoxButtons.OK);
                return false;
            }
            if (textShpFile.Text == "" || System.IO.Path.GetExtension(textShpFile.Text).ToLower() != ".shp")
            {
                MessageBox.Show("保存路径无效，请重新选择！", "提示", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        /// <summary>
        /// access info of points with List
        /// </summary>
        /// <param name="fileFullName">file full name</param>
        /// <returns>list of MyPoint</returns>
        private List<MyPoint> GetPoints(string fileFullName)
        {
            try
            {
                //init
                List<MyPoint> points = new List<MyPoint>();
                char[] splitMarker = new char[] {',',' ','\t'};
                System.IO.FileStream fs = new System.IO.FileStream(fileFullName, FileMode.Open);
                StreamReader reader = new StreamReader(fs, Encoding.Default); //lines reader
                string line = reader.ReadLine(); //skip first line
                bar.Value = 25;

                //start
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineElemArr = new string[3];
                    lineElemArr = line.Split(splitMarker);
                    MyPoint point = new MyPoint();
                    point.No = lineElemArr[0].Trim();
                    point.x = Convert.ToDouble(lineElemArr[1]);
                    point.y = Convert.ToDouble(lineElemArr[2]);
                    points.Add(point);
                }
                bar.Value = 55;

                //end
                if (points.Count() == 0)
                    return null;
                reader.Close();
                return points;
            }
            catch (Exception ex)
            {
                MessageBox.Show("载入txt失败" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// create shp file with points in txt
        /// </summary>
        /// <param name="points">the list of MyPoints init from txt</param>
        /// <param name="fileName">save path for shp file</param>
        /// <returns></returns>
        private ILayer CreateSHPWithPoints(List<MyPoint> points, string fileName)
        {
            int index = fileName.LastIndexOf('\\');
            string shapePath = fileName.Substring(0, index); //get shp file path
            string shapeName = fileName.Substring(index + 1); //get shp file name
            bar.Value = 65;

            //create shp layer
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(shapePath, 0);
            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;
            IField pField = new FieldClass();
            IFieldEdit pFieldEdit = (IFieldEdit)pField;
            pFieldEdit.Name_2 = "Shape";
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            IGeometryDef pGeoDef = new GeometryDefClass();
            IGeometryDefEdit pGdefEdit = (IGeometryDefEdit)pGeoDef;
            pGdefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass(); //define spatial reference
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_Beijing1954);
            pGdefEdit.SpatialReference_2 = pSpatialReference;
            pFieldEdit.GeometryDef_2 = pGeoDef;
            pFieldsEdit.AddField(pField);
            IFeatureClass pFeartureClass;
            pFeartureClass = pFeatureWorkspace.CreateFeatureClass(shapeName, pFields, null, null, esriFeatureType.esriFTSimple, "Shape", "");
            IPoint pPoint = new PointClass();
            bar.Value = 80;
            for (int j = 0; j < points.Count; j++)
            {
                pPoint.X = points[j].x;
                pPoint.Y = points[j].y;
                IFeature pFeature = pFeartureClass.CreateFeature();
                pFeature.Shape = pPoint;
                pFeature.Store();
            }
            bar.Value = 95;
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.Name = shapeName;
            pFeatureLayer.FeatureClass = pFeartureClass;
            return pFeatureLayer as ILayer;
        }
        #endregion
    }
}
