//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BIMRevitAddIn.Util;
//using BIMRevitAddIn.UI;
//using System.Windows.Forms;

//namespace Test
//{
//    class ExcelReaderTest
//    {   [STAThread]
//        static void Main(string[] args)
//        {

//            ExcelReader reader = new ExcelReader();
//            //reader.FilePath_base = @"D:\TO 卫老师\7-录入到插件内的格式\基础构件.xlsx";
//            //reader.FilePath_Wall = @"D:\TO 卫老师\7-录入到插件内的格式\望园路站地墙.xlsx";
//            //reader.FilePath_Data = @"D:\TO 卫老师\4-施工数据\施工数据\20150920\测斜汇总.xls";
//            //reader.UseExcelType = ExcelType.BaseExcel;
//            reader.UseExcelType = ExcelType.WallExcel;
//            //SheetInfo sheetInfo = reader.getSheetInfo_BaseOrWall(0);
//            FileImportDialog dialog = new FileImportDialog();
//            if (dialog.ShowDialog() == DialogResult.OK)
//            {
//                reader.FilePath_base = dialog.FilePathBase;
//                reader.FilePath_Wall = dialog.FilePathWall;
//                reader.FilePath_Data = dialog.FilePathData;
//            }
//            else {
//                System.Console.WriteLine("你还没有选择路径,程序已停止");
//                return;
//            }

//            //reader.UseExcelType = ExcelType.DataExcel;
//            // SheetInfo sheetInfo = reader.getSheetInfo_Data(0);
//            List<SheetInfo> sheetInfos = reader.getSheetInfo_Range(0,reader.getSheetCount());
//            foreach(SheetInfo sheetInfo in sheetInfos)
//            {
//                System.Console.WriteLine("###########################\n索引："+sheetInfo.SheetIndex);
//            Out(sheetInfo);
//            }
//            reader.Close();
//            //System.Console.Write(DateTime.FromOADate(Double.Parse("45897")).ToString("yyyy-MM-dd"));
//            //System.Console.ReadKey();
//        }
//        public static void Out(SheetInfo sheetInfo)
//        {
//            System.Console.WriteLine("输出sheet的信息");
//            StringBuilder str=new StringBuilder();
//            if (sheetInfo == null) {
//                return;
//            }
//            if (sheetInfo.TableType == ExcelType.BaseExcel || sheetInfo.TableType == ExcelType.WallExcel)
//            {

//                str.Append(sheetInfo.TableType);
//            }
//            else if (sheetInfo.TableType == ExcelType.DataExcel) {
//                str.Append(sheetInfo.EntityName[0]);
//            }

//                foreach (string s in sheetInfo.Names)
//                {
//                    str.Append("  ").Append(s);
//                }
//                str.Append("\n");

//                foreach (string key in sheetInfo.Data.Keys)
//                {
//                    foreach (string[] sts in sheetInfo.Data[key])
//                    {
//                        foreach (string s in sts)
//                        {
//                            str.Append(" ").Append(s);
//                        }
//                        str.Append("\n");
//                    }
//                }
            


            
//            System.Console.Write(str.ToString());
//            System.Console.ReadKey();

//        }
//}
//}
