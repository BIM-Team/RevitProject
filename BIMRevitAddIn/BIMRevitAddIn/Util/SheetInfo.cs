using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMRevitAddIn.Util
{
    public enum ExcelType {//表格类型
        None,//没有赋值
        BaseExcel,//基础数据表
        WallExcel,//城墙数据表
        DataExcel//测量数据表

    }
    delegate SheetInfo GetSheetInfo(int sheetIndex);
    public class SheetInfo
    {

        //该sheet在excel中的索引
        private int sheetIndex = -1;
        public int SheetIndex {
            get {
                return sheetIndex;
            }
            set {
                this.sheetIndex = value;
            }
        }

        //存放表的类型，基础数据，城墙数据，或者测量数据
        private ExcelType tableType;
        public ExcelType TableType {
            get {
                return tableType;
            }
            set {
                this.tableType = value;
            }
        }

        //存放表头，也就是所有的属性名
        //基础数据或者城墙数据为列,测量数据为行
        private List<string> propertyNames;
        public List<string> Names {
            get {
                return propertyNames;
            }
        }

        //存放实体
        //基础数据或者城墙数据为行，测量数据为列
        private List<string> entityName;
        public List<string> EntityName {
            get {
                return entityName;
            }
        }
        //存放表格数据,格式<实体名，数据>
        //基础数据和城墙数据中，每个List只包含一个string[],且string[]的第一项为实体名，后续才是对应的数据
        //对于城墙数据而言，每个list中含有多个string[]，且string[]的第一项都为记录时间，后续才是测量的数值
        //string[]的长度对应于propertyNames长度+1
        private Dictionary<string, List<string[]>> data;
        public Dictionary<string, List<string[]>> Data {
            get {
                return data;
            }
            
        }
        //
        //初始化
        public SheetInfo() {
            this.data = new Dictionary<string, List<string[]>>();
            this.propertyNames = new List<string>();
            this.entityName = new List<string>();
        }
    }
}
