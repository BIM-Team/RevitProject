using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

using BIMCoder.OfficeHelper.ExcelCommon;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Addin.RevitTooltip.Properties;

namespace Revit.Addin.RevitTooltip
{
    class Utils
    {
        /// <summary>
        /// Convert Bitmap to BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ConvertFromBitmap(System.Drawing.Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Return currently active UIView or null.
        /// </summary>
        public static UIView GetActiveUiView(
          UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            View view = doc.ActiveView;
            IList<UIView> uiviews = uidoc.GetOpenUIViews();
            UIView uiview = null;

            foreach (UIView uv in uiviews)
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiview = uv;
                    break;
                }
            }
            return uiview;
        }

       

        public static Parameter GetParameter(Element elem, string param)
        {
#if(Since2016)
            return elem.LookupParameter(param);
#else
            return elem.get_Parameter(param);
#endif
        }

        public static string GetParameterValueAsString(Element elem, string parameterName)
        {
#if(Since2016)
            Parameter _param = elem.LookupParameter(parameterName);
#else
            Parameter _param = elem.get_Parameter(parameterName);
#endif
            if (null == _param || _param.StorageType != StorageType.String)
                return null;

            return _param.AsString();
        }
    }

    /// <summary>
    /// 监测数据信息
    /// </summary>
    public class SurveyDataInfo
    {
        /// <summary>
        /// 名称：轨道交通5号线环境监测1标段-望园路站安全监测
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 监测单位
        /// </summary>
        public string MonitorDivision { get; set; }

        /// <summary>
        /// 施工单位
        /// </summary>
        public string BuildDivision { get; set; }

        /// <summary>
        /// 上次监测日期
        /// </summary>
        public string LastTimeMonitorDate { get; set; }

        /// <summary>
        /// 本次监测日期
        /// </summary>
        public string CurrentMonitorDate { get; set; }

        /// <summary>
        /// 天气
        /// </summary>
        public string Weather { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 监测时间
        /// </summary>
        public string MonitorDatetime { get; set; }

        /// <summary>
        /// 所有测点信息数据
        /// </summary>
        public List<SurveyData> Data { get; set; }

        /// <summary>
        /// Loads from excel file
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        public static SurveyDataInfo LoadSurveyFromFile(string excelFile)
        {
            if (!File.Exists(excelFile))
            {
                throw new ArgumentException("The given file doesn't exist!");
            }

            ExcelHelper excel = new ExcelHelper();
            bool openSuccessful = excel.Open(excelFile);
            // Set the current workbook by opening current operation file
            if (!openSuccessful)
            {
                throw new ArgumentException("Open Excel File Failed");
            }

            int sheetIndex = 0;     // Currently only handle the first sheet
            excel.SetActiveSheet(sheetIndex);

            SurveyDataInfo result = new SurveyDataInfo();
            string surveyTableName = excel.GetCellValue(sheetIndex, 0, 0);
            if (!string.IsNullOrEmpty(surveyTableName))
            {
                result.Name = surveyTableName;
            }

            string monitorOrganization = excel.GetCellValue(sheetIndex, 1, 0);
            if (!string.IsNullOrEmpty(monitorOrganization))
            {
                result.MonitorDivision = monitorOrganization.Split('：')[1].Trim();
            }

            string buildOrganization = excel.GetCellValue(sheetIndex, 2, 0);
            if (!string.IsNullOrEmpty(buildOrganization))
            {
                result.BuildDivision = buildOrganization.Split('：')[1].Trim();
            }

            string lastTimeMonitorDate = excel.GetCellValue(sheetIndex, 3, 0);
            if (!string.IsNullOrEmpty(lastTimeMonitorDate))
            {
                result.LastTimeMonitorDate = lastTimeMonitorDate.Split('：')[1].Trim();
            }

            string currentMonitorDate = excel.GetCellValue(sheetIndex, 4, 0);
            if (!string.IsNullOrEmpty(currentMonitorDate))
            {
                result.CurrentMonitorDate = currentMonitorDate.Split('：')[1].Trim();
            }

            string weather = excel.GetCellValue(sheetIndex, 1, 3);
            if (!string.IsNullOrEmpty(weather))
            {
                result.Weather = weather.Split('：')[1].Trim();
            }

            string number = excel.GetCellValue(sheetIndex, 3, 3);
            if (!string.IsNullOrEmpty(number))
            {
                result.Number = number.Split('：')[1].Trim();
            }

            string monitorDatetime = excel.GetCellValue(sheetIndex, 4, 3);
            if (!string.IsNullOrEmpty(monitorDatetime))
            {
                result.MonitorDatetime = monitorDatetime.Split('：')[1].Trim();
            }

            // 读取Excel数据
            int usedRowCount = excel.GetUsedRowCount(sheetIndex);
            int usedColumnCount = excel.GetUsedColumnCount(sheetIndex);
            if (usedRowCount < 7 || usedColumnCount < 3)
            {
                throw new Exception("Please check the Excel File firstly.");
            }

            List<SurveyData> suveyData = new List<SurveyData>();
            for (int rowIndex = 7; rowIndex < usedRowCount; rowIndex++)
            {
                SurveyData oneData = new SurveyData();
                oneData.NO = excel.GetCellValue(sheetIndex, rowIndex, 0);
                if (string.IsNullOrEmpty(oneData.NO))
                {
                    continue;
                }

                oneData.CurrentChange = excel.GetCellValue(sheetIndex, rowIndex, 1);
                oneData.AccumulatedChange = excel.GetCellValue(sheetIndex, rowIndex, 2);
                oneData.Comment = excel.GetCellValue(sheetIndex, rowIndex, 3);

                suveyData.Add(oneData);
            }

            result.Data = suveyData;

            return result;
        }

        public static BIMDataTable LoadBIMDataFromFile(string excelFile)
        {
            ExcelHelper excel = new ExcelHelper();
            bool openSuccessful = excel.Open(excelFile);
            //
            // Set the current workbook by opening current operation file
            if (!openSuccessful)
            {
                throw new ArgumentException("Open Excel File Failed");
            }
            int sheetIndex = 0;     // Currently only handle the first sheet
            excel.SetActiveSheet(sheetIndex);
            int colCount = excel.GetUsedColumnCount(sheetIndex);
            int rowCount = excel.GetUsedRowCount(sheetIndex);

            BIMDataTable table = new BIMDataTable();
            for (int ii = 0; ii < colCount; ii++)
            {
                table.ColumnHeads.Add(excel.GetCellValue(0, ii));
            }

            if (!table.ColumnHeads.Contains(Resources.ExcelColumnHeader_Comment))
            {
                //添加备注列
                FileInfo excelInfo = new FileInfo(excelFile);
                if (excelInfo.IsReadOnly)
                {
                    excelInfo.Attributes &= ~FileAttributes.ReadOnly;
                }

                if (excel.CreateCell(0, colCount, Resources.ExcelColumnHeader_Comment) && excel.Save())
                {
                    table.ColumnHeads.Add(Resources.ExcelColumnHeader_Comment);
                }
                else
                {
                    throw new ApplicationException("添加备注列失败: " + excel.ErrMsg);
                }
            }

            for (int kk = 1; kk < rowCount; kk++)
            {
                BIMDataRow row = new BIMDataRow(table);
                for (int nn = 0; nn < colCount; nn++)
                {
                    row.CellValues.Add(excel.GetCellValue(kk, nn));
                }
                table.Rows.Add(row);
            }
            return table;
        }

        internal static void SaveComment(string excelFile, string elemCode, string comment)
        {
            ExcelHelper excel = new ExcelHelper();
            bool openSuccessful = excel.Open(excelFile);
            //
            // Set the current workbook by opening current operation file
            if (!openSuccessful)
            {
                throw new ApplicationException("Open Excel File Failed");
            }
            int sheetIndex = 0;     // Currently only handle the first sheet
            excel.SetActiveSheet(sheetIndex);
            int colCount = excel.GetUsedColumnCount(sheetIndex);
            int rowCount = excel.GetUsedRowCount(sheetIndex);

            int rowIndex = 0;
            for (int ii = 0; ii < rowCount; ii++)
            {
                //确认构件编号在excel第一列并且备注列在最后
                if (excel.GetCellValue(ii, 0) == elemCode)
                {
                    rowIndex = ii;
                    break;
                }
            }
            if (rowIndex == 0)
            {
                throw new ApplicationException("没有找到相关列");
            }
            //添加备注列
            FileInfo excelInfo = new FileInfo(excelFile);
            if (excelInfo.IsReadOnly)
            {
                excelInfo.Attributes &= ~FileAttributes.ReadOnly;
            }

            if (!excel.CreateCell(rowIndex, colCount - 1, comment))
            {
                throw new ApplicationException("添加备注失败: " + excel.ErrMsg);
            }
            if (!excel.Save())
            {
                throw new ApplicationException("保存备注失败: " + excel.ErrMsg);
            }
        }
    }

    /// <summary>
    /// 沉降点数据
    /// </summary>
    public class SurveyData
    {
        /// <summary>
        /// 测点编号
        /// </summary>
        public string NO { get; set; }

        /// <summary>
        /// 本次变化量
        /// </summary>
        public string CurrentChange { get; set; }

        /// <summary>
        /// 累计变化量
        /// </summary>
        public string AccumulatedChange { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        public SurveyData()
        {
            NO = "N/A";
            CurrentChange = AccumulatedChange = Comment = "";
        }
    }

    public class BIMDataTable
    {
        public List<string> ColumnHeads { get; set; }
        public List<BIMDataRow> Rows { get; set; }
        public BIMDataTable()
        {
            ColumnHeads = new List<string>();
            Rows = new List<BIMDataRow>();
        }
    }

    public class BIMDataRow
    {
        public BIMDataTable Table { get; set; }
        public List<string> CellValues { get; set; }
        public BIMDataRow(BIMDataTable _table)
        {
            Table = _table;
            CellValues = new List<string>();
        }
    }
}
