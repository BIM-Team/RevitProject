using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Revit.Addin.RevitTooltip.Util
{
    public class MysqlUtil : IDisposable, IMysqlUtil, IWriter
    {
        //单例模式
        private static MysqlUtil mysqlUtil;
        //用户名
        // private static String user;
        //密码
        // private static String password;
        //settings
        private static RevitTooltip settings;
        //连接
        private MySqlConnection conn;
        //是否连接的标志符
        private bool isOpen = false;
        //事务
        private MySqlTransaction myTran;

        //是否打开事务
        private bool isBegin = false;

        //是否有备注
        private int hasEntityRemark = 0;

        //单例模式
        public static MysqlUtil CreateInstance()
        {
            if (null == mysqlUtil)
            {
                mysqlUtil = new MysqlUtil(App.settings);
            }
            else if (!MysqlUtil.settings.Equals(App.settings))
            {
                mysqlUtil.Dispose();
                MysqlUtil.settings = App.settings;
                mysqlUtil = new MysqlUtil(settings);
            }
            return mysqlUtil;
        }

        //初始化
        private MysqlUtil(RevitTooltip settings)
        {
            MysqlUtil.settings =(RevitTooltip) settings.Clone();

        }

        //建立mysql数据库连接
        public void OpenConnect()
        {
            if (null == conn)
            {
                 conn = new MySqlConnection("server=" + settings.DfServer + ";user=" + settings.DfUser + ";database=" + settings.DfDB + ";port=" + settings.DfPort + ";password=" + settings.DfPassword + ";charset=" + settings.DfCharset);  //实例化连接
                //conn = new MySqlConnection("server= 127.0.0.1 ;user= hzjuser; database= hzj ;port= 3306;password= hzj20160330;charset= utf8");  //实例化连接          
            }
            conn.Open();
            this.isOpen = true;
        }
        //关闭数据库
        public void Close()
        {
            conn.Close();
            isOpen = false;
        }
        //销毁当前对象
        public void Dispose()
        {
            if (conn != null) {

            conn.Close();
            conn.Dispose();
            }
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
            isBegin = true;
            try
            {
                if (null == sheetInfo)
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
                isBegin = false;
                hasEntityRemark = 0;
                return 1;
            }
            catch (Exception e)
            {
                myTran.Rollback();    // 事务回滚
                isBegin = false;
                hasEntityRemark = 0;
                //删除entitytable表中当前sheet的entity
                DeleteCurrentEntity(sheetInfo.EntityName, InsertIntoTypeTable(sheetInfo));

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

            InsertIntoInclinationTable(sheetInfo, CurrentTableColumns, UpdateEntities, IdType);
        }

        //将所有表中ID自增字段设置为从当前表中最大ID开始插入数据
        private void TableIDUpdate()
        {
            string sql = "alter table TypeTable auto_increment =1;"
                         + "alter table EntityTable auto_increment =1;"
                         + "alter table FrameTable auto_increment =1;"
                         + "alter table BaseComponentTable auto_increment =1;"
                         + "alter table wylWallTable auto_increment =1;"
                         + "alter table InclinationTable auto_increment =1";
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
        //删除所有表的数据
        public void DeleteAllData()
        {
            //关联表外键是级联删除
            string sql = "delete from typetable ";
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
        //当插入一个sheet数据回滚时，删除当前sheet表的entity
        private void DeleteCurrentEntity(List<string> EntityName, int IdType)
        {
            Dictionary<string, int> PeriousEntities = SelectEntities(IdType);
            string sql = "delete from entitytable where entity in (";
            int n = 0;
            foreach (string s in EntityName)
            {
                if (PeriousEntities.Keys.Contains(s))
                {
                    if (n != 0)
                        sql += ",";

                    sql += "'" + s + "'";
                    n++;
                }
            }
            if (n == 0)
                sql += "'')";
            else
                sql += ")";

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
        private int InsertIntoTypeTable(SheetInfo sheetInfo, String tableRemark = "")
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
        private Dictionary<string, int> InsertIntoFrameType(List<string> Names, int IdType)
        {
            Dictionary<string, int> PeriousColumns = SelectColumnNames(IdType);
            String sql = "";
            int n = 0;
            foreach (string name in Names)
            {
                if (name.Equals("备注"))
                {
                    hasEntityRemark = 1;
                    continue;
                }

                //如果没有在原表中匹配到，则添加到插入语句当中
                if (!PeriousColumns.Keys.Contains(name))
                {
                    if (n == 0)
                        sql = GetInsertIntoFrameTableSql(IdType, name);
                    else
                        sql = sql + ",('" + IdType + "','" + name + "')";
                    n++;
                }
            }
            InsertOne(sql);
            //获取更新后的<属性，ID>
            Dictionary<string, int> CurrentTableColumns = SelectColumnNames(IdType);
            return CurrentTableColumns;
        }
        //插入实体，EntityTable表
        private Dictionary<string, int> InsertIntoEntityTable(List<string> EntityName, int IdType)
        {
            Dictionary<string, int> UpdateEntities = new Dictionary<string, int>();
            List<string> UpdateEntityNames = new List<string>();
            Dictionary<string, int> PeriousEntities = SelectEntities(IdType);
            String sql = "";
            int n = 0;
            foreach (string e in EntityName)
            {
                //如果没有在原表中匹配到，则添加到插入语句当中
                if (!PeriousEntities.Keys.Contains(e))
                {
                    if (n == 0)
                        sql = GetInsertIntoEntityTableSql(IdType, e);
                    else
                        sql = sql + ",('" + IdType + "','" + e + "','" + "')";

                    UpdateEntityNames.Add(e);
                    n++;
                }
            }
            InsertOne(sql);

            //获取当前表所有的Entities
            foreach (string s in UpdateEntityNames)
            {
                int id = getIdEntity(IdType, s);
                UpdateEntities.Add(s, id);
            }

            return UpdateEntities;
        }
        //插入数据项，三个数据表BaseComponentTable、wylWallTable、InclinationTable
        private void InsertIntoBaseTable(SheetInfo sheetInfo, Dictionary<string, int> CurrentTableColumns, Dictionary<string, int> UpdateEntities)
        {
            int IdFrame;
            int IdEntity;
            int count = sheetInfo.Names.Count;
            List<string> SqlStringList = new List<string>();  // 用来存放多条SQL语句
            String sql = "";
            int n = 0;
            foreach (String key in sheetInfo.Data.Keys)
            {
                if (UpdateEntities.Keys.Contains(key))
                {
                    IdEntity = UpdateEntities[key];
                    foreach (String[] sts in sheetInfo.Data[key])
                    {
                        for (int i = 0; i < count - hasEntityRemark; i++)
                        {
                            IdFrame = CurrentTableColumns[sheetInfo.Names.ElementAt(i)];
                            if (n % 500 == 0)
                            {
                                if (n > 0)
                                    SqlStringList.Add(sql);
                                sql = GetInsertIntoBaseCTableSql(IdFrame, IdEntity, sts[i + 1]);
                            }
                            else
                                sql = sql + ",('" + IdFrame + "','" + IdEntity + "','" + sts[i + 1] + "')";

                            n++;
                        }
                        //如果有“备注”，则修改Entity表的EntityRemark
                        if (hasEntityRemark == 1 && !string.IsNullOrEmpty(sts[count]))
                            ModifyEntityRemark(key, sts[count]);
                    }
                }
            }
            if (n % 500 != 0)
                SqlStringList.Add(sql);  //不够500values的SQL语言也要添加进去
            InsertSqlStringList(SqlStringList);
        }
        private void InsertIntoWallTable(SheetInfo sheetInfo, Dictionary<string, int> CurrentTableColumns, Dictionary<string, int> UpdateEntities)
        {
            int IdFrame;
            int IdEntity;
            int count = sheetInfo.Names.Count;
            List<string> SqlStringList = new List<string>();  // 用来存放多条SQL语句
            String sql = "";
            int n = 0;
            foreach (String key in sheetInfo.Data.Keys)
            {
                if (UpdateEntities.Keys.Contains(key))
                {
                    IdEntity = UpdateEntities[key];
                    foreach (String[] sts in sheetInfo.Data[key])
                    {

                        for (int i = 0; i < count - hasEntityRemark; i++)
                        {
                            IdFrame = CurrentTableColumns[sheetInfo.Names.ElementAt(i)];
                            if (n % 500 == 0)
                            {
                                if (n > 0)
                                    SqlStringList.Add(sql);
                                sql = GetInsertIntoWallTableSql(IdFrame, IdEntity, sts[i + 1]);
                            }
                            else
                                sql = sql + ",('" + IdFrame + "','" + IdEntity + "','" + sts[i + 1] + "')";

                            n++;
                        }
                        //如果有“备注”，则修改Entity表的EntityRemark
                        if (hasEntityRemark == 1 && !string.IsNullOrEmpty(sts[count]))
                            ModifyEntityRemark(key, sts[count]);
                    }
                }
            }
            if (n % 500 != 0)
                SqlStringList.Add(sql);  //不够100values的SQL语言也要添加进去
            InsertSqlStringList(SqlStringList);
        }
        private void InsertIntoInclinationTable(SheetInfo sheetInfo, Dictionary<string, int> CurrentTableColumns, Dictionary<string, int> UpdateEntities, int IdType)
        {
            int IdFrame;
            int IdEntity;
            int count = sheetInfo.Names.Count;
            List<string> SqlStringList = new List<string>();  // 用来存放多条SQL语句
            String sql = "";
            int n = 0;


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
                            if (n % 500 == 0)
                            {
                                if (n > 0)
                                    SqlStringList.Add(sql);
                                sql = GetInsertIntoInclinationTableSql(IdFrame, IdEntity, DateTime.Parse(sts[0]), string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2"));
                            }
                            else
                                sql = sql + ",('" + IdFrame + "','" + IdEntity + "','" + DateTime.Parse(sts[0]) + "','" + (string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2")) + "')";

                            n++;

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
                                if (n % 500 == 0)
                                {
                                    if (n > 0)
                                        SqlStringList.Add(sql);
                                    sql = GetInsertIntoInclinationTableSql(IdFrame, IdEntity, DateTime.Parse(sts[0]), string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2"));  // Convert.ToDouble(sts[i + 1]).ToString("f2")
                                }
                                else
                                    sql = sql + ",('" + IdFrame + "','" + IdEntity + "','" + DateTime.Parse(sts[0]) + "','" + (string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2")) + "')";

                                n++;

                            }
                        }
                    }
                }
            }

            if (n % 500 != 0)
                SqlStringList.Add(sql);  //不够500values的SQL语言也要添加进去            
            InsertSqlStringList(SqlStringList);
        }

        //获取FrameTable表的COLUMN和ID
        private Dictionary<string, int> SelectColumnNames(int IdType)
        {
            Dictionary<string, int> ColumnNames = new Dictionary<string, int>();
            String sql = "select COLUMNNAME, ID from FrameTable where ID_TYPE = " + IdType;

            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        ColumnNames.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
                return ColumnNames;
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
        private Dictionary<string, int> SelectEntities(int IdType)
        {
            Dictionary<string, int> Entities = new Dictionary<string, int>();
            String sql = "select distinct ENTITY, ID from EntityTable where ID_TYPE = " + IdType;

            MySqlCommand mycom = new MySqlCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        //这里如果添加了相同的项会出错，所以entity一定要唯一
                        Entities.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
                return Entities;
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
        //执行批量插入
        private void InsertSqlStringList(List<string> SqlStringList)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = this.conn;
            command.Transaction = myTran;
            try
            {
                for (int i = 0; i < SqlStringList.Count; i++)
                {
                    string sql = SqlStringList[i].ToString();
                    if (sql.Equals(""))
                        return;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //执行单个插入语句，返回插入结果
        private void InsertOne(String sql)
        {
            if (sql.Equals(""))
                return;
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

        //表TypeTable的InsertSql
        private String GetInsertIntoTypeTableSql(String tableType, String tableRemark = "")
        {
            String sql = String.Format("insert into TypeTable (TYPENAME,TABLEREMARK) values ('{0}', '{1}')", tableType, tableRemark);
            return sql;
        }
        //表FrameTable的InsertSql
        private String GetInsertIntoFrameTableSql(int idType, String ColumnName)
        {
            String sql = String.Format("insert into FrameTable ( ID_TYPE, COLUMNNAME ) values ('{0}','{1}')", idType, ColumnName);
            return sql;
        }
        //表EntityTable的InsertSql
        private String GetInsertIntoEntityTableSql(int idType, String Entity, String EntityColumn = "测点编号", String EntityRemark = "")
        {
            String sql = String.Format("insert into EntityTable ( ID_TYPE, ENTITY, ENTITYREMARK ) values ('{0}','{1}','{2}')", idType, Entity, EntityRemark);
            return sql;
        }
        //表BaseComponentTable的InsertSql
        private String GetInsertIntoBaseCTableSql(int idFrame, int idEntity, String value)
        {
            String sql = String.Format("insert into BaseComponentTable ( ID_FRAME, ID_ENTITY, VALUE ) values ('{0}','{1}','{2}')", idFrame, idEntity, value);
            return sql;
        }
        //表wylWallTable的InsertSql
        private String GetInsertIntoWallTableSql(int idFrame, int idEntity, String value)
        {
            String sql = String.Format("insert into wylWallTable ( ID_FRAME, ID_ENTITY, VALUE ) values ('{0}','{1}','{2}')", idFrame, idEntity, value);
            return sql;
        }
        //表InclinationTable的InsertSql
        private String GetInsertIntoInclinationTableSql(int idFrame, int idEntity, DateTime date, String value)
        {
            String sql = String.Format("insert into InclinationTable ( ID_FRAME, ID_ENTITY, DATE, VALUE ) values ('{0}','{1}','{2}','{3}')", idFrame, idEntity, date, value);
            return sql;
        }

        //获取表的ID
        private int getIdType(String type)
        {
            string sql = String.Format("select ID from TypeTable where TYPENAME = '{0}'", type);
            int id = getID(sql);
            return id;
        }
        //获取表头（即COLUMNNAME）对应的ID
        private int getIdFrame(int idType, String ColumnName)
        {
            string sql = String.Format("select ID from FrameTable where ID_TYPE = {0} and COLUMNNAME = '{1}'", idType, ColumnName);
            int id = getID(sql);
            return id;
        }
        //获取实体（即ENTITY）对应的ID
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

        //*****注意，sql语句多行写的话，开始要注意留空格*******

        //查询单个CX某日期的测斜数据，返回键值对<属性，数据>
        public Dictionary<string, float> SelectOneDateData(string Entity, DateTime date)
        {
            Dictionary<string, float> data = new Dictionary<string, float>();

            String sql = String.Format("select it.DATE,cast(ft.COLUMNNAME as DECIMAL(4,2)),cast(it.VALUE as DECIMAL(5,2)) from InclinationTable it,FrameTable ft,EntityTable et "
                          + " where ft.ID_TYPE=et.ID_TYPE and it.ID_ENTITY=et.ID and it.ID_FRAME=ft.ID and et.ENTITY= '{0}' and it.DATE = '{1}' ", Entity, date);
            //判断数据库是否打开
            if (!isOpen)
            {
                OpenConnect();
            }
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
                return data;
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

        //查询单个CX连续的测量结果（每个测量日期最大值），返回键值对<日期，数据>
        public Dictionary<string, float> SelectOneCXData(string entity)
        {
            Dictionary<string, float> data = new Dictionary<string, float>();

            String sql = String.Format("Select e.Entity, c.Date,c.Value from CXView c, EntityTable e where c.ID_Entity = e.ID and e.ENTITY = '{0}' "
                            + " group by c.ID_Entity,c.Date order by c.ID_Entity,c.Date ", entity);

            //判断数据库是否打开
            if (!isOpen)
            {
                OpenConnect();
            }
            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        DateTime dateTime = reader.GetDateTime(1);
                        string str = dateTime.ToShortDateString();
                        if (dateTime.Hour >= 12)
                        {
                            str += "pm";
                        }
                        data.Add(str, reader.GetFloat(2));

                    }
                }
                return data;
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

        

        //根据Entity名，查询基础数据和地墙数据，
        public List<Revit.Addin.RevitTooltip.App.ParameterData> SelectEntityData(string entity)
        {
            List<Revit.Addin.RevitTooltip.App.ParameterData> list = new List<Revit.Addin.RevitTooltip.App.ParameterData>();
            list.Add(new Revit.Addin.RevitTooltip.App.ParameterData("测点编号", entity));
            string sql = String.Format("select BaseWallView.ColumnName, BaseWallView.Value,BaseWallView.FrameID from BaseWallView where BaseWallView.Entity = '{0}' order by BaseWallView.FrameID ", entity);
            string sql2 = String.Format("select ENTITYREMARK from EntityTable where ENTITY = '{0}'", entity);
            //判断数据库是否打开
            if (!this.isOpen)
            {
                OpenConnect();
            }
            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭           
            MySqlCommand mycom2 = new MySqlCommand(sql2, this.conn);  //建立执行命令语句对象
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        list.Add(new Revit.Addin.RevitTooltip.App.ParameterData(reader.GetString(0), reader.GetString(1)));
                    }
                }
                reader.Close();
                list.Add(new Revit.Addin.RevitTooltip.App.ParameterData("备注", Convert.ToString(mycom2.ExecuteScalar())));
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();
            }
        }

        //根据entity,修改备注
        public bool ModifyEntityRemark(string entity, string entityremark)
        {
            bool flag = false;
            string sql = String.Format("Update EntityTable set ENTITYREMARK = '{0}' where ENTITY = '{1}'", entityremark, entity);
            //判断数据库是否打开
            if (!this.isOpen)
            {
                OpenConnect();
            }
            MySqlCommand mycom;
            if (isBegin)
                mycom = new MySqlCommand(sql, this.conn, this.myTran);  //建立执行命令语句对象
            else
                mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象

            try
            {
                if (mycom.ExecuteNonQuery() > 0)
                    flag = true;
                return flag;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //读取各表数据来更新给SQLite数据库*******************************************
        public MySqlDataReader SelectTable(string tablename)
        {
            string sql = "select * from " + tablename;
            //判断数据库是否打开
            if (!this.isOpen)
            {
                OpenConnect();
            }
            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭

            return reader;
        }

        public MySqlDataReader GetCXTable()
        {
            String sql = "Select e.Entity,c.Date,c.Value from CXView c, EntityTable e where c.ID_Entity = e.ID "
                       + " group by c.ID_Entity,c.Date order by c.ID_Entity,c.Date ";

            //判断数据库是否打开
            if (!this.isOpen)
            {
                OpenConnect();
            }
            MySqlCommand mycom = new MySqlCommand(sql, this.conn);  //建立执行命令语句对象
            MySqlDataReader reader = mycom.ExecuteReader();    //需要关闭

            return reader;
        }
    }
}
