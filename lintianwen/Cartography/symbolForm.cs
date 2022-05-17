using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using lintianwen.Cartography;
using ESRI.ArcGIS.Carto;

namespace lintianwen.Cartography
{
    public partial class symbolForm : Form
    {
        public ISymbol pSelSymbol;
        private IStyleGalleryItem pStyleGalleryItem;
        private ISymbologyStyleClass pSymStyleClass;

        public delegate void GetSelSymbolItemEventHandler(ref IStyleGalleryItem pStyleItem);
        public event GetSelSymbolItemEventHandler GetSelSymbolItem = null;

        string filepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;


        private EnumMapSurroundType _enumMapSurType = EnumMapSurroundType.None;
        public EnumMapSurroundType EnumMapSurType
        {
            get { return _enumMapSurType; }
            set { _enumMapSurType = value; }
        }

        public symbolForm()
        {
            InitializeComponent();
        }

        public void InitUI()
        {
            SymbologyCtrl.Clear();
            string StyleFilePath = "C:\\Symbol\\ESRI.ServerStyle"; 
            SymbologyCtrl.LoadStyleFile(StyleFilePath);
            switch (_enumMapSurType)
            {
                case EnumMapSurroundType.NorthArrow: 
                    SymbologyCtrl.StyleClass = esriSymbologyStyleClass.esriStyleClassNorthArrows;
                    pSymStyleClass = SymbologyCtrl.GetStyleClass(esriSymbologyStyleClass.esriStyleClassNorthArrows);
                    break;
                case EnumMapSurroundType.ScaleBar: 
                    SymbologyCtrl.StyleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
                    pSymStyleClass = SymbologyCtrl.GetStyleClass(esriSymbologyStyleClass.esriStyleClassScaleBars);
                    break;
            }
            pSymStyleClass.UnselectItem();
        }

        private void frmSymbol_Load(object sender, EventArgs e)
        {

        }

        private void SymbologyCtrl_OnMouseDown(object sender, ISymbologyControlEvents_OnMouseDownEvent e)
        {
            try
            {
                pStyleGalleryItem = SymbologyCtrl.HitTest(e.x, e.y);            
            }
            catch (Exception ex)
            {

            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GetSelSymbolItem(ref pStyleGalleryItem); 
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GetSelSymbolItem(ref pStyleGalleryItem); 
            this.Close();
        }


    }
}
