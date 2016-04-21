﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BIMRevitAddIn.Util
{
    public class MysqlUtil: IDisposable
    {
        //单例模式
        private static MysqlUtil mysqlUtil;
        //server地址
        private String server;
        //数据库
        private String database;
        //用户名
        private String user;
        //密码
        private String password;
        //端口
        private int port = 3306;
        //MySql编码
        private String charset = "utf8";
        //连接
        private MySqlConnection conn;
        //是否连接的标志符
        private bool isOpen = false;
        //事务
        private MySqlTransaction myTran;
        //
        //连接信息，用于属性面板绑定数据源
        private string serverPath;
        public string ServerPath
        {
            get
            {
                return this.serverPath;
            }
        }
        //单例模式
        public static MysqlUtil CreateInstance(String server, String user, String database, String password) {
            if (null == mysqlUtil) {
                mysqlUtil = new MysqlUtil(server, user, database, password);
            }
            return mysqlUtil;
        }
        //初始化
        private MysqlUtil(String server, String user, String database, String password) {
            this.server = server;
            this.user = user;
            this.database = database;
            this.password = password;
            this. //指明要连接的数据库地址，用户名，数据库名，端口，密码
            serverPath = "server=" + this.server + ";user=" + this.user + ";database=" + this.database + ";port=" + this.port + ";password=" + this.password + ";charset=" + this.charset;

        }
       

        //建立mysql数据库连接
        public void OpenConnect()
        {
            if (null == conn) {
            this.conn = new MySqlConnection(serverPath);  //实例化连接
            }
            this.conn.Open();
            this.isOpen = true;       
        }
        //关闭数据库
        public void Close() {
            conn.Close();
            isOpen = false;
        }
        //销毁当前对象
        public void Dispose()
        {
            conn.Close();
            conn.Dispose();
            GC.SuppressFinalize(this);
            this.isOpen = false;
        }

        //插入一个SheetInfo，只插入以前表中不存在的数据
        public int InsertSheetInfo(SheetInfo sheetInfo)
        {
            //判断数据库是否打开
            if (!this.isOpen)
            {
                OpenConnect();
            }
            //事务开始                       
           myTran = conn.BeginTransaction();
            try
            {             
                if (null==sheetInfo)
                {
                    return 0;
                }
                //插入数据前，将所有表中ID自增字段设置为从当前表中最大ID开始插入
                TableIDUpdate();
                if (sheetInfo.TableType == ExcelType.BaseExcel)
                {
                    InsertSheetInfoBaseExcel(sheetInfo);
                }
                else if (sheetInfo.TableType == ExcelType.WallExcel)
                {
                    InsertSheetInfoWallExcel(sheetInfo);
                }
                else if (sheetInfo.TableType == ExcelType.DataExcel)
                {
                    InsertSheetInfoDataExcel(sheetInfo);                    
                }

                myTran.Commit();    //事务提交
                return 1;
            }
            catch (Exception e)
            {
                myTran.Rollback();    // 事务回滚
                throw new Exception("事务操作出错，系统信息：" + e.Message);
            }
            
        }

        //插入基础构建表，只插入以前表中不存在的数据
        private void InsertSheetInfoBaseExcel(SheetInfo sheetInfo)
        {
            //插入到TypeTable表,并返回ID
            int IdType = InsertIntoTypeTable(sheetInfo);

            //插入到FrameTable表，并返回当前表的键值对<属性名，ID>
            Dictionary<string, int> CurrentTableColumns = InsertIntoFrameType(sheetInfo.Names, IdType);

            //插入到EntityTable表，并返回新插入的实体数据的键值对<实体名,ID>
            Dictionary<string, int> UpdateEntities = InsertIntoEntityTable(sheetInfo.EntityName, IdType);           

            //插入到BaseComponentTable表
            InsertIntoBaseTable(sheetInfo, CurrentTableColumns, UpdateEntities);
        }
        //插入望园路站地墙表，只插入以前表中不存在的数据
        private void InsertSheetInfoWallExcel(SheetInfo sheetInfo)
        {
            int IdType = InsertIntoTypeTable(sheetInfo);
           
            Dictionary<string, int> CurrentTableColumns = InsertIntoFrameType(sheetInfo.Names, IdType);

            Dictionary<string, int> UpdateEntities = InsertIntoEntityTable(sheetInfo.EntityName, IdType);

            InsertIntoWallTable(sheetInfo, CurrentTableColumns, UpdateEntities);
        }
        //插入测斜汇总表，只插入以前表中不存在的数据
        private void InsertSheetInfoDataExcel(SheetInfo sheetInfo)
        {            
            int IdType = InsertIntoTypeTable(sheetInfo);

            Dictionary<string, int> CurrentTableColumns = InsertIntoFrameType(sheetInfo.Names, IdType);

            Dictionary<string, int> UpdateEntities = InsertIntoEntityTable(sheetInfo.EntityName, IdType);

            InsertIntoInclinationTable(sheetInfo, CurrentTableColumns, UpdateEntities,IdType);
        }

        //将所有表中ID自增字段设置为从当前表中最大ID开始插入数据
        private void TableIDUpdate()
        {
            string sql = "alter table TypeTable auto_increment =1;"
                         +"alter table EntityTable auto_increment =1;"
                         +"alter table FrameTable auto_increment =1;"
                         +"alter table BaseComponentTable auto_increment =1;"
                         +"alter table wylWallTable auto_increment =1;"
                         +"alter table InclinationTable auto_increment =1";
            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象，其中myTran为事务
            try
            {
                mycom.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        //先查看TypeTable表中是否存在该tableType，有则直接返回ID，没有则插入，并返回ID
        private int InsertIntoTypeTable(SheetInfo sheetInfo, String tableRemark="")
        {
            String TYPENAME;
            if (sheetInfo.TableType == ExcelType.BaseExcel)
                TYPENAME = "基础构件";
            else if (sheetInfo.TableType == ExcelType.WallExcel)
                TYPENAME = "望园路站地墙";
            else if (sheetInfo.TableType == ExcelType.DataExcel)
                TYPENAME = "测斜汇总";
            else
            {
                TYPENAME = "";
                throw new Exception("表的名称有误！");
            }
            int id = getIdType(TYPENAME);           
            if (id == 0)
            {
                String sql = GetInsertIntoTypeTableSql(TYPENAME, tableRemark);
                InsertOne(sql);
                id = getIdType(TYPENAME);                
            }          
            return id;
        }

        //插入属性，FrameTable表，并返回当前表的属性及对应ID，<属性，ID>
        private Dictionary<string,int> InsertIntoFrameType(List<string> Names, int IdType)
        {
            Dictionary<string, int> CurrentTableColumns = new Dictionary<string,int>();
            Dictionary<string, int> PeriousColumns = SelectPeriousColumnNames(IdType);

            foreach (string name in Names){
                
                if (PeriousColumns.Keys.Contains(name)){
                    CurrentTableColumns.Add(name, PeriousColumns[name]);
                }                   
                else{
                //如果没有在原表中匹配到，则插入当中
                String sql = GetInsertIntoFrameTableSql(IdType, name);
                InsertOne(sql);
                int id = getIdFrame(IdType,name);
                CurrentTableColumns.Add(name, id);
                }                            
            }
            return CurrentTableColumns;
        }
        //插入实体，EntityTable表
        private Dictionary<string, int> InsertIntoEntityTable(List<string> EntityName, int IdType)
        {
            Dictionary<string, int> UpdateEntities = new Dictionary<string, int>();
            Dictionary<string, int> PeriousEntities = SelectPeriousEntities(IdType);
            foreach (string e in EntityName)
            {
                if (!PeriousEntities.Keys.Contains(e))
                {
                    //如果没有在原表中匹配到，则插入当中
                    String sql = GetInsertIntoEntityTableSql(IdType, e);
                    InsertOne(sql);
                    int id = getIdEntity(IdType,e);
                    UpdateEntities.Add(e, id); 
                }                         
            }
            return UpdateEntities;
        }
        //插入数据项，三个数据表BaseComponentTable、wylWallTable、InclinationTable
        private void InsertIntoBaseTable(SheetInfo sheetInfo, Dictionary<string, int> CurrentTableColumns, Dictionary<string, int> UpdateEntities)
        {
            int IdFrame;
            int IdEntity;
            String InsertIntoBaseCTableSql;
            int count = sheetInfo.Names.Count;
           
            foreach (String key in sheetInfo.Data.Keys)
            {
                if (UpdateEntities.Keys.Contains(key))
                {
                    IdEntity = UpdateEntities[key];
                    foreach (String[] sts in sheetInfo.Data[key])
                    {
                        for (int i = 0; i < count; i++)
                        {
                            IdFrame = CurrentTableColumns[sheetInfo.Names.ElementAt(i)];
                            InsertIntoBaseCTableSql = GetInsertIntoBaseCTableSql(IdFrame, IdEntity, sts[i + 1]);
                            InsertOne(InsertIntoBaseCTableSql);
                        }
                    }
                }          
            }
        }
        private void InsertIntoWallTable(SheetInfo sheetInfo, Dictionary<string, int> CurrentTableColumns, Dictionary<string, int> UpdateEntities)
        {
            int IdFrame;
            int IdEntity;
            String InsertIntoWallTableSql;
            int count = sheetInfo.Names.Count;

            foreach (String key in sheetInfo.Data.Keys)
            {
                if (UpdateEntities.Keys.Contains(key))
                {
                    IdEntity = UpdateEntities[key];
                    foreach (String[] sts in sheetInfo.Data[key])
                    {
                        for (int i = 0; i < count; i++)
                        {
                            IdFrame = CurrentTableColumns[sheetInfo.Names.ElementAt(i)];
                            InsertIntoWallTableSql = GetInsertIntoWallTableSql(IdFrame, IdEntity, sts[i + 1]);
                            InsertOne(InsertIntoWallTableSql);
                        }
                    }
                }  
            }
        }
        private void InsertIntoInclinationTable(SheetInfo sheetInfo, Dictionary<string, int> CurrentTableColumns, Dictionary<string, int> UpdateEntities, int IdType)
        {
            int IdFrame;
            int IdEntity;
            String InsertIntoInclinationTableSql;
            int count = sheetInfo.Names.Count;

            foreach (String key in sheetInfo.Data.Keys)
            {
                //对于测斜数据，一个实体数据是一个表，实体表如果有更新，则在InsertIntoInclinationTable插入该实体对应的一个表的数据
                if (UpdateEntities.Keys.Contains(key))
                {
                    IdEntity = UpdateEntities[key];
                    foreach (String[] sts in sheetInfo.Data[key])
                    {
                        for (int i = 0; i < count; i++)
                        {
                            IdFrame = CurrentTableColumns[sheetInfo.Names.ElementAt(i)];
                            InsertIntoInclinationTableSql = GetInsertIntoInclinationTableSql(IdFrame, IdEntity, DateTime.Parse(sts[0]), sts[i + 1]);
                            InsertOne(InsertIntoInclinationTableSql);
                        }
                    }
                }
                else  //实体表如果没有更新，要检查每个实体表中，时间是否增加
                {
                    IdEntity = getIdEntity(IdType, key);
                    List<DateTime> PeriousDateTimes = GetInclinationTableDateTimes(IdEntity);
                    
                    foreach (String[] sts in sheetInfo.Data[key])
                    {
                        if (!PeriousDateTimes.Contains(DateTime.Parse(sts[0])))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                IdFrame = CurrentTableColumns[sheetInfo.Names.ElementAt(i)];
                                InsertIntoInclinationTableSql = GetInsertIntoInclinationTableSql(IdFrame, IdEntity, DateTime.Parse(sts[0]), sts[i + 1]);
                                InsertOne(InsertIntoInclinationTableSql);
                            }
                        }     
                    }              
                }
            }
        }

        //获取FrameTable表的COLUMN和ID
        private Dictionary<string, int> SelectPeriousColumnNames(int IdType)
        {
            Dictionary<string, int> PeriousColumnNames = new Dictionary<string, int>();
            String sql = "select COLUMNNAME, ID from FrameTable where ID_TYPE = " + IdType;

            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        PeriousColumnNames.Add(reader.GetString(0), reader.GetInt32(1));                      
                    }
                }
                return PeriousColumnNames;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }
        //获取EntityTable表的ENTITY和ID
        private Dictionary<string, int> SelectPeriousEntities(int IdType)
        {
            Dictionary<string, int> PeriousEntities = new Dictionary<string, int>();
            String sql = "select ENTITY, ID from EntityTable where ID_TYPE = " + IdType;

            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        PeriousEntities.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
                return PeriousEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }
        //从InclinationTable表中，获取该实体数据中，当前存储的DATETIME项
        private List<DateTime> GetInclinationTableDateTimes(int IdEntity)
        {
            List<DateTime> PeriousDateTimes = new List<DateTime>();            
            String sql = "select DATE from InclinationTable where ID_ENTITY = " + IdEntity;

            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        PeriousDateTimes.Add(reader.GetDateTime(0));
                    }
                }
                return PeriousDateTimes;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }
        
        //执行单个插入语句，返回插入结果
        private void InsertOne(String sql)
        {
            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象，其中myTran为事务
             
            try
            {
                mycom.ExecuteNonQuery();           
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //获取向表TypeTable中插入数据的sql查询语句
        private String GetInsertIntoTypeTableSql(String tableType, String tableRemark="")
        {
            String sql = String.Format("insert into TypeTable (TYPENAME,TABLEREMARK) values ('{0}', '{1}')", tableType, tableRemark);
            return sql;
        }
        //获取向表FrameTable中插入数据的sql查询语句
        private String GetInsertIntoFrameTableSql(int idType, String ColumnName)
        {
            String sql = String.Format("insert into FrameTable ( ID_TYPE, COLUMNNAME ) values ('{0}','{1}')", idType, ColumnName);
            return sql;
        }
        //获取向表EntityTable中插入数据的sql查询语句
        private String GetInsertIntoEntityTableSql(int idType, String Entity, String EntityColumn = "测点编号", String EntityRemark = "")
        {
            String sql = String.Format("insert into EntityTable ( ID_TYPE, ENTITY, ENTITYCOLUMN, ENTITYREMARK ) values ('{0}','{1}','{2}','{3}')", idType, Entity, EntityColumn, EntityRemark);
            return sql;
        }
        //获取向表BaseComponentTable中插入数据的sql查询语句
        private String GetInsertIntoBaseCTableSql(int idFrame, int idEntity, String value)
        {
            String sql = String.Format("insert into BaseComponentTable ( ID_FRAME, ID_ENTITY, VALUE ) values ('{0}','{1}','{2}')", idFrame, idEntity, value);
            return sql;
        }
        //获取向表wylWallTable中插入数据的sql查询语句
        private String GetInsertIntoWallTableSql(int idFrame, int idEntity, String value)
        {
            String sql = String.Format("insert into wylWallTable ( ID_FRAME, ID_ENTITY, VALUE ) values ('{0}','{1}','{2}')", idFrame, idEntity, value);
            return sql;
        }
        //获取向表InclinationTable中插入数据的sql查询语句
        private String GetInsertIntoInclinationTableSql(int idFrame, int idEntity, DateTime date, String value)
        {
            String sql = String.Format("insert into InclinationTable ( ID_FRAME, ID_ENTITY, DATE, VALUE ) values ('{0}','{1}','{2}','{3}')", idFrame, idEntity, date, value);
            return sql;
        }

        //从表TypeTable中查询各种表对应的ID
        private int getIdType(String type)
        {
            string sql =  String.Format("select ID from TypeTable where TYPENAME = '{0}'", type);
            int id = getID(sql);
            return id;
        }
        //从表FrameTable中查询各表表头（即COLUMNNAME）对应的ID
        private int getIdFrame(int idType, String ColumnName)
        {
            string sql = String.Format("select ID from FrameTable where ID_TYPE = {0} and COLUMNNAME = '{1}'",idType, ColumnName);
            int id = getID(sql);
            return id;
        }
        //从表EntityTable中查询各表实体（即ENTITY）对应的ID
        private int getIdEntity(int idType, String Entity)
        {
            string sql = String.Format("select ID from EntityTable where ID_TYPE = {0} and ENTITY = '{1}'", idType, Entity);
            int id = getID(sql);            
            return id;
        }
        private int getID(String sql)
        {
            MySqlCommand mycom = new MySqlCommand(sql, this.conn, this.myTran);  //建立执行命令语句对象
            int id = 0;
            try
            {
                id = Convert.ToInt32(mycom.ExecuteScalar());   //查询返回一个值的时候，用ExecuteScalar()更节约资源，快捷
            }
            catch { throw; }
            return id;
        }

/*        
                //查询数据，返回查询结果
                public void select(String cx, String type)
                {
                    //查询单个CX测斜
                    String sql_singlecx = "select ENTITY, DATE, MAX(VALUE), ENTITYREMARK from InclinationTable it,EntityTable et"+
                                          "where it.ID_ENTITY=et.ID and et.ENTITY=" + cx +
                                          "group by DATE" +
                                          "order by DATE ASC";

                   

                    //查询“基础构件”表中所有数据
                    String sql_bct = "select ENTITY,  from BaseComponentTable bct,FrameTable ft,EntityTable et";

                }

                //数据转存功能***************
                public void saveas()
                {

                }
         */
        
        //*****注意，sql语句多行写的话，开始要注意留空格*******
        //查询基础数据
        public Dictionary<string, Dictionary<string, string>> SelectBaseData(String Entity)
        {
            Dictionary<string, Dictionary<string, string>> Data_Base = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            String entity = "";
            
            String sql =String.Format("select et.ENTITY, ft.COLUMNNAME, bt.VALUE from TypeTable tt, EntityTable et, FrameTable ft, BaseComponentTable bt" 
                                      +" where tt.ID = et.ID_TYPE and tt.ID = ft.ID_TYPE and bt.ID_ENTITY = et.ID and bt.ID_FRAME = ft.ID and et.ENTITY = '{0}' order by et.ENTITY,ft.COLUMNNAME", Entity);
            
            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        data.Add(reader.GetString(1), reader.GetString(2));
                    }
                }
                Data_Base.Add(entity, data);
                return Data_Base;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }

        }
        //查询望园路地墙数据
        public Dictionary<string, Dictionary<string, string>> SelectWallData(string Entity)
        {
            Dictionary<string, Dictionary<string, string>> Data_Wall = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            String entity = "";

            String sql = String.Format("select et.ENTITY, ft.COLUMNNAME, wt.VALUE from TypeTable tt, EntityTable et, FrameTable ft, wylWallTable wt "
                         + " where tt.ID = et.ID_TYPE and tt.ID = ft.ID_TYPE and wt.ID_ENTITY = et.ID and wt.ID_FRAME = ft.ID and et.ENTITY = '{0}'"
                         + " order by et.ENTITY,ft.COLUMNNAME", Entity);

            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        data.Add(reader.GetString(1), reader.GetString(2));     
                    }                           
                }
                Data_Wall.Add(entity, data);
                return Data_Wall;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }

        }

        //查询单个CX某日期的测斜数据，返回键值对<日期，<属性，数据>>
        public Dictionary<DateTime, Dictionary<string, float>> SelectOneDateData(string Entity, DateTime date)
        {
            Dictionary<DateTime, Dictionary<string, float>> Data_CX = new Dictionary<DateTime, Dictionary<string, float>>();
            Dictionary<string, float> data = new Dictionary<string, float>();

            String sql = String.Format("select it.DATE,cast(ft.COLUMNNAME as DECIMAL(4,2)),cast(it.VALUE as DECIMAL(5,2)) from InclinationTable it,FrameTable ft,EntityTable et "
                          + " where ft.ID_TYPE=et.ID_TYPE and it.ID_ENTITY=et.ID and it.ID_FRAME=ft.ID and et.ENTITY= '{0}' and it.DATE = '{1}' ", Entity, date);

            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        data.Add(reader.GetString(1), reader.GetFloat(2));       
                    }
                }
                Data_CX.Add(date, data);
                return Data_CX;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }

        }

        //查询所有CX测斜汇总，返回键值对<实体名，<日期，数据>>
        public Dictionary<string, Dictionary<DateTime, float>> SelectOneCXData(string entity)
        {

            Dictionary<string, Dictionary<DateTime, float>> Data_CX = new Dictionary<string, Dictionary<DateTime, float>>();
            Dictionary<DateTime, float> data = new Dictionary<DateTime, float>();

            //用cast()函数将string类型转换为浮点数，在用MAX()函数
            String sql = String.Format("select et.ENTITY, it.DATE, MAX(cast(it.VALUE as DECIMAL(5,2))) from InclinationTable it,EntityTable et,TypeTable tt " 
                          + " where it.ID_ENTITY=et.ID and et.ID_TYPE=tt.ID and tt.TYPENAME= '测斜汇总' and et.ENTITY='{0}' "                       
                          + " group by ENTITY,DATE "
                          + " order by ENTITY,DATE " , entity);
            
            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {                                                                   
                         data.Add(reader.GetDateTime(1), reader.GetFloat(2));   
                          
                     }
                }
                Data_CX.Add(entity, data);
                return Data_CX;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
