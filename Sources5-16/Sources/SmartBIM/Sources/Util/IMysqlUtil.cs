using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Revit.Addin.RevitTooltip;
using MySql.Data.MySqlClient;

namespace Revit.Addin.RevitTooltip.Util
{
    interface IMysqlUtil
    {
        //插入一个SheetInfo，自动检测，只插入以前表中不存在的数据
        int InsertSheetInfo(SheetInfo sheetInfo);

        //查询单个CX连续的测量结果（每个测量日期最大值），返回键值对<日期，数据>
        Dictionary<string, float> SelectOneCXData(string entity);

        //查询单个CX某日期的测斜数据，返回键值对<属性，数据>
        Dictionary<string, float> SelectOneDateData(string Entity, DateTime date);

        //根据Entity名，查询基础数据和地墙数据，
        List<Revit.Addin.RevitTooltip.App.ParameterData> SelectEntityData(string entity);

        //根据entity,修改备注
        bool ModifyEntityRemark(string entity, string entityremark);

        //读取各表数据传给SQLite数据库
        MySqlDataReader SelectTable(string tablename);


    }
}
