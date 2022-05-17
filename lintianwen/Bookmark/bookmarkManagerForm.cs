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

namespace lintianwen.Bookmark
{
    public partial class bookmarkManagerForm : Form
    {
        IMap pMap;
        Dictionary<string, ISpatialBookmark> pDictBookmarksName = new Dictionary<string, ISpatialBookmark>();
        IMapBookmarks pBookmarks;

        public bookmarkManagerForm(IMap mainMap)
        {
            InitializeComponent();
            pMap = mainMap;
        }

        private void bookmarkManagerForm_Load(object sender, EventArgs e)
        {
            InitTree();
        }


        //init viewing of tree view
        private void InitTree()
        {
            pBookmarks = pMap as IMapBookmarks;
            IEnumSpatialBookmark pEnum = pBookmarks.Bookmarks;
            pEnum.Reset();
            ISpatialBookmark pBookmark;

            //add bookmarks name to tree view and dict
            while ((pBookmark = pEnum.Next()) != null)
            {
                treeBookmark.Nodes.Add(pBookmark.Name);
                pDictBookmarksName.Add(pBookmark.Name, pBookmark);
            }

            //init btn enable
            if (treeBookmark.Nodes.Count == 0)
            {
                btnLocate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //locate the bookmark
        private void btnLocate_Click(object sender, EventArgs e)
        {
            if (treeBookmark.SelectedNode == null)
            {
                MessageBox.Show("未明确定位对象");
                return;
            }
            TreeNode pSelectedNode = treeBookmark.SelectedNode;
            ISpatialBookmark pBookmark = pDictBookmarksName[pSelectedNode.Text];
            pBookmark.ZoomTo(pMap);
            IActiveView pActivateView = pMap as IActiveView;
            pActivateView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            this.Close();
        }

        //delete a bookmark, its tree node and its dict elem
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (treeBookmark.Nodes.Count == 0)
            {
                MessageBox.Show("未明确删除对象");
                return;
            }
            TreeNode pSelectedNode = treeBookmark.SelectedNode;
            pBookmarks.RemoveBookmark(pDictBookmarksName[pSelectedNode.Text]);
            pDictBookmarksName.Remove(pSelectedNode.Text);
            treeBookmark.Nodes.Remove(pSelectedNode);
            treeBookmark.Refresh();
        }
    }
}
