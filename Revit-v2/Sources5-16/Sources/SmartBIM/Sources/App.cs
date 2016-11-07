#region Namespaces
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System.IO;
#endregion // Namespaces

using Res = Revit.Addin.RevitTooltip.Properties.Resources;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Revit.Addin.RevitTooltip.Util;
using Revit.Addin.RevitTooltip.UI;
using System.Threading;
using System;

namespace Revit.Addin.RevitTooltip
{
    /// <summary>
    /// External application handling a modeless 
    /// form and the Idling event, based on the 
    /// ModelessForm_IdlingEvent SDK sample.
    /// </summary>
    public class App : IExternalApplication
    {
        /// <summary>
        /// Singleton external application class instance.
        /// </summary>
        internal static App _app = null;
        private string m_previousDocPathName = null;
        // internal Document currentDoc=null;
        internal static RevitTooltip settings = null;
        ElementInfoPanel m_elementInfoPanel = null;
        private int m_selectedElementId = -1;
        UIControlledApplication m_uiApp = null;
        

        /// <summary>
        /// Provide access to singleton class instance.
        /// </summary>
        public static App Instance
        {
            get { return _app; }
        }
        /// <summary>
        /// 点击显示属性面板
        /// </summary>
        internal PushButton ElementInfoButton { get; set; }
        /// <summary>
        /// 点击弹出折线图面板
        /// </summary>
        internal PushButton SurveyImageInfoButton { get; set; }
        /// <summary>
        /// 点击加载excel
        /// </summary>
        internal PushButton LoadExcelButton { get; set; }
        internal PushButton LoadExcelToSQLiteButton { get; set; }

        /// <summary>
        /// 点击重新加载::加载到SQLite
        /// </summary>
        internal PushButton ReloadDataButton { get; set; }
        //internal PushButton TooltipOnButton { get; set; }
        //internal PushButton TooltipOffButton { get; set; }

        public Result OnStartup(UIControlledApplication app)
        {

            m_uiApp = app;
            _app = this;
            app.ViewActivated += OnViewActivated;
            app.Idling += IdlingHandler;
            string file = Path.Combine(Path.GetDirectoryName(typeof(App).Assembly.Location), "MahApps.Metro.dll");
            if (File.Exists(file))
                System.Reflection.Assembly.LoadFrom(file);

            m_elementInfoPanel = ElementInfoPanel.GetInstance();
            app.RegisterDockablePane(new DockablePaneId(m_elementInfoPanel.Id), "构件信息", m_elementInfoPanel);
            //
            // Assembly members initialization 
            string tabName = Res.String_AppTabName;
            string addinAssembly = this.GetType().Assembly.Location;
            string addinDir = Path.GetDirectoryName(addinAssembly);
            string userManual = Path.Combine(addinDir, "help.pdf");
            //
            // Ribbon suites firstly
            ContextualHelp cHelp = new ContextualHelp(ContextualHelpType.Url, userManual);
            app.CreateRibbonTab(tabName);
            RibbonPanel ribbonPanel = app.CreateRibbonPanel(tabName, Res.String_AppPanelName);

            // settings
            PushButton cmdButton = (PushButton)ribbonPanel.AddItem(
                new PushButtonData("Tooltip_Settings", Res.CommandName_Settings,
                    addinAssembly, "Revit.Addin.RevitTooltip.CmdSettings"));
            BitmapSource image = Utils.ConvertFromBitmap(Res.settings.ToBitmap());
            cmdButton.Image = cmdButton.LargeImage = image;
            cmdButton.ToolTip = Res.CommandDescription_Settings;
            cmdButton.SetContextualHelp(cHelp);
            ribbonPanel.AddSeparator();
            //load excel file to DB
            LoadExcelButton = (PushButton)ribbonPanel.AddItem(
                    new PushButtonData("LoadExcelToDB", Res.CommandName_Import,
                        addinAssembly, "Revit.Addin.RevitTooltip.CmdLoadExcelToDB"));
            image = Utils.ConvertFromBitmap(Res.tooltip_on.ToBitmap());
            LoadExcelButton.Image = LoadExcelButton.LargeImage = image;
            LoadExcelButton.ToolTip = Res.CommandDescription_Import;
            LoadExcelButton.SetContextualHelp(cHelp);
            ribbonPanel.AddSeparator();

            //load excel file to SQLite
            LoadExcelToSQLiteButton = (PushButton)ribbonPanel.AddItem(
                    new PushButtonData("LoadExcelToSQLite", Res.CommandName_Import_SQLite,
                        addinAssembly, "Revit.Addin.RevitTooltip.CmdLoadExcelToSQLite"));
            image = Utils.ConvertFromBitmap(Res.tooltip_on.ToBitmap());
            LoadExcelToSQLiteButton.Image = LoadExcelToSQLiteButton.LargeImage = image;
            LoadExcelToSQLiteButton.ToolTip = Res.CommandDescription_Import_SQLite;
            LoadExcelToSQLiteButton.SetContextualHelp(cHelp);
            ribbonPanel.AddSeparator();

            //////////////////////////////////////////////////////////////////////////
            // dock panel
            ElementInfoButton = (PushButton)ribbonPanel.AddItem(
                    new PushButtonData("ElementInfo", Res.Command_ElementInfo,
                        addinAssembly, "Revit.Addin.RevitTooltip.CmdElementInfo"));
            image = Utils.ConvertFromBitmap(Res.tooltip_on.ToBitmap());
            ElementInfoButton.Image = ElementInfoButton.LargeImage = image;
            ElementInfoButton.ToolTip = Res.CommandDescription_TooltipOn;
            ElementInfoButton.SetContextualHelp(cHelp);
            // Tooltip off 
            ribbonPanel.AddSeparator();
            SurveyImageInfoButton = (PushButton)ribbonPanel.AddItem(
                new PushButtonData("SurveyImageInfo", Res.Command_SurveyImageInfo,
                    addinAssembly, "Revit.Addin.RevitTooltip.CmdSurveyImageInfo"));
            image = Utils.ConvertFromBitmap(Res.tooltip_on.ToBitmap());
            SurveyImageInfoButton.Image = SurveyImageInfoButton.LargeImage = image;
            SurveyImageInfoButton.ToolTip = Res.CommandDescription_SurveyImage;
            SurveyImageInfoButton.SetContextualHelp(cHelp);
            //
            ribbonPanel.AddSeparator();
            //reload
            ReloadDataButton = (PushButton)ribbonPanel.AddItem(
                new PushButtonData("CommandReloadSQLiteData", Res.Command_ReloadExcelData,
                    addinAssembly, "Revit.Addin.RevitTooltip.CommandReloadSQLiteData"));
            image = Utils.ConvertFromBitmap(Res.refresh);
            ReloadDataButton.Image = ReloadDataButton.LargeImage = image;
            ReloadDataButton.ToolTip = Res.CommandDescription_ReloadExcelData;
            ReloadDataButton.SetContextualHelp(cHelp);
            // Tooltip off 
            ribbonPanel.AddSeparator();

            // About\Help
            PushButtonData aboutButtonData = new PushButtonData("AboutButton",
                Res.CommandName_About, addinAssembly, "Revit.Addin.RevitTooltip.CommandAbout");
            aboutButtonData.Image = Utils.ConvertFromBitmap(Properties.Resources.about_16.ToBitmap());
            aboutButtonData.ToolTip = Properties.Resources.CommandDescription_About;
            aboutButtonData.SetContextualHelp(cHelp);
            PushButtonData helpButtonData = new PushButtonData("HelpButton",
                Res.CommandName_Help, addinAssembly, "Revit.Addin.RevitTooltip.CommandHelp");
            helpButtonData.Image = Utils.ConvertFromBitmap(Properties.Resources.help_16.ToBitmap());
            helpButtonData.ToolTip = Properties.Resources.CommandDescription_Help;
            helpButtonData.SetContextualHelp(cHelp);
            ribbonPanel.AddStackedItems(aboutButtonData, helpButtonData);
            return Result.Succeeded;


        }

        public void SetPanelEnabled(bool enabled)
        {
            string tabName = Res.String_AppTabName;

            List<RibbonPanel> panels = m_uiApp.GetRibbonPanels(tabName);
            foreach (var panel in panels)
            {
                foreach (RibbonItem item in panel.GetItems())
                {
                    if (item.ItemText == Res.CommandName_About ||
                        item.ItemText == Res.CommandName_Help ||
                        item.ItemText == Res.CommandName_LicenseInfo)
                    {
                        //about ribbon always enabled
                        item.Enabled = true;
                        continue;
                    }
                    item.Enabled = enabled;
                }
            }
        }

        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            settings = ExtensibleStorage.GetTooltipInfo(e.Document.ProjectInformation);
            //load project settings when document changed
            try
            {
                if (string.IsNullOrEmpty(m_previousDocPathName) || m_previousDocPathName != e.Document.PathName)
                {
                   
                    m_previousDocPathName = e.Document.PathName;
                }
                //hide the info panel if not registered
                DockablePane panel = m_uiApp.GetDockablePane(new DockablePaneId(ElementInfoPanel.GetInstance().Id));
                if (panel != null)
                {
                    panel.Hide();
                }
                
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public Result OnShutdown(UIControlledApplication a)
        {

            return Result.Succeeded;
        }
        /// <summary>
        /// Idling event handler.
        /// </summary>
        /// <remarks>
        /// We keep the handler very simple. First check
        /// if we still have the form. If not, unsubscribe 
        /// from Idling, for we no longer need it and it 
        /// makes Revit speedier. If the form is around, 
        /// check if it has a request ready and process 
        /// it accordingly.
        /// </remarks>
        public void IdlingHandler(
          object sender,
          IdlingEventArgs args)
        {
            UIApplication uiapp = sender as UIApplication;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            // UI document is null if the project is closed.

            if (null != uidoc)
            {
                //存放实体
                string entity = string.Empty;
                bool isSurvey = false;
                MysqlUtil mysql = null;

                // ElementInfoPanel
#if(Since2016)
                Element selectElement = uidoc.Document.GetElement(uidoc.Selection.GetElementIds().FirstOrDefault());
#else
                Element selectElement = uidoc.Selection.Elements.Cast<Element>().FirstOrDefault<Element>();
#endif
                if (selectElement != null)
                {
                Category category = selectElement.Category;
                    entity = Utils.GetParameterValueAsString(selectElement, Res.String_ParameterName);
                    if (!string.IsNullOrEmpty(entity))
                    {
                        isSurvey = Res.String_ParameterSurveyType.Equals(selectElement.Name);
                        mysql = MysqlUtil.CreateInstance();
                        if (m_selectedElementId != selectElement.Id.IntegerValue)
                        {
                            m_selectedElementId = selectElement.Id.IntegerValue;
                            // isSurvey = true;
                            if (!isSurvey)
                            {//不是测量数据
                             //mysql 
                             // List<ParameterData> parameterDataList = mysql.SelectEntityData(entity);
                             //sqlite
                                List<ParameterData> parameterDataList = SQLiteHelper.CreateInstance().SelectEntityData(entity);
                                ElementInfoPanel.GetInstance().Update(parameterDataList);
                            }
                            else
                            {//测量数据绘制折线图
                                ImageForm.GetInstance().EntityName = entity;
                                // ImageForm.GetInstance().EntityName = "CX1";
                            }
                        }
                    }
                }
                else
                {
                    if (m_selectedElementId != -1)
                    {//清空数据
                        m_selectedElementId = -1;
                        ElementInfoPanel.GetInstance().Update(new List<ParameterData>());
                    }
                }
            }
        }

        public class ParameterData : IComparable<ParameterData>
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public ParameterData(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public override bool Equals(object obj)
            {
                ParameterData o = (ParameterData)obj;
                return this.Value.Equals(o.Value)&&this.Name.Equals(o.Name);
            }
            public override int GetHashCode()
            {
                return (this.Name+this.Value).GetHashCode();
            }

            public int CompareTo(ParameterData other)
            {
                return Convert.ToInt32(this.Value.Substring(Res.String_Reg.Length))- Convert.ToInt32(other.Value.Substring(Res.String_Reg.Length));
            }
        }

        /// <summary>
        /// Gets help document
        /// </summary>
        /// <returns></returns>
        public static string GetHelpDoc()
        {
            string addinAssembly = typeof(Revit.Addin.RevitTooltip.App).Assembly.Location;
            string userManual = Path.Combine(Path.GetDirectoryName(addinAssembly), "help.pdf");
            return userManual;
        }

        public string GetProductName()
        {
            return "BIMRevit2014-2016";
        }
       
        
    }
}
