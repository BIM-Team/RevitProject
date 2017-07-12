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
using Revit.Addin.RevitTooltip.UI;
using System.Threading;
using System;
using Revit.Addin.RevitTooltip.Intface;
using Revit.Addin.RevitTooltip.Impl;
using Revit.Addin.RevitTooltip.Dto;
using Autodesk.Revit.DB.Events;

namespace Revit.Addin.RevitTooltip
{
    

    /// <summary>
    /// ������������
    /// </summary>
    public class App : IExternalApplication
    {
        private bool ColorMaterialIsReady = false;
        private bool isThresholdChanged = true;
        /// <summary>
        /// ��¼��ֵ�Ƿ�ı�
        /// </summary>
        public bool ThresholdChanged
        {
            set { this.isThresholdChanged = value; }
        }
        //���ڼ�¼��ɫ�Ƿ��иı�

        /// <summary>
        /// ��ɫ��ۣ��ۼ��쳣
        /// </summary>
        private Material color_red = null;
        /// <summary>
        /// ��ɫ��ۣ�����
        /// </summary>
        private Material color_gray = null;
        /// <summary>
        /// ��ɫ��ۣ��쳣
        /// </summary>
        private Material color_blue = null;
        /// <summary>
        /// ����ģʽ
        /// </summary>
        private static App _app = null;

        private string m_previousDocPathName = null;
        //ģ������ʱʹ��
        private string _previousselectedNoInfoEntity = null;
        private string selectedNoInfoEntity = null;
        /// <summary>
        /// ģ������
        /// </summary>
        public string SelectedNoInfoEntity
        {
            set
            {
                this.selectedNoInfoEntity = value;
            }
        }
        //���ĵ��͹���һ��
        private Dictionary<string, Element> keyNameToElementMap = null;

        private bool isSettingChange = false;
        /// <summary>
        /// ģ����ص�����
        /// ԭ�������ģ���м�
        /// </summary>
        internal RevitTooltip settings = null;
        internal RevitTooltip Settings
        {
            set
            {
                if (null == this.settings || !this.settings.Equals(value))
                {
                    this.settings = value;
                    this.mysql = new MysqlUtil(value);
                    this.sqlite = new SQLiteHelper(value);
                    this.isSettingChange = true;

                }
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        ElementInfoPanel m_elementInfoPanel = null;
        /// <summary>
        /// ��ͼ�Ŀ������
        /// </summary>
        ImageControl m_ImageControl = null;
        /// <summary>
        /// ���ڼ�¼��ǰѡ������ݣ������ж��Ƿ��ǰһ��ѡ���������ͬ
        /// </summary>
        private int m_selectedElementId = -1;
        /// <summary>
        /// UI_APP
        /// </summary>
        UIControlledApplication m_uiApp = null;
        /// <summary>
        /// ��ȡ����
        /// </summary>
        public static App Instance
        {
            get { return _app; }
        }
        /// <summary>
        /// �����ʾ�������
        /// </summary>
        internal PushButton ElementInfoButton { get; set; }
        /// <summary>
        /// �����������ͼ���
        /// </summary>
        internal PushButton SurveyImageInfoButton { get; set; }


        /// <summary>
        /// ������¼���:��Mysql���ص�SQLite
        /// </summary>
        internal PushButton LoadDataToSqliteButton { get; set; }
        /// <summary>
        /// MySQL��ʵ������
        /// </summary>
        private IMysqlUtil mysql = null;
        /// <summary>
        /// MySQL��ʵ������
        /// </summary>
        public IMysqlUtil MySql
        {
            get { return this.mysql; }
        }
        private ISQLiteHelper sqlite = null;
        /// <summary>
        /// ����SQLiteʵ��
        /// </summary>
        public ISQLiteHelper Sqlite
        {
            get { return this.sqlite; }
        }
        //��¼��ǰ�򿪵��ĵ�
        private Document current_doc;
        /// <summary>
        /// ��ȡ��ǰ�򿪵��ĵ�
        /// </summary>
        public Document CurrentDoc
        {
            get { return this.current_doc; }
        }


        public Result OnStartup(UIControlledApplication app)
        {
            //UI����
            m_uiApp = app;
            //APP������
            _app = this;
            //�¼��󶨣�����һ���ĵ�����ͼ
            app.ViewActivated += OnViewActivated;
            //�¼��󶨣������¼�
            //app.Idling += IdlingHandler;
            app.ControlledApplication.DocumentClosing += DocumentClosingAction;
            app.ControlledApplication.DocumentOpened += DocumentOpenedAction;


            //���ظ�ʽ�ļ�
            string file = Path.Combine(Path.GetDirectoryName(typeof(App).Assembly.Location), "MahApps.Metro.dll");
            if (File.Exists(file))
                System.Reflection.Assembly.LoadFrom(file);
            //�������
            m_elementInfoPanel = ElementInfoPanel.GetInstance();
            m_ImageControl = ImageControl.Instance();
            //ע��Dockable���
            app.RegisterDockablePane(new DockablePaneId(m_elementInfoPanel.Id), "������Ϣ", m_elementInfoPanel);
            app.RegisterDockablePane(new DockablePaneId(m_ImageControl.Id), "�����Ϣ", m_ImageControl);
            //
            //
            string tabName = Res.String_AppTabName;
            string addinAssembly = this.GetType().Assembly.Location;
            string addinDir = Path.GetDirectoryName(addinAssembly);
            string userManual = Path.Combine(addinDir, "help.pdf");
            //
            // ��������ѡ��
            ContextualHelp cHelp = new ContextualHelp(ContextualHelpType.Url, userManual);
            app.CreateRibbonTab(tabName);
            RibbonPanel ribbonPanel = app.CreateRibbonPanel(tabName, Res.String_AppPanelName);

            // ���ð�ť
            PushButton cmdButton = (PushButton)ribbonPanel.AddItem(
                new PushButtonData("Tooltip_Settings", Res.CommandName_Settings,
                    addinAssembly, "Revit.Addin.RevitTooltip.CmdSettings"));
            BitmapSource image = Utils.ConvertFromBitmap(Res.settings.ToBitmap());
            cmdButton.Image = cmdButton.LargeImage = image;
            cmdButton.ToolTip = Res.CommandDescription_Settings;
            cmdButton.SetContextualHelp(cHelp);
            //��ӷָ�
            ribbonPanel.AddSeparator();
            //�����ʾ�������
            ElementInfoButton = (PushButton)ribbonPanel.AddItem(
                    new PushButtonData("ElementInfo", Res.Command_ElementInfo,
                        addinAssembly, "Revit.Addin.RevitTooltip.CmdElementInfo"));
            image = Utils.ConvertFromBitmap(Res.tooltip_on.ToBitmap());
            ElementInfoButton.Image = ElementInfoButton.LargeImage = image;
            ElementInfoButton.ToolTip = Res.CommandDescription_TooltipOn;
            ElementInfoButton.SetContextualHelp(cHelp);
            //��ӷָ�
            ribbonPanel.AddSeparator();
            //�򿪻�ͼ���
            SurveyImageInfoButton = (PushButton)ribbonPanel.AddItem(
                new PushButtonData("SurveyImageInfo", Res.Command_SurveyImageInfo,
                    addinAssembly, "Revit.Addin.RevitTooltip.CmdImageControl"));
            image = Utils.ConvertFromBitmap(Res.tooltip_on.ToBitmap());
            SurveyImageInfoButton.Image = SurveyImageInfoButton.LargeImage = image;
            SurveyImageInfoButton.ToolTip = Res.CommandDescription_SurveyImage;
            SurveyImageInfoButton.SetContextualHelp(cHelp);
            //��ӷָ�
            ribbonPanel.AddSeparator();
            //��Mysql�������ݵ�Sqlite
            LoadDataToSqliteButton = (PushButton)ribbonPanel.AddItem(
                new PushButtonData("CommandReloadSQLiteData", Res.Command_ReloadExcelData,
                    addinAssembly, "Revit.Addin.RevitTooltip.CmdLoadSQLiteData"));
            image = Utils.ConvertFromBitmap(Res.refresh);
            LoadDataToSqliteButton.Image = LoadDataToSqliteButton.LargeImage = image;
            LoadDataToSqliteButton.ToolTip = Res.CommandDescription_ReloadExcelData;
            LoadDataToSqliteButton.SetContextualHelp(cHelp);
            // �ָ�
            ribbonPanel.AddSeparator();

            // About\Help
            PushButtonData aboutButtonData = new PushButtonData("AboutButton",
                Res.CommandName_About, addinAssembly, "Revit.Addin.RevitTooltip.CommandAbout");
            aboutButtonData.Image = Utils.ConvertFromBitmap(Res.about_16.ToBitmap());
            aboutButtonData.ToolTip = Properties.Resources.CommandDescription_About;
            aboutButtonData.SetContextualHelp(cHelp);
            PushButtonData helpButtonData = new PushButtonData("HelpButton",
                Res.CommandName_Help, addinAssembly, "Revit.Addin.RevitTooltip.CommandHelp");
            helpButtonData.Image = Utils.ConvertFromBitmap(Res.help_16.ToBitmap());
            helpButtonData.ToolTip = Properties.Resources.CommandDescription_Help;
            helpButtonData.SetContextualHelp(cHelp);
            ribbonPanel.AddStackedItems(aboutButtonData, helpButtonData);
            //���ó�����
            // this.SetPanelEnabled(false);

            return Result.Succeeded;


        }


        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {

            try
            {
                //���´���ͼ������Panel
                DockablePane panel = m_uiApp.GetDockablePane(new DockablePaneId(ElementInfoPanel.GetInstance().Id));
                if (panel != null)
                {
                    ElementInfoPanel.GetInstance().Update(new InfoEntityData());
                    panel.Hide();
                }
                DockablePane imageControl = m_uiApp.GetDockablePane(new DockablePaneId(ImageControl.Instance().Id));
                if (imageControl != null)
                {
                    imageControl.Hide();
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
                //���ʵ��
                string entity = string.Empty;
                bool isSurvey = false;
                // ElementInfoPanel
#if (Since2016)
                Element selectElement = uidoc.Document.GetElement(uidoc.Selection.GetElementIds().FirstOrDefault());
#else
                Element selectElement = uidoc.Selection.Elements.Cast<Element>().FirstOrDefault<Element>();
#endif
                if (selectElement != null)
                {
                    entity = Utils.GetParameterValueAsString(selectElement, Res.String_ParameterName);
                    if (!string.IsNullOrEmpty(entity))
                    {
                        isSurvey = Res.String_ParameterSurveyType.Equals(selectElement.Name);

                        if (m_selectedElementId != selectElement.Id.IntegerValue)
                        {
                            m_selectedElementId = selectElement.Id.IntegerValue;
                            // isSurvey = true;
                            if (!isSurvey)
                            {//���ǲ�������
                             //sqlite
                                InfoEntityData infoEntityData = Sqlite.SelectInfoData(entity);
                                ElementInfoPanel.GetInstance().Update(infoEntityData);
                            }
                            else
                            {//�������ݻ�������ͼ
                                DrawEntityData drawEntityData = App.Instance.Sqlite.SelectDrawEntityData(entity, null, null);
                                NewImageForm.Instance().EntityData = drawEntityData;
                                ExcelTable excel = App.Instance.Sqlite.SelectADrawType(entity);
                                NewImageForm.Instance().Text= excel == null ? "���" + entity + "�Ĳ�������" : excel.CurrentFile + ": ���" + entity+ "�Ĳ�������";
                                NewImageForm.Instance().Show();
                            }
                        }
                    }
                }
                else
                {
                    if (m_selectedElementId != -1)
                    {//�������
                        m_selectedElementId = -1;
                        ElementInfoPanel.GetInstance().Update(null);
                        if (NewImageForm.Instance().Visible)
                        {
                            NewImageForm.Instance().EntityData = new DrawEntityData();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SettingForm�������¼�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SettingIdlingHandler(
          object sender,
          IdlingEventArgs args)
        {
            UIApplication uiapp = sender as UIApplication;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (null != uidoc)
            {
                //��settings������ģ��
                if (isSettingChange)
                {
                    ExtensibleStorage.StoreTooltipInfo(CurrentDoc.ProjectInformation, settings);
                    App.Instance.CurrentDoc.Save();
                    isThresholdChanged = false;
                    isSettingChange = false;
                }
            }
        }
        /// <summary>
        /// imageControl�������¼�������
        /// ģ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ImageControlIdlingHandler(
                  object sender,
                  IdlingEventArgs args)
        {
            UIApplication uiapp = sender as UIApplication;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (null != uidoc)
            {
                //ģ������
                if (!string.IsNullOrWhiteSpace(selectedNoInfoEntity) && !selectedNoInfoEntity.Equals(_previousselectedNoInfoEntity))
                {
                    IList<UIView> uiViews = uidoc.GetOpenUIViews();
                    if (uiViews != null && uiViews.Count != 0)
                    {
                        _previousselectedNoInfoEntity = selectedNoInfoEntity;
                        UIView currentUIView = uiViews[uiViews.Count - 1];
                        try
                        {
                            uidoc.ShowElements(keyNameToElementMap[selectedNoInfoEntity]);
                            //currentUIView.ZoomSheetSize();
                   
                            currentUIView.Zoom(0.1d);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                //�����쳣�����óɲ�ͬ����ɫ
                if (isThresholdChanged && ColorMaterialIsReady)
                {
                    
                    List<CEntityName> all_entity = App.Instance.Sqlite.SelectAllEntitiesAndErrIgnoreSignal();
                    using (Transaction tran = new Transaction(uidoc.Document))
                    {
                        if (tran.Start("ChangeColor") == TransactionStatus.Started)
                        {

                            foreach (CEntityName one in all_entity)
                            {
                                try
                                {
                                    if (!keyNameToElementMap.ContainsKey(one.EntityName))
                                    {
                                        continue;
                                    }
                                    Parameter param_ma = keyNameToElementMap[one.EntityName].get_Parameter(Res.String_Color);
                                    if (null != param_ma)
                                    {
                                        string err = one.ErrMsg;
                                        if (err.IndexOf("Total") != -1)
                                        {
                                            param_ma.Set(color_red.Id);
                                        }
                                        else if (err.IndexOf("Diff") != -1)
                                        {
                                            param_ma.Set(color_blue.Id);
                                        }
                                        else
                                        {
                                            param_ma.Set(color_gray.Id);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {

                                    throw e;
                                }
                            }
                        }
                        if (tran.Commit() == TransactionStatus.Committed)
                        {
                            isThresholdChanged = false;
                        }
                        else
                        {
                            tran.RollBack();
                        }
                        if (uidoc.Document.IsModified) {
                            uidoc.Document.Save();
                        }

                    }
                }
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
        private void DocumentClosingAction(object sender, DocumentClosingEventArgs even)
        {
            //isColorChanged = false;
            isThresholdChanged = true;
            color_blue = null;
            color_gray = null;
            color_red = null;
            settings = null;
            keyNameToElementMap = null;
            current_doc = null;
            m_uiApp.Idling -= SettingIdlingHandler;
            m_uiApp.Idling -= IdlingHandler;
            m_uiApp.Idling -= ImageControlIdlingHandler;
        }
        private void DocumentOpenedAction(object sender, DocumentOpenedEventArgs even)
        {
            settings = ExtensibleStorage.GetTooltipInfo(even.Document.ProjectInformation);
            this.mysql = new MysqlUtil(settings);
            this.sqlite = new SQLiteHelper(settings);
            m_previousDocPathName = even.Document.PathName;
            current_doc = even.Document;

            //���˲���ʹ��
            //���ĵ��͹���һ��
            keyNameToElementMap = new Dictionary<string, Element>();
            BuiltInParameter testParam = BuiltInParameter.ALL_MODEL_TYPE_NAME;
            ParameterValueProvider pvp = new ParameterValueProvider(new ElementId(testParam));
            FilterStringEquals eq = new FilterStringEquals();
            FilterRule rule = new FilterStringRule(pvp, eq, Res.String_ParameterSurveyType, false);
            ElementParameterFilter paramFilter = new ElementParameterFilter(rule);
            Document document = current_doc;
            FilteredElementCollector elementCollector = new FilteredElementCollector(document).OfClass(typeof(FamilyInstance));
            IList<Element> elems = elementCollector.WherePasses(paramFilter).ToElements();
            foreach (var elem in elems)
            {
                string param_value = string.Empty;
                Parameter param = elem.get_Parameter(Res.String_ParameterName);
                if (null != param && param.StorageType == StorageType.String)
                {
                    param_value = param.AsString();
                    if (!string.IsNullOrWhiteSpace(param_value))
                    {
                        keyNameToElementMap.Add(param_value, elem);
                    }
                }
            }


            //׼��Material
            IEnumerable<Material> allMaterial = new FilteredElementCollector(even.Document).OfClass(typeof(Material)).Cast<Material>();
            foreach (Material elem in allMaterial)
            {
                if (elem.Name.Equals(Res.String_Color_Redline))
                {
                    color_red = elem;
                }
                if (elem.Name.Equals(Res.String_Color_Gray))
                {
                    color_gray = elem;
                }
                if (elem.Name.Equals(Res.String_Color_Blue))
                {
                    color_blue = elem;
                }
                if (color_gray != null && color_red != null && color_blue != null)
                {
                    this.ColorMaterialIsReady = true;
                    break;
                }
            }

        }
    }
}
