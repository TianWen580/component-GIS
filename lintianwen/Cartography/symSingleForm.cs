using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lintianwen.Cartography
{
    public partial class symSingleForm : Form
    {
        int r, g, b;
        List<IFeatureClass> _lstFeatCls = null;
        public delegate void SimpleRenderEventHandler(string sFeatClsName, IRgbColor pRgbColr);
        public event SimpleRenderEventHandler SimpleRender = null;

        public symSingleForm()
        {
            InitializeComponent();
        }
        private IMap _pMap;
        public IMap PMap
        {
            get { return _pMap; }
            set { _pMap = value; }
        }

        public void InitUI()
        {
            string sClsName = string.Empty;
            IFeatureClass pFeatCls = null;
            cmbSelLyr.Items.Clear();
            _lstFeatCls = MapAlgo.GetFeatureClass(_pMap);
            for (int i = 0; i < _lstFeatCls.Count; i++)
            {
                pFeatCls = _lstFeatCls[i];
                sClsName = pFeatCls.AliasName;
                if (!cmbSelLyr.Items.Contains(sClsName))
                {
                    cmbSelLyr.Items.Add(sClsName);
                }
            }
        }

        private bool check()
        {
            if (cmbSelLyr.SelectedIndex == -1)
            {
                MessageBox.Show("请选择符号化图层！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private void symSingleForm_Load(object sender, EventArgs e)
        {
            cmbSelColor.Items.Clear();
            cmbSelColor.Items.Add("Black");
            cmbSelColor.Items.Add("Red");
            cmbSelColor.Items.Add("Green");
            cmbSelColor.Items.Add("IndianRed");
            cmbSelColor.Items.Add("LightBlue");
        }

        private void cmbSelColor_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                SolidBrush brush = new SolidBrush(Color.FromName(cmbSelColor.Items[e.Index].ToString()));
                Rectangle rect = e.Bounds;
                rect.Inflate(-2, -2);
                Rectangle rectColor = new Rectangle(rect.Location, new Size(10, rect.Height));
                Rectangle rectColor1 = new Rectangle(rect.Location.X + 30, rect.Location.Y, 18, rect.Height);
                e.Graphics.FillRectangle(brush, rectColor);
                System.Drawing.Font font = new Font("宋体", 10, FontStyle.Bold);
                e.Graphics.DrawString(cmbSelColor.Items[e.Index].ToString(), font, Brushes.Blue, (rect.X + 30), rect.Y);
            }
        }

        private void cmbSelColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            r = Color.FromName(cmbSelColor.SelectedItem.ToString()).R;
            g = Color.FromName(cmbSelColor.SelectedItem.ToString()).G;
            b = Color.FromName(cmbSelColor.SelectedItem.ToString()).B;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!check()) return;
            //System.Drawing.Color m_Color;           
            int m_Red = r;
            int m_Green = g;
            int m_Blue = b;
            IRgbColor pRgbColor = MapAlgo.ColorRGBT(m_Red, m_Green, m_Blue);
            SimpleRender(cmbSelLyr.SelectedItem.ToString(), pRgbColor);
            cmbSelLyr.Items.Clear();
            cmbSelLyr.Text = "";
            Close();                     
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
