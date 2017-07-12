using Revit.Addin.RevitTooltip.Dto;
using System;
using System.Collections.Generic;

namespace Revit.Addin.RevitTooltip.Intface
{
    public interface ISQLiteHelper
    {
        /// <summary>
        /// 插入SheetInfo
        /// </summary>
        void InsertSheetInfo(SheetInfo sheetInfo);

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
        DrawEntityData SelectDrawEntityData(string EntityName, DateTime? StartTime, DateTime? EndDate);


        /// <summary>
        /// 查询DrawDataTable
        ///查询Entity某日期的数据
        /// </summary>
        List<DrawData> SelectDrawData( String signal,DateTime Date);
        /// <summary>
        /// 通过传入的Signal，查询与之对应的所有的测点
        ///传入的Signal应该是测量数据的signal
        /// </summary>
        List<CEntityName> SelectAllEntitiesAndErr(string ExcelSignal,DateTime? start=null,DateTime? end=null);
        /// <summary>
        /// 获取所有的entity和ErrMsg
        /// </summary>
        /// <returns></returns>
        List<CEntityName> SelectAllEntitiesAndErrIgnoreSignal();

        /// <summary>
        /// 复制MySQL中的数据到Sqlite中
        /// </summary>
        bool LoadDataToSqlite();
        /// <summary>
        /// 查询所有的测量数据类型
        /// </summary>
        /// <returns></returns>
        List<ExcelTable> SelectDrawTypes();
        /// <summary>
        ///修改备注
        /// </summary>
        /// <param name="EntityName">实体名</param>
        /// <param name="Remark">备注</param>
        /// <returns>是否成功</returns>
        bool ModifyEntityRemark(string EntityName, string Remark);
        /// <summary>
        /// 用于回退
        /// </summary>
        /// <param name="timeStamp"></param>
        void RollBack(string timeStamp);
        /// <summary>
        /// 列举Threshold
        /// </summary>
        /// <param name="isInfo"></param>
        /// <returns></returns>
        List<ExcelTable> ListExcelsMessage(bool isInfo);
        /// <summary>
        /// 列举分组
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        List<Group> loadGroupForAExcel(string signal);
        /// <summary>
        /// 列举分组KeyNames的分组信息
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        List<CKeyName> loadKeyNameForExcelAndGroup(string signal,int group_id=-1);
        /// <summary>
        /// 添加KeyNames到分组
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="oK_ids"></param>
        /// <returns></returns>
        bool AddKeysToGroup(int group_id, List<int> oK_ids);
        /// <summary>
        /// 修改备注
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="total_hold"></param>
        /// <param name="diff_hold"></param>
        bool ModifyThreshold(string signal, string total_hold, string diff_hold,string totalOpr,string diffOpr);
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteGroup(int id);
        Group AddNewGroup(string signal, string newGroupName);
        ExcelTable SelectADrawType(string EntityName);
    }
}