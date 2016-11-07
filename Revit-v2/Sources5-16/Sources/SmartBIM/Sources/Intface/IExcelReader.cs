using Revit.Addin.RevitTooltip.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revit.Addin.RevitTooltip.Intface
{
    interface IExcelReader
    {
        /// <summary>
        /// 加载全部Excel表数据，适用数据量较少的Excel
        /// 这里对Excel的fileName有要求
        /// filename=Desc+Signal+InfoOrDraw+HOrV
        /// 即：中文描述+简称+数据用途（作为属性数据显示，还是绘制折线图）+是横向读取还是竖向读取
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        SheetInfo loadExcelData(string filename);
        /// <summary>
        /// 加载部分Excel表格数据
        /// 这里对Excel的fileName有要求
        /// filename=Desc+Signal+InfoOrDraw+HOrV
        /// 即：中文描述+简称+数据用途（作为属性数据显示，还是绘制折线图）+是横向读取还是竖向读取
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="startPos">起始sheet的位置</param>
        /// <param name="len">读取sheet的数量</param>
        /// <returns></returns>
        SheetInfo loadSheetsData(string filename, int startPos, int len);
    }
}
