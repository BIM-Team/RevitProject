#region Namespaces
using System;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

using System.Windows.Forms;
using Revit.Addin.RevitTooltip.UI;

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
                NewSettings settingForm = new NewSettings(App.Instance.settings);
                settingForm.Show();
                commandData.Application.Idling += App.Instance.SettingIdlingHandler;
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }

    

    #region CommandReloadExcelData
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CmdLoadSQLiteData : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (MessageBox.Show("确定同步本地文件与Mysql一致?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    if (App.Instance.Sqlite.LoadDataToSqlite())
                    {
                        App.Instance.ThresholdChanged = true;
                        MessageBox.Show("数据更新成功");
                    }
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return Result.Failed;
                }
            }
            return Result.Cancelled;
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
            if (!File.Exists(Path.Combine(App.Instance.settings.SqliteFilePath, App.Instance.settings.SqliteFileName)))
            {
                MessageBox.Show("本地数据文件不存在，请先更新");
                return Result.Succeeded;
            }
            DockablePane panel = commandData.Application.GetDockablePane(new DockablePaneId(ElementInfoPanel.GetInstance().Id));
            panel.Show();
            commandData.Application.Idling += App.Instance.IdlingHandler;
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdImageControl : TooltipCommandBase
    {
        public override Result RunIt(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!File.Exists(Path.Combine(App.Instance.settings.SqliteFilePath, App.Instance.settings.SqliteFileName))) {
                MessageBox.Show("本地数据文件不存在，请先更新");
                return Result.Succeeded;
            }
            ImageControl.Instance().setExcelType(App.Instance.Sqlite.SelectDrawTypes());
            DockablePane imagePanel = commandData.Application.GetDockablePane(new DockablePaneId(ImageControl.Instance().Id));
            imagePanel.Show();
            commandData.Application.Idling += App.Instance.IdlingHandler;
            commandData.Application.Idling += App.Instance.ImageControlIdlingHandler;

            


            return Result.Succeeded;
        }
    }



}
