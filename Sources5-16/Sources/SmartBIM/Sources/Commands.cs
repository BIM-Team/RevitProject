#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

using Res = Revit.Addin.RevitTooltip.Properties.Resources;
using System.Windows.Forms;
using Revit.Addin.RevitTooltip.UI;
using Revit.Addin.RevitTooltip;
using Revit.Addin.RevitTooltip.Util;

namespace Revit.Addin.RevitTooltip
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public abstract class TooltipCommandBase : IExternalCommand
    {
        #region Class Members
        public Autodesk.Revit.ApplicationServices.Application RevitApp = null;
        public UIDocument RevitUiDoc = null;
        public Document RevitDoc = null;
        #endregion

        #region Class Implementation
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // member initialization
            RevitApp = commandData.Application.Application;
            RevitUiDoc = commandData.Application.ActiveUIDocument;
            RevitDoc = RevitUiDoc.Document;
            //
            // command run
            return RunIt(commandData, ref message, elements);
        }


        #endregion

        /// <summary>
        /// Implements this method to run the external command
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public abstract Result RunIt(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements);

        /// <summary>
        /// Indicates whether administrator is required to run this command.
        /// </summary>
        /// <returns></returns>
        protected virtual bool RequireAdmin()
        {
            return false;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdSettings : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                SettingsForm setForm = new SettingsForm(commandData.Application.ActiveUIDocument.Document);
                if (setForm.ShowDialog() == DialogResult.OK)
                {
                    //释放
                    setForm.Dispose();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdLoadFile : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            LoadFromFile();
            return Result.Succeeded;
        }

        public static string LoadFromFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Res.String_SelectExcelFile;
            ofd.DefaultExt = ".xls";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;
            ofd.Filter = "Excel 97-2003 Workbook(*.xls)|*.xls";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //App.Instance.SurveyInfo = SurveyDataInfo.LoadSurveyFromFile(ofd.FileName);
                //string msg = string.Empty;
                //msg += "MonitorDatetime:" + App.Instance.SurveyInfo.MonitorDatetime + System.Environment.NewLine;
                //msg += "Data count:" + App.Instance.SurveyInfo.Data.Count + System.Environment.NewLine;
                //MessageBox.Show(msg);
                return ofd.FileName;
            }
            return string.Empty;
        }
    }

    #region CommandReloadExcelData
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CommandReloadSQLiteData : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                string path = commandData.Application.ActiveUIDocument.Document.PathName;
                path=Directory.GetParent(path).ToString();
                SQLiteHelper.CreateInstance().UpdateDB(path);
                MessageBox.Show("数据更新成功");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }
    #endregion

    #region CommandAbout
    /// <summary>
    /// This command allows user to show about dialog.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CommandAbout : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            // About form may needs 
            AboutForm aboutFrm = new AboutForm();
            aboutFrm.ShowDialog();
            return Result.Succeeded;
        }
    }
    #endregion

    #region CommandHelp
    /// <summary>
    /// This command allows user to open the help document directly
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CommandHelp : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            // open the help document directly
            try
            {
                string helpDoc = App.GetHelpDoc();
                System.Diagnostics.Process.Start(helpDoc);
            }
            catch (System.Exception)
            {
                MessageBox.Show("打开帮助文件失败！请联系开发技术支持人员！");
            }
            return Result.Succeeded;
        }
    }
    #endregion

    [Transaction(TransactionMode.Manual)]
    public class CmdElementInfo : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            DockablePane panel = commandData.Application.GetDockablePane(new DockablePaneId(ElementInfoPanel.GetInstance().Id));
            panel.Show();
            commandData.Application.Idling += App.Instance.IdlingHandler;
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdSurveyImageInfo : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ImageForm.GetInstance().Show();
            commandData.Application.Idling += App.Instance.IdlingHandler;
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdLoadExcelToDB : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ProcessBarForm processBarForm = new ProcessBarForm();
            if (processBarForm.ShowDialog() == DialogResult.OK) {
            processBarForm.Dispose();
            }    
            return Result.Succeeded;
        }
    }
}
