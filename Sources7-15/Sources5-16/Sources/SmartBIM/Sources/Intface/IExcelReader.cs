using Revit.Addin.RevitTooltip.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revit.Addin.RevitTooltip.Intface
{
    public interface IExcelReader
    {
        /// <summary>
        /// 加载全部Excel表数据，适用数据量较少的Excel
        /// 这里对Excel的fileName有要求
        /// filename=Desc+Signal+excelType
        /// 即：中文描述+简称+excel表的种类（I：属性表 C:普通测量表 CX:测斜汇总)
        /// </summary>
        /// <param name="filePath">全路径文件名</param>
        /// <returns></returns>
        SheetInfo loadExcelData(string filePath);
        /// <summary>
        /// 加载部分Excel表格数据
        /// 这里对Excel的fileName有要求
        ///  filename=Desc+Signal+excelType
        /// 即：中文描述+简称+excel表的种类（I：属性表 C:普通测量表 CX:测斜汇总)
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="startPos">起始sheet的位置</param>
        /// <param name="len">读取sheet的数量</param>
        /// <returns></returns>
        SheetInfo loadSheetsData(string filename, int startPos, int len);
    }
}