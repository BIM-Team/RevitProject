using Revit.Addin.RevitTooltip.Util;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Revit.Addin.RevitTooltip.UI
{
    public partial class ProcessBarForm : Form
    {
        private IWriter dbWriter=null;
       
        public ProcessBarForm(IWriter dbw)
        {
            this.dbWriter = dbw;
            InitializeComponent();
        }

        private void ProcessBarForm_Shown(object sender, System.EventArgs e)
        {
            ExcelReader excel = new ExcelReader();
            ExcelReader excel1 = new ExcelReader();
            ExcelReader excel2 = new ExcelReader();
            RevitTooltip settings = App.settings;
            if (!string.IsNullOrEmpty(settings.SurveyFile))
            {
                progressLabel.Text = "现在正在导入测量数据";
                excel.FilePath_Data = settings.SurveyFile;
                excel.UseExcelType = ExcelType.DataExcel;
                List<SheetInfo> sheets = excel.getSheetInfo_Range(0, excel.getSheetCount());
                foreach (SheetInfo sheet in sheets)
                {
                    dbWriter.InsertSheetInfo(sheet);
                    if (sheet.SheetIndex <= 33)
                    {
                      progressBar.Value++;
                    }
                }

            }
            if (!string.IsNullOrEmpty(settings.FoundationFile))
            {
                progressLabel.Text = "现在正在导入基础数据";
                progressBar.Value = 33;
                excel1.FilePath_base = settings.FoundationFile;
                excel1.UseExcelType = ExcelType.BaseExcel;

                List<SheetInfo> sheets = excel1.getSheetInfo_Range(0, excel1.getSheetCount());
                foreach (SheetInfo sheet in sheets)
                {
                    dbWriter.InsertSheetInfo(sheet);
                    if (sheet.SheetIndex <= 33)
                    {
                        progressBar.Value++;
                    }
                }

            }
            if (!string.IsNullOrEmpty(settings.UnderWallFile))
            {
                progressLabel.Text = "现在正在导入基础城墙数据";
                progressBar.Value = 66;
                excel2.FilePath_Wall = settings.UnderWallFile;
                excel2.UseExcelType = ExcelType.WallExcel;
                List<SheetInfo> sheets = excel2.getSheetInfo_Range(0, excel2.getSheetCount());
                foreach (SheetInfo sheet in sheets)
                {
                    dbWriter.InsertSheetInfo(sheet);
                    if (sheet.SheetIndex <= 33)
                    {
                        progressBar.Value++;
                    }
                }

            }
            progressLabel.Text = "导入完成";
            this.DialogResult = DialogResult.OK;
        }
    }
}
