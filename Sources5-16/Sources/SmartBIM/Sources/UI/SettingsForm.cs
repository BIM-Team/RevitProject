using System;
using System.Windows.Forms;
using System.IO;
using Revit.Addin.RevitTooltip.Util;
using System.Collections.Generic;

namespace Revit.Addin.RevitTooltip
{
    public partial class SettingsForm : Form
    {
        private Autodesk.Revit.DB.Document m_doc = null;
        public SettingsForm(Autodesk.Revit.DB.Document doc)
        {
            InitializeComponent();

            m_doc = doc;
            RevitTooltip settings = ExtensibleStorage.GetTooltipInfo(m_doc.ProjectInformation);
            if (null != settings)
            {
                 //
                textBoxSurveyFile.Text = settings.SurveyFile;
                textBoxAlert.Text = settings.AlertNumber.ToString();
                alertNumberAdd.Text = settings.AlertNumberAdd.ToString();
                 //
                textBoxFoundationFile.Text = settings.FoundationFile;
                //
                textBoxUnderWallFile.Text = settings.UnderWallFile;
                //
                textServerPath.Text = settings.DfServer;
                textDB.Text = settings.DfDB;
                textPort.Text = settings.DfPort;
                textUser.Text = settings.DfUser;
                textPass.Text = settings.DfPassword;


            }
            else {
                settings = RevitTooltip.Default;
                string dir = Path.GetDirectoryName(this.GetType().Assembly.Location);


                textBoxSurveyFile.Text = Path.Combine(dir, settings.SurveyFile) ;
                textBoxAlert.Text = settings.AlertNumber.ToString();
                alertNumberAdd.Text = settings.AlertNumberAdd.ToString();

                textBoxFoundationFile.Text = Path.Combine(dir, settings.FoundationFile);
                textBoxUnderWallFile.Text = Path.Combine(dir, settings.UnderWallFile);
                textServerPath.Text = settings.DfServer;
                textDB.Text = settings.DfDB;
                textPort.Text = settings.DfPort;
                textUser.Text = settings.DfUser;
                textPass.Text = settings.DfPassword;


            }
            this.ActiveControl = textBoxSurveyFile;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            buttonOK.Enabled = false;
            try
            {
                RevitTooltip settings = ExtensibleStorage.GetTooltipInfo(m_doc.ProjectInformation);
                if (null == settings)
                {
                    settings = RevitTooltip.Default;
                }
                    settings.AlertNumber = double.Parse(textBoxAlert.Text);
                    settings.AlertNumberAdd = double.Parse(alertNumberAdd.Text);
                    settings.SurveyFile = textBoxSurveyFile.Text;
                    settings.FoundationFile = textBoxFoundationFile.Text;
                    settings.UnderWallFile = textBoxUnderWallFile.Text;
                    settings.DfServer = textServerPath.Text;
                    settings.DfDB = textDB.Text;
                    settings.DfPort = textPort.Text;
                    settings.DfUser = textUser.Text;
                    settings.DfPassword = textPass.Text;
                ExtensibleStorage.StoreTooltipInfo(m_doc.ProjectInformation, settings);
                groupBox5.Visible = true;
                progressBar.Visible = true;
                ExcelReader excel =new  ExcelReader();
                ExcelReader excel1 = new ExcelReader();
                ExcelReader excel2 = new ExcelReader();
                if (!string.IsNullOrEmpty(settings.SurveyFile)) {
                    groupBox5.Text = "现在正在导入测量数据";
                    excel.FilePath_Data = settings.SurveyFile;
                    excel.UseExcelType = ExcelType.DataExcel;
                    List<SheetInfo> sheets=excel.getSheetInfo_Range(0,excel.getSheetCount());
                    foreach (SheetInfo sheet in sheets) {
                        MysqlUtil.CreateInstance(settings).InsertSheetInfo(sheet);
                        if (sheet.SheetIndex <= 33) {
                            progressBar.Value++;
                        }
                    }

                }
                if (!string.IsNullOrEmpty(settings.FoundationFile))
                {
                    groupBox5.Text = "现在正在导入基础数据";
                    progressBar.Value = 34;
                    excel1.FilePath_base = settings.FoundationFile;
                    excel1.UseExcelType = ExcelType.BaseExcel;

                    List<SheetInfo> sheets = excel1.getSheetInfo_Range(0, excel1.getSheetCount());
                    foreach (SheetInfo sheet in sheets)
                    {
                        MysqlUtil.CreateInstance(settings).InsertSheetInfo(sheet);
                        if (sheet.SheetIndex <= 33)
                        {
                            progressBar.Value++;
                        }
                    }

                }
                if (!string.IsNullOrEmpty(settings.UnderWallFile))
                {
                    groupBox5.Text = "现在正在导入城墙数据";
                    progressBar.Value = 67;
                    excel2.FilePath_Wall = settings.UnderWallFile;
                    excel2.UseExcelType = ExcelType.WallExcel;
                    List<SheetInfo> sheets = excel2.getSheetInfo_Range(0, excel2.getSheetCount());
                    foreach (SheetInfo sheet in sheets)
                    {
                        MysqlUtil.CreateInstance(settings).InsertSheetInfo(sheet);
                        if (sheet.SheetIndex <= 33)
                        {
                            progressBar.Value++;
                        }
                    }

                }
                groupBox5.Text = "导入完成";
                progressBar.Value = 100;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonBrowseSurvey_Click(object sender, EventArgs e)
        {
            string excelFile = CmdLoadFile.LoadFromFile();
            if (File.Exists(excelFile))
            {
                textBoxSurveyFile.Text = excelFile;
            }
        }

        private void buttonBrowseFoundationFile_Click(object sender, EventArgs e)
        {
            string excelFile = CmdLoadFile.LoadFromFile();
            if (File.Exists(excelFile))
            {
                textBoxFoundationFile.Text = excelFile;
            }
        }

        private void buttonBrowseUnderWallFile_Click(object sender, EventArgs e)
        {
            string excelFile = CmdLoadFile.LoadFromFile();
            if (File.Exists(excelFile))
            {
                textBoxUnderWallFile.Text = excelFile;
            }
        }
    }
}
