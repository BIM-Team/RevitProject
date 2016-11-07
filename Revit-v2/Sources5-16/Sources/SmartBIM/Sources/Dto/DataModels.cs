using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revit.Addin.RevitTooltip.Dto
{
    /// <summary>
    /// 对应于ExcelTable表
    /// 根据excel的文件名解析得来
    /// </summary>
    public class ExcelTable {
        string signal;
        /// <summary>
        /// 存放excel表的简写形式，如测斜CX
        /// </summary>
        public string Signal {
            get { return signal; }
            set { this.signal = value; }
        }
        string tableDesc;
        /// <summary>
        /// 存放excel表的中文描述
        /// </summary>
        public string TableDesc {
            get { return this.tableDesc;}
            set { this.tableDesc = value; }
        }
    }
    /// <summary>
    /// 对应于InfoTable某个entity和其相关的所有Info数据
    /// </summary>
    public class InfoEntityData {
        string entityName;
        /// <summary>
        /// 和EntityTable中的EntityName字段相对应
        /// </summary>
        public string EntityName {
            get { return this.entityName; }
            set { this.entityName = value; }
        }
        Dictionary<string, string> data;
        /// <summary>
        /// 直接用Dictionary类型来表示InfoTable中的键值对
        /// key:KeyTable中的keyName
        /// value:与之对应的value,对应于InfoTable中的value
        /// </summary>
        public Dictionary<string, string> Data {
        get { return this.data; }
        set { this.data = value; }
        }
		Dictionary<string,List<string>> groupMsg;
        /// <summary>
        /// InfoTable的分组信息
        /// key:GroupName
        /// value:在GroupName中的所有KeyName
        ///该类型在插入到数据库时不使用，只用于查询某Entity相关数据时使用
        ///该分组信息作为查询返回时，总是有一个未定义组，用于存放未进行分组的所以KeyName
        ///这样分组信息中的KeyName应该和Data中的KeyName总量是一样的。
        /// </summary>
        public Dictionary<string,List<string>> GroupMsg{
		get { return this.groupMsg; }
        set { this.groupMsg = value; }
		}
        
    }
    /// <summary>
    /// 对应于某个测点的所有测量数据，时间序列数据
    /// </summary>
    public class DrawData {
        DateTime date;
        /// <summary>
        /// 对应于DrawTable中的日期
        /// </summary>
        public DateTime Date {
        get { return this.date; }
        set { this.date = value; }
        }
        double maxValue;
        /// <summary>
        /// 对应于DrawTable中的最大值
        /// </summary>
        public double MaxValue {
        get { return this.maxValue; }
        set { this.maxValue = value; }
        }
        double midValue;
        /// <summary>
        /// 对应于DrawTable中的中位数
        /// </summary>
        public double MidValue {
        get { return this.midValue; }
        set { this.midValue = value; }    
        }
        double minValue;
        /// <summary>
        /// 对应于DrawTable中的最小值
        /// </summary>
        public double MinValue {
        get { return this.minValue; }
        set { this.minValue = value; }
        }
        string detail;
        /// <summary>
        /// 对应于DrawTable中的Detail字段
        /// </summary>
        public string Detail {
        get { return this.detail; }
        set { this.detail = value; }
        }
    }
    /// <summary>
    /// 对应于DrawTable中的某个entity相关的数据
    /// </summary>
    public class DrawEntityData {
        string entityName;
        /// <summary>
        /// 对应于entityTable中的EntityName
        /// </summary>
        public string EntityName
        {
            get { return this.entityName; }
            set { this.entityName = value; }
        }
        List<DrawData> data;
        /// <summary>
        /// 和EntityName相对应的测量数据，时间序列数据。
        /// </summary>
        public  List<DrawData> Data {
            get { return this.data; }
            set { this.data = value; }
        }
    }
    /// <summary>
    /// 一批待处理数据，一般为一个Excel中的数据，或者几张sheet的数据。
    /// </summary>
    public class SheetInfo
    {
        ExcelTable excelTableData;
        /// <summary>
        /// 用于和ExcelTable交互的数据类型
        /// </summary>
        public ExcelTable ExcelTableData {
        get { return this.excelTableData; }
        set { this.excelTableData = value; }
        }
        bool tag;
        /// <summary>
        /// 用于标识数据是InfoTable数据还是DrawTable数据
        /// True:InfoData
        /// Fale:DrawData
        /// </summary>
        public bool Tag {
        get { return this.tag; }
        set { this.tag = value; }
        }
        List<string> keyNames;
        /// <summary>
        /// 用于插入KeyTable中的数据，保存所有的列名，即表头
        /// </summary>
        public List<string> KeyNames {
            get { return this.keyNames; }
            set { this.keyNames = value; }
        }
        List<string> entityNames;
        /// <summary>
        /// 用于插入EntityTable中的所用EntityName
        /// </summary>
        public List<string> EntityNames {
        get { return this.entityNames; }
        set { this.entityNames = value; }
        }
        List<InfoEntityData> infoRows;
        /// <summary>
        /// 用于插入InfoTalbe中的数据类型
        /// </summary>
        public List<InfoEntityData> InfoRows {
        get { return this.infoRows; }
        set { this.infoRows = value; }
        }
        List<DrawEntityData> drawRows;
        /// <summary>
        /// 用于插入到DrawTable中的数据类型
        /// </summary>
        public List<DrawEntityData> DrawRows {
        get { return this.drawRows; }
        set { this.drawRows = value; } }
    }
}
