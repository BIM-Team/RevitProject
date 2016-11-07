using System.Collections.Generic;
using Revit.Addin.RevitTooltip.Dto;

namespace Revit.Addin.RevitTooltip.Intface
{
    interface IMysqlUtil
    {
        /// <summary>
        /// 用测试语句测试当前Mysql的可用状态
        /// </summary>
        /// <returns></returns>
        bool isReady();
        /// <summary>
        /// 插入SheetInfo
		///返回成功完成了多少个Entity相关的数据
        /// </summary>
        int InsertSheetInfo(SheetInfo sheetInfo);
		
		/// <summary>
        /// 新建一个Group
		///在选定的某种Excel的基础上新建一个Group
		///传入Signal标志
        /// </summary>
		bool AddKeyGroup(string Signal, string GroupName);
		
		/// <summary>
        /// 删除一个Group
        /// </summary>
		bool DeleteKeyGroup(int Group_ID);
		
		/// <summary>
        /// 修改某个Group的GroupName
        /// </summary>
		bool ModifyKeyGroup(int Group_ID, string GroupName);

        /// <summary>
        /// 添加一些属性名到某一Group
        /// </summary>
        bool AddKeysToGroup(int Group_ID, List<int> Key_Ids);

        /// <summary>
        /// 列举所有的Info Excel等待分组，这里不包括测量数据的excel
        ///返回的Dictionary中
        ///string2:signal简写（全局唯一）
        ///string1:desc描述
        /// </summary>
        Dictionary<string, string> ListExcelToGroup();
		
		/// <summary>
        /// 修改Entity的Remark
        /// </summary>
		bool ModifyEntityRemark(string EntityName, string Remark);

		/// <summary>
        /// 修改阈值Total_hold和Diff_hold
		///修改某一种Excel表的阈值，这里的Excel表必须是测量数据表
        /// </summary>
		bool ModifyThreshold(string signal, double Total_hold, double Diff_hold);

		/// <summary>
        /// 复制MySQL中的数据到Sqlite中
        /// </summary>
        bool LoadDataToSqlite();

    }
}
