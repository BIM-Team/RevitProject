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
    public class ExcelTable
    {
        /// <summary>
        /// 存放ExcelId
        /// </summary>
        public int? Id { get; set; }
        
        /// <summary>
        /// 存放excel表的简写形式，如测斜CX
        /// </summary>
        public string Signal{  get;set;}
        /// <summary>
        /// 存放excel表的中文描述，且是最新的Excel表，即是最后一次导入的Excel
        /// </summary>
        public string CurrentFile{get;set;}
        /// <summary>
        /// 用于查询返回
        /// 累计阈值
        /// </summary>
        public string Total_hold { get; set; }
        /// <summary>
        /// 用于查询返回
        /// 相邻阈值
        /// </summary>
        public string Diff_hold { get; set; }
        /// <summary>
        /// 记录所有导入过的文件
        /// </summary>
        public string History { get; set; }
        /// <summary>
        /// 记录该Excel的测量数据的比较方式
        /// TotalThrehold的计算方式
        /// </summary>
        public string TotalOperator { get; set; }
        /// <summary>
        /// DiffThreshold的计算方式
        /// </summary>
        public string DiffOperator { get; set; }
    }
    /// <summary>
    /// 对应于InfoTable某个entity和其相关的所有Info数据
    /// </summary>
    public class InfoEntityData
    {
        
        /// <summary>
        /// 和EntityTable中的EntityName字段相对应
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// 直接用Dictionary类型来表示InfoTable中的键值对
        /// key:KeyTable中的keyName
        /// value:与之对应的value,对应于InfoTable中的value
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
        
        /// <summary>
        /// InfoTable的分组信息
        /// key:GroupName
        /// value:在GroupName中的所有KeyName
        ///该类型在插入到数据库时不使用，只用于查询某Entity相关数据时使用
        ///该分组信息作为查询返回时，总是有一个未定义组，用于存放未进行分组的所以KeyName
        ///这样分组信息中的KeyName应该和Data中的KeyName总量是一样的。
        /// </summary>
        public Dictionary<string, List<string>> GroupMsg { get; set; }
        /// <summary>
        /// 存放实体备注
        /// 目前仅用于查询
        /// </summary>
        public string Remark { get; set; }

    }
    /// <summary>
    /// 对应于某个测点的所有测量数据，时间序列数据
    /// </summary>
    public class DrawData
    {
        /// <summary>
        /// 用于唯一标识
        /// </summary>
        public String UniId { set; get; }
        public String EntityName { set; get; }

        DateTime date;
        /// <summary>
        /// 对应于DrawTable中的日期
        /// </summary>
        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }
        float maxValue;
        /// <summary>
        /// 对应于DrawTable中的最大值
        /// </summary>
        public float MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }
        float midValue;
        /// <summary>
        /// 对应于DrawTable中的中位数
        /// </summary>
        public float MidValue
        {
            get { return this.midValue; }
            set { this.midValue = value; }
        }
        float minValue;
        /// <summary>
        /// 对应于DrawTable中的最小值
        /// </summary>
        public float MinValue
        {
            get { return this.minValue; }
            set { this.minValue = value; }
        }
        string detail;
        /// <summary>
        /// 对应于DrawTable中的Detail字段
        /// </summary>
        public string Detail
        {
            get { return this.detail; }
            set { this.detail = value; }
        }
        
    }
    /// <summary>
    /// 对应于DrawTable中的某个entity相关的数据
    /// </summary>
    public class DrawEntityData
    {
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
        public List<DrawData> Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
        /// <summary>
        /// 用于查询返回
        /// 累计阈值
        /// </summary>
        public string Total_hold { get; set; }
        /// <summary>
        /// 用于查询返回
        /// 相邻阈值
        /// </summary>
        public string Diff_hold { get; set; }
        /// <summary>
        /// 记录该Excel的测量数据的比较方式
        /// TotalThrehold的计算方式
        /// </summary>
        public string TotalOperator { get; set; }
        /// <summary>
        /// DiffThreshold的计算方式
        /// </summary>
        public string DiffOperator { get; set; }

    }
    /// <summary>
    /// 一批待处理数据，一般为一个Excel中的数据，或者几张sheet的数据。
    /// </summary>
    public class SheetInfo
    {
        
        public ExcelTable ExcelTableData{get;set;}
        /// <summary>
        /// 用于标识数据是InfoTable数据还是DrawTable数据
        /// True:InfoData
        /// Fale:DrawData
        /// </summary>
        public bool Tag{get;set;}
        /// <summary>
        /// 用于插入KeyTable中的数据，保存所有的列名，即表头
        /// </summary>
        public List<string> KeyNames { get; set; }
        
        /// <summary>
        /// 用于插入EntityTable中的所用EntityName
        /// </summary>
        public List<string> EntityNames { get; set; }
        /// <summary>
        /// 用于插入InfoTalbe中的数据类型
        /// </summary>
        public List<InfoEntityData> InfoRows { get; set; }
        /// <summary>
        /// 用于插入到DrawTable中的数据类型
        /// </summary>
        public List<DrawEntityData> DrawRows { get; set; }
    }
    public class ParameterData
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ParameterData(string name, string value)
        {
            Name = name;
            Value = value;
        }
        //public override bool Equals(object obj)
        //{
        //    ParameterData o = (ParameterData)obj;
        //    return this.Value.Equals(o.Value) && this.Name.Equals(o.Name);
        //}
        //public override int GetHashCode()
        //{
        //    return (this.Name + this.Value).GetHashCode();
        //}
    }
    public class Group {
        /// <summary>
        /// Group表的Id数据项，仅用于查询返回数据
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用于查询返回GropName
        /// </summary>
        public string GroupName { get; set; }

        public override bool Equals(object obj)
        {
            Group o = (Group)obj;

            return this.Id.Equals(o.Id) && this.GroupName.Equals(o.GroupName);
        }
        public override int GetHashCode()
        {
            return (this.GroupName+this.Id).GetHashCode();
        }

    }
    /// <summary>
    /// 对应于Keytable
    /// 仅用于查询时使用
    /// </summary>
    public class CKeyName {
        
        /// <summary>
        /// 仅用于初始化DataGrid的CheckBox列
        /// </summary>
        public bool IsCheck { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 和KeyTable中的keyName对应
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 在Excel表中的顺序
        /// </summary>
        public int Odr { get; set; }
        public override bool Equals(object obj)
        {
            CKeyName o = (CKeyName)obj;

            return this.KeyName.Equals(o.KeyName) && this.Odr == o.Odr;
        }
        public override int GetHashCode()
        {
            return (this.KeyName + this.Odr).GetHashCode();
        }
    }
    /// <summary>
    /// 仅用于查询，查询所有的EntityName
    /// </summary>
    public class CEntityName {
        /// <summary>
        /// 存放EntityId
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 存放EntityName
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// 标记其错误类型
        /// err1,err2,err3,noErr
        /// </summary>
        public string ErrMsg { get; set; }

    }
}