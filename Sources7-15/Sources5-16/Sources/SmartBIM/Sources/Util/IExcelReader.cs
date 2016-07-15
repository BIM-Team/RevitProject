using System.Collections.Generic;

namespace Revit.Addin.RevitTooltip.Util

{
    interface IExcelReader
    {
        //设置基础数据Excel表的路径
        string FilePath_base
        {
            set;
        }
        //
        //设置城墙数据的路径
        string FilePath_Wall
        {
            set;
        }
        //
        //设置测量数据的路径
        string FilePath_Data
        {
            set;

        }

        //设置当前的阅读模式，基础数据，城墙数据或者测量数据
        ExcelType UseExcelType
        {
            set;
        }

        //读取基础数据和城墙数据，每次返回一个sheet
        SheetInfo getSheetInfo_BaseOrWall(int sheetIndex);

        //读取测量数据，每次返回单个sheet
        SheetInfo getSheetInfo_Data(int sheetIndex);

        //批量获取信息
        //批量获取，from 到to,不包括to
        List<SheetInfo> getSheetInfo_Range(int from, int to);

        //获取Excel的sheet总数
        int getSheetCount();


        //关闭
        void Close();

    }
}
