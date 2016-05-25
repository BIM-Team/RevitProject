using BIMCoder.OfficeHelper.ExcelCommon;
using System;
using System.Collections.Generic;

namespace Revit.Addin.RevitTooltip.Util
{
    public class ExcelReader:IExcelReader
    {
        //局部变量，临时存放一张sheet
        private SheetInfo sheetInfo;


        //基础表的路径
        private string _filePath_base;
        public string FilePath_base {
            set {
                _filePath_base = value;
            }
        }

        //存放城墙表格的路径
        private string _filePath_wall;
        public string FilePath_Wall {
            set {
                _filePath_wall = value;
            }
        }

        //测试数据表格的路径
        private string _filePath_data;
        public string FilePath_Data {
            set {
                _filePath_data = value;
            }
        }

        //excelhelper第三方程序集
        private ExcelHelper excel = null;

        //文件是否打开
        private bool isOpen = false;

        //当前的阅读模式，基础数据，城墙数据或是测量数据
        private ExcelType excelType;
        public ExcelType UseExcelType {
            set {
                this.excelType = value;
            }
        }

        //
        //
        public ExcelReader() {
            sheetInfo = new SheetInfo();
            excel = new ExcelHelper();
        }

        //重置sheetInfo
        private void ResetData() {
            sheetInfo = new SheetInfo();
        }

        //读取基础数据或是城墙数据，但是不能处理测量数据
        public SheetInfo getSheetInfo_BaseOrWall(int sheetIndex) {

            if (excelType ==ExcelType.None|| excelType == ExcelType.DataExcel) {
                throw new Exception("类型不匹配");
            }

            string path = string.Empty;
            switch (excelType) {
                case ExcelType.BaseExcel:path = _filePath_base;break;
                case ExcelType.WallExcel:path = _filePath_wall; break;
            }

            if (string.IsNullOrEmpty(path)) {//没有路径
                throw new Exception("没有赋值路径");
            }

            if (!isOpen) {//没有打开
                OpenExcel(path);
            }
            excel.SetActiveSheet(sheetIndex);//可用于读取当前
            //
            int rowCount = excel.GetUsedRowCount(sheetIndex);
            int colCount = excel.GetUsedColumnCount(sheetIndex);
            //
            if (rowCount < 1 || colCount < 1)
            {//没有数据
                return null;
            }

            int sheetNum = excel.GetSheetCount();
            if (sheetIndex >= sheetNum)
            {//
                throw new Exception("索引越界");
            }
            //
            //设置属性，即表头属性
            //基础数据和城墙数据为第一行除第一项之后的数据
            //城墙数据为第一列除第一项之后的数据
            for (int i = 1; i < colCount; i++) {
                string str = excel.GetCellValue(0, i).Trim();
                //已经没有更多属性
                if (string.IsNullOrEmpty(str)) {
                    break;
                }
                sheetInfo.Names.Add(str);
            }

            //
            //设置实体
            for (int i = 1; i < rowCount; i++) {
                string str = excel.GetCellValue(i,0);
                //
                //没有更多的实体
                if (string.IsNullOrEmpty(str)) {
                    break;
                }
                sheetInfo.EntityName.Add(str);
                //
                List<string[]> datas = new List<string[]>();
                string[] data = new string[sheetInfo.Names.Count+1];
                data[0] = str;
                for (int j=1;j< data.Length; j++) {
                    data[j] = excel.GetCellValue(i, j).Trim();
                }

                datas.Add(data);

                sheetInfo.Data.Add(str, datas);//存放数据
            }
            sheetInfo.TableType = excelType;//设置表类型
            sheetInfo.SheetIndex = sheetIndex;//保存索引值
            return sheetInfo;
        }

        //获取测量数据
        //测量数据，以第一列的深度作为属性
        //
        public SheetInfo getSheetInfo_Data(int sheetIndex) {
            if (excelType != ExcelType.DataExcel) {
                throw new Exception("类型不匹配");
            }
            if (string.IsNullOrEmpty(_filePath_data)) {
                throw new Exception("测量数据路径没有赋值");
            }
            if (!isOpen)
            {//没有打开
                OpenExcel(_filePath_data);
            }

            //
            int rowCount = excel.GetUsedRowCount(sheetIndex);
            int colCount = excel.GetUsedColumnCount(sheetIndex);

            int sheetNum = excel.GetSheetCount();
            if (sheetIndex >= sheetNum) {//
                throw new Exception("索引越界");
            }
            excel.SetActiveSheet(sheetIndex);

          //
            
            for (int i = 1; i < rowCount; i++) {//设置表头
                string str = excel.GetCellValue(i,0).Trim();
                //没有更多的属性
                if (string.IsNullOrEmpty(str)) {
                    break;
                }
                sheetInfo.Names.Add(str);
                
            }

            //数据
            List<string[]> datas = new List<string[]>();
             for (int i = 1; i < colCount; i++)
            {
                //
                string[] data = new string[sheetInfo.Names.Count+1];//存放一列，第一个表示日期
                                                                    //
                //（1,i）为空则认为该列无数据
                if (string.IsNullOrEmpty(excel.GetCellValue(1, i))) {
                    continue;
                }

                //获取测量时间
                string date = excel.GetCellValue(0, i).Trim();
                if (string.IsNullOrEmpty(date)) {
                    throw new Exception("没有日期");
                }
                try
                {
                double d = Double.Parse(date);//Excel表为日期格式，且不包括中文
                data[0] = DateTime.FromOADate(d).ToString("yyyy/MM/dd");
                }
                catch (Exception)
                {
                    data[0] = date;//自定义时间或异常时间
                }

                //获取数据
                for (int j = 1; j < data.Length; j++)
                {
                    data[j] = excel.GetCellValue(j, i).Trim();
                }
                datas.Add(data);
            }


            //实体名
            string s = excel.GetCellValue(0,0).Trim();
            //
            sheetInfo.Data.Add(s, datas);//存放数据
            //
            sheetInfo.TableType = excelType;//设置表类型

            //存放实体名
            //测量数据的每个sheet只包含一个实体，所以长度为1
            sheetInfo.EntityName.Add(s);//实体名
            //
            sheetInfo.SheetIndex = sheetIndex;//保存索引值
            return sheetInfo;
        }
        //批量获取，from 到to,不包括to
        public List<SheetInfo> getSheetInfo_Range(int from,int to) {
            if (from < 0 || to < from||to>this.getSheetCount()) {//错误
                throw new Exception("超出边界");
            }
            GetSheetInfo function =null;
            switch (excelType) {
                case ExcelType.BaseExcel:
                case ExcelType.WallExcel: function = getSheetInfo_BaseOrWall; break;
                case ExcelType.DataExcel: function = getSheetInfo_Data; break;
                case ExcelType.None: throw new Exception("还没有指定阅读模式");
            }
            List<SheetInfo> listSheetInfo = new List<SheetInfo>();
            for (int i = from; i < to; i++){
                ResetData();//重置sheetInfo
                SheetInfo sheet = function(i);
                if (sheet == null)
                {
                    break;
                }
                listSheetInfo.Add(sheet);
            }
            return listSheetInfo;
        }
        public int getSheetCount() {//计算sheet总数
            string path = string.Empty;

            switch (excelType) {
                case ExcelType.BaseExcel: path = _filePath_base;break;
                case ExcelType.WallExcel:path = _filePath_wall;break;
                case ExcelType.DataExcel:path = _filePath_data; break;
                case ExcelType.None:throw new Exception("还没有指定阅读模式");
                       
            }
            if (string.IsNullOrEmpty(path)) {
                return 0;
            }
            if (!isOpen) {
                OpenExcel(path);
            }
            return excel.GetSheetCount();
        }
        private void OpenExcel(string path) {//文件是否已经打开
            try
            {
                isOpen = excel.Open(path);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public void Close() {//关闭
            excel = null;
            sheetInfo = null;      
        }
    }
}
