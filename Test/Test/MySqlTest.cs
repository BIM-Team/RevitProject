using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIMRevitAddIn.Util;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Test
{
    class MySqlTest
    {
        static void Main(string[] args)
        {
            //连接打开数据库
            MysqlUtil mysql = MysqlUtil.CreateInstance("127.0.0.1", "root", "hzj", "root");
           //打开连接
            mysql.OpenConnect();

            Dictionary<string, Dictionary<DateTime, float>> Data_CX = mysql.SelectOneCXData("CX1");
            OutputCXdata(Data_CX);

            string cx = "CX1";
            DateTime dt = Convert.ToDateTime("2015/7/17下午");
            Dictionary<DateTime, Dictionary<string, float>> Data_OneCX = mysql.SelectOneDateData(cx,dt);
            OutputOneCXdata(Data_OneCX);

 /* 
            System.Console.WriteLine();
            string base_entity = "26-29中板下侧墙";
            Dictionary<string, Dictionary<string, string>> Data_Base = mysql.SelectBaseData(base_entity);
            Outputdata(Data_Base);

            string wall_entity = "E6";
            Dictionary<string, Dictionary<string, string>> Data_Wall = mysql.SelectWallData(wall_entity);
            Outputdata(Data_Wall);

                   
            //从Excel表中读取数据
            ExcelReader reader = new ExcelReader();
            reader.FilePath_base = @"F:\慧之建资料\TO 卫老师\7-录入到插件内的格式\基础构件.xlsx";
            reader.FilePath_Wall = @"F:\慧之建资料\TO 卫老师\7-录入到插件内的格式\望园路站地墙.xlsx";
            reader.FilePath_Data = @"F:\慧之建资料\TO 卫老师\4-施工数据\施工数据\20150920\测斜汇总.xls";
            
           // reader.UseExcelType = ExcelType.BaseExcel;
           // reader.UseExcelType = ExcelType.WallExcel;
            //SheetInfo sheetInfo = reader.getSheetInfo_BaseOrWall(0);
            reader.UseExcelType = ExcelType.DataExcel;
             SheetInfo sheetInfo = reader.getSheetInfo_Data(0);
           // List<SheetInfo> sheetInfos = reader.getSheetInfo_Range(0, reader.getSheetCount());
           // foreach (SheetInfo sheetInfo in sheetInfos)
           // {
                System.Console.WriteLine("###########################\n索引：" + sheetInfo.SheetIndex);
                Out(sheetInfo);
                
                System.Console.WriteLine("####################将数据插入到数据库:");
                
                mysql.InsertSheetInfo(sheetInfo);
                System.Console.WriteLine("一次插入完成");
           // }
            reader.Close();
            //System.Console.Write(DateTime.FromOADate(Double.Parse("45897")).ToString("yyyy-MM-dd"));
            //System.Console.ReadKey();
*/
            mysql.Close();  //关闭数据库连接
            mysql.Dispose();
            System.Console.WriteLine(DateTime.Parse("2016/2/23下午"));
            System.Console.WriteLine(DateTime.Parse("2016/2/23下午"));
            Console.ReadKey();

        }
        public static void Out(SheetInfo sheetInfo)
        {
            System.Console.WriteLine("输出sheet的信息");
            StringBuilder str = new StringBuilder();
            if (sheetInfo == null)
            {
                return;
            }
            if (sheetInfo.TableType == ExcelType.BaseExcel || sheetInfo.TableType == ExcelType.WallExcel)
            {

                str.Append(sheetInfo.TableType);
            }
            else if (sheetInfo.TableType == ExcelType.DataExcel)
            {
                str.Append(sheetInfo.EntityName[0]);
            }

            foreach (string s in sheetInfo.Names)
            {
                str.Append("  ").Append(s);
            }
            str.Append("\n");

            foreach (string key in sheetInfo.Data.Keys)
            {
                foreach (string[] sts in sheetInfo.Data[key])
                {
                    foreach (string s in sts)
                    {
                        str.Append(" ").Append(s);
                    }
                    str.Append("\n");
                }
            }

            System.Console.Write(str.ToString());
            System.Console.ReadKey();

        }

        //更新数据库数据
        public static void UpdateMysql(SheetInfo sheetInfo)
        {

        }

        //获取数据库数据
        public static void OutputCXdata(Dictionary<string, Dictionary<DateTime, float>> Data_CX)
        {
            System.Console.WriteLine("输出查询的CX信息");
            StringBuilder str = new StringBuilder();
            foreach (string key in Data_CX.Keys)
            {
                str.Append(key + ": ").Append("\n");
                foreach (DateTime d in Data_CX[key].Keys)
                {
                    str.Append(d).Append(" ").Append(Data_CX[key][d]).Append(" ").Append("\n");
                }
                str.Append("\n");
            }
            System.Console.Write(str.ToString());
            System.Console.ReadKey();
        }
        public static void OutputOneCXdata(Dictionary<DateTime, Dictionary<string, float>> Data_CX)
        {
            System.Console.WriteLine("输出查询的CX信息");
            StringBuilder str = new StringBuilder();
            foreach (DateTime key in Data_CX.Keys)
            {
                str.Append(key + ": ").Append("\n");
                foreach (string d in Data_CX[key].Keys)
                {
                    str.Append(d).Append(" ").Append(Data_CX[key][d]).Append(" ").Append("\n");
                }
                str.Append("\n");
            }
            System.Console.Write(str.ToString());
            System.Console.ReadKey();
        }

        //获取数据库数据
        public static void Outputdata(Dictionary<string, Dictionary<string, string>> Data)
        {
            System.Console.WriteLine("输出查询的实体信息：");
            StringBuilder str = new StringBuilder();
            foreach (string key in Data.Keys)
            {
                str.Append(key + ": ").Append("\n");
                foreach (string d in Data[key].Keys)
                {
                    str.Append(d).Append(" ").Append(Data[key][d]).Append(" ").Append("\n");
                }
                str.Append("\n");
            }
            System.Console.Write(str.ToString());
            System.Console.ReadKey();
        }
    }
}
