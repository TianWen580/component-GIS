﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;

namespace lintianwen.CommonToolsAndCommands
{
    public class UndoCommandClass : ICommand
    {
        private IMap m_Map = null;
        private bool bEnable = true;
        private IActiveView m_activeView = null;
        private IHookHelper m_hookHelper = null;
        private IEngineEditor m_EngineEditor = null;

        #region ICommand members

        public int Bitmap
        {
            get { return -1; }
        }

        public string Caption
        {
            get { return "撤销操作"; }
        }

        public string Category
        {
            get { return "编辑按钮"; }
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
            get { return "撤销操作"; }
        }

        public string Name
        {
            get { return "UndoCommand"; }
        }

        public void OnClick()
        {
            try
            {
                m_Map = m_hookHelper.FocusMap;
                m_activeView = m_Map as IActiveView;
                m_EngineEditor = MapAlgo.EngineEditor;
                EditVertexClass.ClearResource();
                if (m_EngineEditor == null) return;
                if (m_EngineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
                IWorkspaceEdit2 pWSEdit = m_EngineEditor.EditWorkspace as IWorkspaceEdit2;
                if (pWSEdit == null) return;
                Boolean bHasUndo = false;
                pWSEdit.HasUndos(ref bHasUndo);
                if (bHasUndo) pWSEdit.UndoEditOperation();
                m_activeView.Refresh();

            }
            catch (Exception ex)
            {
            }

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
            get { return "撤销操作"; }
        }

        #endregion


    }
}
