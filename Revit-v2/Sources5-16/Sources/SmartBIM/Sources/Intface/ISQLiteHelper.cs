using Revit.Addin.RevitTooltip.Dto;
using System;
using System.Collections.Generic;

namespace Revit.Addin.RevitTooltip.Intface
{
    interface ISQLiteHelper
    {
		/// <summary>
        /// 插入SheetInfo
        /// </summary>
        int InsertSheetInfo(SheetInfo sheetInfo);
        
		/// <summary>
        /// 查询InfoTable
		///返回的数据是已分组数据
        /// </summary>
		InfoEntityData SelectInfoData(string EntityName);
		
		/// <summary>
        /// 查询DrawDataTable
		///查询Entity时间序列数据
		///根据传入的起始时间查询
        /// </summary>
		DrawEntityData SelectDrawEntityData(string EntityName, DateTime StartTime, DateTime EndDate);
		
		
		/// <summary>
        /// 查询DrawDataTable
		///查询Entity某日期的数据
        /// </summary>
		DrawData SelectDrawData(string EntityName, DateTime Date);
		
	
		/// <summary>
        /// 查询Total_hold异常点
		///返回该类型的所有异常点
        /// </summary>
		List<string> SelectTotalThresholdEntity(string ExcelSignal, string TotalThreshold);
		
		/// <summary>
        /// 查询Diff_hold异常点
		///返回该类型的所有异常点
        /// </summary>
		List<string> SelectDiffThresholdEntity(string ExcelSignal, string DiffThreshold);
		
		/// <summary>
        /// 通过传入的Signal，查询与之对应的所有的测点
		///传入的Signal应该是测量数据的signal
        /// </summary>
		List<string> SelectAllEntities(string ExcelSignal);			

    }
}
