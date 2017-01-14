using Revit.Addin.RevitTooltip.Intface;
using System;
using System.Linq;
using Revit.Addin.RevitTooltip.Dto;
using BIMCoder.OfficeHelper.ExcelCommon;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Revit.Addin.RevitTooltip.Impl
{
    public class ExcelReader : IExcelReader
    {

        public void readExcelData(string fileName, SheetInfo sheetInfo, string parts2)
        {
            ExcelHelper eHelper = new ExcelHelper();
            eHelper.Open(fileName);
            int sheetNum = eHelper.GetSheetCount();
            if (sheetNum == 0)
            {
                throw new Exception("Excel表格为空");
            }
            //实体名
            sheetInfo.EntityNames = new List<string>();
            switch (parts2)
            {
                //属性数据
                case "I":
                    {
                        //属性名，即表头
                        sheetInfo.KeyNames = new List<string>();
                        sheetInfo.InfoRows = new List<InfoEntityData>();
                        int col = eHelper.GetUsedColumnCount(0);
                        eHelper.SetActiveSheet(0);
                        //读取表头，即KeyNames
                        for (int c = 1; c < col; c++)
                        {
                            string value = eHelper.GetCellValue(0, c);
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                sheetInfo.KeyNames.Add(value);
                            }
                        }
                        //读取表格数据
                        for (int i = 0; i < sheetNum; i++)
                        {
                            eHelper.SetActiveSheet(i);
                            //当前sheet的行数
                            int row = eHelper.GetUsedRowCount(i);
                            //读取EntityNames，跳过第一行
                            for (int r = 1; r < row; r++)
                            {
                                string value = eHelper.GetCellValue(r, 0);
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    sheetInfo.EntityNames.Add(value);
                                    InfoEntityData infoEntityData = new InfoEntityData();
                                    infoEntityData.EntityName = value;
                                    infoEntityData.Data = new Dictionary<string, string>();
                                    //读取属性值
                                    for (int c = 1; c < col; c++)
                                    {
                                        string key2 = eHelper.GetCellValue(r, c);
                                        if (!string.IsNullOrWhiteSpace(key2))
                                        {
                                            string key1 = eHelper.GetCellValue(0, c);
                                            if (string.IsNullOrWhiteSpace(key1))
                                            {
                                                throw new Exception("属性值对应表头为空");
                                            }
                                            if (!key1.Equals(sheetInfo.KeyNames[c - 1])) {
                                                throw new Exception("前后表头不一致");
                                            }
                                            infoEntityData.Data.Add(key1, key2);
                                        }
                                    }
                                    sheetInfo.InfoRows.Add(infoEntityData);
                                }
                            }

                        }
                        break;
                    }
                //一般的测量数据
                case "C":
                    {
                        sheetInfo.DrawRows = new List<DrawEntityData>();
                        sheetInfo.EntityNames = new List<string>();
                        for (int i = 0; i < sheetNum; i++)
                        {
                            eHelper.SetActiveSheet(i);
                            int row = eHelper.GetUsedRowCount(i);
                            int col = eHelper.GetUsedColumnCount(i);
                            //读取实体列表
                            for (int r = 1; r < row; r++)
                            {
                                string entityName = eHelper.GetCellValue(r, 0);
                                if (!string.IsNullOrWhiteSpace(entityName))
                                {
                                    sheetInfo.EntityNames.Add(entityName);
                                    DrawEntityData drawEntityData = new DrawEntityData();
                                    drawEntityData.EntityName = entityName;
                                    drawEntityData.Data = new List<DrawData>();
                                    //读取数据和对应的日期，数据为空，则不读取日期
                                    //后续会统一excel表格中的日期格式
                                    for (int c = 1; c < col; c++)
                                    {
                                        string value = eHelper.GetCellValue(r, c);
                                        if (!string.IsNullOrWhiteSpace(value))
                                        {
                                            float dValue = 0;
                                            try
                                            {
                                                dValue = float.Parse(value);
                                            }
                                            catch (Exception) {
                                                continue;
                                            }
                                            dValue = (float)Math.Round(dValue, 2, MidpointRounding.AwayFromZero);
                                            double date = Convert.ToDouble(eHelper.GetCellValue(0, c));
                                            DateTime dateTime;
                                            try
                                            {
                                                dateTime = DateTime.FromOADate(date);
                                            }
                                            catch (Exception)
                                            {
                                                throw;
                                            }
                                            DrawData drawData = new DrawData();
                                            drawData.Date = dateTime;
                                            drawData.MaxValue = drawData.MidValue = drawData.MinValue = dValue;
                                            drawData.Detail = dValue.ToString();
                                            drawEntityData.Data.Add(drawData);
                                        }
                                    }
                                            sheetInfo.DrawRows.Add(drawEntityData);
                                }
                            }
                        }
                        break;
                    }
                case "CX":
                    {
                        sheetInfo.DrawRows = new List<DrawEntityData>();
                        sheetInfo.EntityNames = new List<string>();
                        StringBuilder text = new StringBuilder();
                        for (int i = 0; i < sheetNum; i++)
                        {
                            eHelper.SetActiveSheet(i);
                            string entityName = eHelper.GetCellValue(0, 0);
                            if (string.IsNullOrWhiteSpace(entityName))
                            {
                                throw new Exception("数据异常");
                            }
                            sheetInfo.EntityNames.Add(entityName);
                            DrawEntityData drawEntityData = new DrawEntityData();
                            drawEntityData.EntityName = entityName;
                            drawEntityData.Data = new List<DrawData>();
                            int row = eHelper.GetUsedRowCount(i);
                            int col = eHelper.GetUsedColumnCount(i);
                            text.Clear();
                            for (int c = 1; c < col; c++)
                            {
                                string _min = eHelper.GetCellValue(1, c);
                                //如果第一个测量值为空，跳过该列
                                if (string.IsNullOrWhiteSpace(_min)){
                                    continue;
                                }

                                float min = 0;
                                try
                                {
                                    min = float.Parse(_min);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                                min = (float)Math.Round(min,2,MidpointRounding.AwayFromZero);
                                float mid = min;
                                float max = min;
                                int minCount = 0;
                                int maxCount = 0;

                                for (int r = 1; r < row; r++)
                                {
                                    string value = eHelper.GetCellValue(r, c);
                                    if (!string.IsNullOrWhiteSpace(value))
                                    {
                                        float dValue = 0;
                                        try
                                        {
                                            dValue = float.Parse(value);
                                        }
                                        catch (Exception)
                                        {
                                            continue;
                                        }
                                        dValue = (float)Math.Round(dValue, 2, MidpointRounding.AwayFromZero);
                                        text.Append(eHelper.GetCellValue(r, 0) + ":" + dValue + ";");
                                        //计算最小值
                                        if (min > dValue)
                                        {
                                            min = dValue;
                                        }
                                        //计算最大值
                                        if (max < dValue)
                                        {
                                            max = dValue;
                                        }
                                        //计算中位数
                                        if (mid < dValue)
                                        {
                                            maxCount++;
                                        }
                                        else
                                        {
                                            minCount++;
                                        }
                                        if (minCount < maxCount)
                                        {
                                            mid = mid > dValue ? mid : dValue;
                                            minCount++;
                                            maxCount--;
                                        }
                                        else
                                        {
                                            mid = mid < dValue ? mid : dValue;
                                            minCount--;
                                            maxCount++;
                                        }
                                    }
                                }
                                text.Remove(text.Length - 1, 1);
                                //生成每行数据
                                DrawData drawData = new DrawData();
                                double date = Convert.ToDouble(eHelper.GetCellValue(0, c));
                                drawData.Date = DateTime.FromOADate(date);
                                drawData.MaxValue = max;
                                drawData.MidValue = mid;
                                drawData.MinValue = min;
                                drawData.Detail = text.ToString();
                                drawEntityData.Data.Add(drawData);
                            }
                                //添加一张sheet的数据
                                sheetInfo.DrawRows.Add(drawEntityData);
                        }
                        break;
                    }
            }
        }
        public SheetInfo loadExcelData(string filePath)
        {
            //文件名为空
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("文件名为空");
            }
            string filename=System.IO.Path.GetFileName(filePath);

            string[] parts = filename.Split("-_.".ToCharArray());
            //格式定义：I,C,CX
            if (parts.Length != 4 || !new string[] { "I", "C", "CX" }.Contains(parts[2]))
            {
                throw new Exception("文件名格式错误");
            }
            SheetInfo result = new SheetInfo();
            ExcelTable exceltable = new ExcelTable();
            exceltable.CurrentFile = parts[0];
            exceltable.Signal = parts[1];
            result.ExcelTableData = exceltable;
            result.Tag = parts[2].Equals("I");
            readExcelData(filePath, result, parts[2]);
            return result;
        }

        public SheetInfo loadSheetsData(string filename, int startPos, int len)
        {
            throw new NotImplementedException();
        }
    }
}