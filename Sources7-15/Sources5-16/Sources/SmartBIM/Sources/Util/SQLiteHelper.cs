using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using static Revit.Addin.RevitTooltip.App;
using System.Windows;
using System.IO;

namespace Revit.Addin.RevitTooltip.Util
{
    public class SQLiteHelper: IWriter
    {
        //单例模式
        private static SQLiteHelper sqliteHelper;
        private MysqlUtil mysql;

        //连接
        private SQLiteConnection conn;
        private string connectionString = string.Empty;
        private static string dbName = App.settings.SqliteFileName;   
        private static string dbPath = App.settings.SqliteFilePath; 


        //事务
        private SQLiteTransaction myTran;
        //是否打开事务
        private bool isBegin = false;
        //是否有备注
        private int hasEntityRemark = 0;

        //构造函数
        private SQLiteHelper()
        {
            setSQLiteDbDirectory(dbPath);
            this.connectionString = "Data Source=" + dbName;
            this.conn = new SQLiteConnection(this.connectionString);
        }
        //单例模式
        public static SQLiteHelper CreateInstance()
        {
            if (null == sqliteHelper)
            {
                sqliteHelper = new SQLiteHelper();
            }
            
            else if (!App.settings.SqliteFileName.Equals(SQLiteHelper.dbName) ||
               !App.settings.SqliteFilePath.Equals(SQLiteHelper.dbPath))
            {
                SQLiteHelper.dbName = App.settings.SqliteFileName;
                SQLiteHelper.dbPath = App.settings.SqliteFilePath;
                sqliteHelper.Dispose();
                sqliteHelper = new SQLiteHelper();
            }
            return sqliteHelper;
        }
        //建立SQLite数据库连接
        public void OpenConnect()
        {
            if (null == conn)
            {
                this.connectionString = "Data Source=" + dbName;
                this.conn = new SQLiteConnection(this.connectionString);
            }
            conn.Open();
        }
        //关闭数据库
        public void Close()
        {
            conn.Close();
        }

        //销毁当前对象
        public void Dispose()
        {
            conn.Close();
            conn.Dispose();
            GC.SuppressFinalize(this);     //垃圾回收机制跳过this           
        }

        //设置SQLiteDB目录
        public void setSQLiteDbDirectory(string path)
        {
            System.IO.Directory.SetCurrentDirectory(path);
        }

        //如果本地有SQLiteDB数据库，先删除，再创建，并从Mysql数据中导入数据 
        public void UpdateDB()
        {
            //先关闭数据库连接
            if (conn.State == ConnectionState.Open)
                Close();
           
            //删除原有本地数据库
            if (System.IO.File.Exists(Path.Combine(App.settings.SqliteFilePath,App.settings.SqliteFileName)))
            {
                System.IO.File.Delete(Path.Combine(App.settings.SqliteFilePath, App.settings.SqliteFileName));               
            }

            mysql = MysqlUtil.CreateInstance();
            //打开数据库连接       
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }            

            using (DbTransaction transaction = conn.BeginTransaction())
            {
                CreateDB();
                InsertDBfromMysql();
                transaction.Commit();
            }
        }
        //创建SQLite数据库文件
        public void CreateDB()
        {
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            using (SQLiteCommand command = new SQLiteCommand(this.conn))
            {
                if (!isExist("TypeTable", "table"))
                {
                    command.CommandText = "CREATE TABLE TypeTable(ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,TYPENAME VARCHAR(20) NOT NULL, TABLEREMARK VARCHAR(100) )";
                    command.ExecuteNonQuery();
                }              

                if (!isExist("FrameTable", "table"))
                {
                    command.CommandText = "CREATE TABLE FrameTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_TYPE integer NOT NULL REFERENCES TypeTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, COLUMNNAME VARCHAR(20) NOT NULL )";
                    command.ExecuteNonQuery();
                }                

                if (!isExist("EntityTable", "table"))
                {
                    command.CommandText = "CREATE TABLE EntityTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_TYPE integer NOT NULL REFERENCES TypeTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ENTITY VARCHAR(20) NOT NULL, ENTITYCOLUMN VARCHAR(20), ENTITYREMARK VARCHAR(100))";
                    command.ExecuteNonQuery();
                }                

                if (!isExist("BaseComponentTable", "table"))
                {
                    command.CommandText = "CREATE TABLE BaseComponentTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_FRAME integer NOT NULL REFERENCES FrameTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ID_ENTITY integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, VALUE VARCHAR(20))";
                    command.ExecuteNonQuery();
                }                

                if (!isExist("wylWallTable", "table"))
                {
                    command.CommandText = "CREATE TABLE wylWallTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_FRAME integer NOT NULL REFERENCES FrameTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ID_ENTITY integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, VALUE VARCHAR(20))";
                    command.ExecuteNonQuery();
                }

                if (!isExist("InclinationTable", "table"))
                {
                    command.CommandText = "CREATE TABLE InclinationTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_FRAME integer NOT NULL REFERENCES FrameTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ID_ENTITY integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, DATE VARCHAR(20), VALUE VARCHAR(10))";
                    command.ExecuteNonQuery();                  
                }
                
                if (!isExist("CXTable", "table"))
                {
                    command.CommandText = "CREATE TABLE CXTable (ID integer PRIMARY KEY AUTOINCREMENT, ENTITY VARCHAR(20), DATE VARCHAR(20), VALUE FLOAT)";
                    command.ExecuteNonQuery();
                }                

                if (!isExist("BaseWallView", "view"))
                {
                    command.CommandText = "Create view BaseWallView (Entity, ColumnName, Value, FrameID) AS Select et.ENTITY, ft.COLUMNNAME, bt.VALUE, ft.ID from  EntityTable et, FrameTable ft, BaseComponentTable bt Where et.ID_TYPE = ft.ID_TYPE and bt.ID_ENTITY = et.ID and bt.ID_FRAME = ft.ID "
                                          +" UNION Select et.ENTITY, ft.COLUMNNAME, wt.VALUE, ft.ID from  EntityTable et, FrameTable ft, wylWallTable wt Where et.ID_TYPE = ft.ID_TYPE and wt.ID_ENTITY = et.ID and wt.ID_FRAME = ft.ID ";
                    command.ExecuteNonQuery();
                }

                if(!isExist("CXView", "view"))
                {
                    command.CommandText = "Create view CXView(ID_Entity, Date, Value) AS Select ID_Entity, DATE, MAX(cast(VALUE as DECIMAL(5, 2))) from InclinationTable group by ID_Entity,DATE order by ID_Entity, DATE ";
                    command.ExecuteNonQuery();                   
                }
            }

        }

        //插入数据
        private void InsertDBfromMysql()
        {
            InsertTypeTableFromMysql();
            InsertFrameTableFromMysql();
            InsertEntityTableFromMysql();
            InsertBaseComponentTableFromMysql();
            InsertWallTableFromMysql();
            InsertCXTableFromMysql();
        }       

        //判断表是否存在
        private bool isExist(string table, string type)
        {
            bool flag = false;
            string sql = String.Format("select count(*) from sqlite_master where type='{0}' and name = '{1}' ", type, table);

            using (SQLiteCommand command = new SQLiteCommand(sql, this.conn))
            {
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                    flag = true;
            }

            return flag;
        }

        private void InsertTypeTableFromMysql()
        {
            //读取typetable表数据，并插入
            MySqlDataReader reader = this.mysql.SelectTable("typetable");
            string sql = "INSERT OR IGNORE INTO TypeTable(ID,TYPENAME,TABLEREMARK)values(@ID,@TYPENAME,@TABLEREMARK)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ID", reader.GetInt64(0)),
                            new SQLiteParameter("@TYPENAME", reader.GetString(1)),
                            new SQLiteParameter("@TABLEREMARK", reader.GetString(2))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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
        private void InsertFrameTableFromMysql()
        {
            //读取frametable表数据并插入
            MySqlDataReader reader = this.mysql.SelectTable("frametable");
            string sql = "INSERT OR IGNORE INTO FrameTable(ID,ID_TYPE,COLUMNNAME)values(@ID,@ID_TYPE,@COLUMNNAME)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ID", reader.GetInt64(0)),
                            new SQLiteParameter("@ID_TYPE", reader.GetInt64(1)),
                            new SQLiteParameter("@COLUMNNAME", reader.GetString(2))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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
        private void InsertEntityTableFromMysql()
        {
            //读取entitytable表数据并插入
            MySqlDataReader reader = this.mysql.SelectTable("entitytable");
            string sql = "INSERT OR IGNORE INTO EntityTable(ID,ID_TYPE,ENTITY,ENTITYCOLUMN,ENTITYREMARK)values(@ID,@ID_TYPE,@ENTITY,@ENTITYCOLUMN,@ENTITYREMARK)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ID", reader.GetInt64(0)),
                            new SQLiteParameter("@ID_TYPE", reader.GetInt64(1)),
                            new SQLiteParameter("@ENTITY", reader.GetString(2)),
                            new SQLiteParameter("@ENTITYCOLUMN", reader.GetString(3)),
                            new SQLiteParameter("@ENTITYREMARK", reader.GetString(4))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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
        private void InsertBaseComponentTableFromMysql()
        {
            //读取BaseComponentTable表数据并插入
            MySqlDataReader reader = this.mysql.SelectTable("BaseComponentTable");
            string sql = "INSERT OR IGNORE INTO BaseComponentTable(ID,ID_FRAME,ID_ENTITY,VALUE)values(@ID,@ID_FRAME,@ID_ENTITY,@VALUE)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ID", reader.GetInt64(0)),
                            new SQLiteParameter("@ID_FRAME", reader.GetInt64(1)),
                            new SQLiteParameter("@ID_ENTITY", reader.GetInt64(2)),
                            new SQLiteParameter("@VALUE", reader.GetString(3))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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
        private void InsertWallTableFromMysql()
        {
            //读取wylWallTable表数据并插入
            MySqlDataReader reader = this.mysql.SelectTable("wylWallTable");
            string sql = "INSERT OR IGNORE INTO wylWallTable(ID,ID_FRAME,ID_ENTITY,VALUE)values(@ID,@ID_FRAME,@ID_ENTITY,@VALUE)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ID", reader.GetInt64(0)),
                            new SQLiteParameter("@ID_FRAME", reader.GetInt64(1)),
                            new SQLiteParameter("@ID_ENTITY", reader.GetInt64(2)),
                            new SQLiteParameter("@VALUE", reader.GetString(3))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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
        private void InsertCXTableFromMysql()
        {
            //读取数据并插入
            MySqlDataReader reader = this.mysql.GetCXTable();
            string sql = "INSERT OR IGNORE INTO CXTable(ENTITY,DATE,VALUE)values(@ENTITY,@DATE,@VALUE)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        DateTime dateTime = reader.GetDateTime(1);
                        string datestr = dateTime.ToString("yyyy-MM-dd");
                        if (dateTime.Hour >= 12)
                        {
                            datestr += "pm";
                        }
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ENTITY", reader.GetString(0)),
                            new SQLiteParameter("@DATE", datestr),  //reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss")
                            new SQLiteParameter("@VALUE", reader.GetFloat(2))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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

        //对SQLite数据库执行增删改操作
        public int ExecuteNonQuery(string sql, SQLiteParameter[] parameters)
        {
            int affectedRows = 0;
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }

            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                affectedRows = command.ExecuteNonQuery();
            }                    
            return affectedRows;
        }

        //*****************************插入Excel数据***************************
        //导入excel表，插入一个SheetInfo
        public int InsertSheetInfo(SheetInfo sheetInfo)
        {
            //判断数据库是否打开
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            //if (!System.IO.File.Exists(Path.Combine(App.settings.SqliteFilePath, App.settings.SqliteFileName)))
            if (!System.IO.File.Exists(Path.Combine(dbName, dbPath)))
            {
                CreateDB();
            }
            //if (!isExist("TypeTable", "table"))
            //{
            //    CreateDB();                             
            //}
            //事务开始               
            myTran = conn.BeginTransaction();
            isBegin = true;
            try
            {
                if (null == sheetInfo)
                {
                    return 0;
                }
                //插入到TypeTable表,并返回ID
                int IdType = InsertIntoTypeTable(sheetInfo);

                //插入到FrameTable表，并返回当前表的键值对<属性名，ID>
                Dictionary<string, int> CurrentTableColumns = InsertIntoFrameType(sheetInfo.Names, IdType);

                //插入到EntityTable表，并返回新插入的实体数据的键值对<实体名,ID>
                Dictionary<string, int> UpdateEntities = InsertIntoEntityTable(sheetInfo.EntityName, IdType);

                if (sheetInfo.TableType == ExcelType.BaseExcel)
                {
                    InsertIntoBaseTable(sheetInfo, CurrentTableColumns, UpdateEntities);
                }
                else if (sheetInfo.TableType == ExcelType.WallExcel)
                {
                    InsertIntoWallTable(sheetInfo, CurrentTableColumns, UpdateEntities);
                }
                else if (sheetInfo.TableType == ExcelType.DataExcel)
                {
                    InsertIntoInclinationTable(sheetInfo, CurrentTableColumns, UpdateEntities, IdType);
                    InsertIntoCXTable(sheetInfo.EntityName);
                }

                myTran.Commit();    //事务提交
                hasEntityRemark = 0;
                return 1;
            }
            catch (Exception e)
            {
                myTran.Rollback();    // 事务回滚
                hasEntityRemark = 0;
                //删除entitytable表中当前sheet的entity
                DeleteCurrentEntity(sheetInfo.EntityName, InsertIntoTypeTable(sheetInfo));

                throw new Exception("事务操作出错，系统信息：" + e.Message);
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

            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn, myTran);  //建立执行命令语句对象，其中myTran为事务
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
                            string datestr = DateTime.Parse(sts[0]).ToString("yyyy-MM-dd HH:mm:ss");                            

                            if (n % 500 == 0)
                            {
                                if (n > 0)
                                    SqlStringList.Add(sql);
                                sql = GetInsertIntoInclinationTableSql(IdFrame, IdEntity, datestr, string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2"));
                            }
                            else
                                sql = sql + ",('" + IdFrame + "','" + IdEntity + "','" + datestr + "','" + (string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2")) + "')";

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
                                string datestr = DateTime.Parse(sts[0]).ToString("yyyy-MM-dd HH:mm:ss");
                                if (n % 500 == 0)
                                {
                                    if (n > 0)
                                        SqlStringList.Add(sql);
                                    sql = GetInsertIntoInclinationTableSql(IdFrame, IdEntity, datestr, string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2"));  // Convert.ToDouble(sts[i + 1]).ToString("f2")
                                }
                                else
                                    sql = sql + ",('" + IdFrame + "','" + IdEntity + "','" + datestr + "','" + (string.IsNullOrEmpty(sts[i + 1]) ? "0" : Convert.ToDouble(sts[i + 1]).ToString("f2")) + "')";

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
        private void InsertIntoCXTable(List<string> EntityName)
        {
            string CX = null;
            foreach (string e in EntityName)
            {
                CX = e;
            }
            Console.WriteLine(CX+"*********************************************************");
            Console.WriteLine(HasCX(CX) + "*********************************************************");
            if (HasCX(CX))
                return;
            
            String sql1 = String.Format( "Select e.Entity,c.Date,c.Value from CXView c, EntityTable e where c.ID_Entity = e.ID and e.Entity = '{0}' "
                       + " group by c.ID_Entity,c.Date order by c.ID_Entity,c.Date ", CX);

            SQLiteCommand mycom = new SQLiteCommand(sql1, this.conn, myTran);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();
            string sql = "INSERT OR IGNORE INTO CXTable(ENTITY,DATE,VALUE)values(@ENTITY,@DATE,@VALUE)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        DateTime dateTime = Convert.ToDateTime(reader.GetString(1));
                        string datestr = dateTime.ToString("yyyy-MM-dd");
                        if (dateTime.Hour >= 12)
                        {
                            datestr += "pm";
                        }
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ENTITY", reader.GetString(0)),
                            new SQLiteParameter("@DATE", datestr),  //reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss")
                            new SQLiteParameter("@VALUE", reader.GetFloat(2))
                        };
                        ExecuteNonQuery(sql, paramenters);
                    }
                }
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
        private Boolean HasCX(string CX)
        {
            string sql =String.Format( "select * from CXTable where ENTITY = '{0}'", CX);
            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();
            try
            {
                reader.Read();
                if (reader.HasRows)
                    return true;
                else
                    return false;
            }
            finally
            {
                reader.Close();
            }
                            
        }
        //Test############################################################################
        public List<string> SelectCXTable()
        {
            Dictionary<string, string> cxTable = new Dictionary<string, string>();
            List<string> list = new List<string>();
            //String sql = "Select e.Entity,c.Date,c.Value from CXView c, EntityTable e where c.ID_Entity = e.ID "
            //           + " group by c.ID_Entity,c.Date order by c.ID_Entity,c.Date ";

           // String sql = " Select ENTITY,DATE,VALUE from CXTable group by ENTITY,DATE order by ENTITY,DATE";
            String sql = " Select ENTITY,DATE,VALUE from CXTable order by ENTITY,DATE";

            //判断数据库是否打开
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        
                        //cxTable.Add(reader.GetString(1), reader.GetFloat(2));
                        //DateTime dateTime = Convert.ToDateTime(reader.GetString(1));
                        //string datestr = dateTime.ToString("yyyy-MM-dd");
                        //if (dateTime.Hour >= 12)
                        //{
                        //    datestr += "pm";
                        //}
                        list.Add(reader.GetString(1));
                    }
                }
                return list;
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

        //获取FrameTable表的COLUMN和ID
        private Dictionary<string, int> SelectColumnNames(int IdType)
        {
            Dictionary<string, int> ColumnNames = new Dictionary<string, int>();
            String sql = "select COLUMNNAME, ID from FrameTable where ID_TYPE = " + IdType;

            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();    //需要关闭
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

            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();    //需要关闭
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

            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn, myTran);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();    //需要关闭
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
            SQLiteCommand command = new SQLiteCommand();
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
            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn, myTran);  //建立执行命令语句对象，其中myTran为事务

            try
            {
                mycom.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //根据entity,修改备注
        public bool ModifyEntityRemark(string entity, string entityremark)
        {
            bool flag = false;
            string sql = String.Format("Update EntityTable set ENTITYREMARK = '{0}' where ENTITY = '{1}'", entityremark, entity);
            //判断数据库是否打开
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            SQLiteCommand mycom;
            if (isBegin)
                mycom = new SQLiteCommand(sql, this.conn, this.myTran);  //建立执行命令语句对象
            else
                mycom = new SQLiteCommand(sql, this.conn);  //建立执行命令语句对象

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
        private String GetInsertIntoInclinationTableSql(int idFrame, int idEntity, string date, String value)
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
            SQLiteCommand mycom = new SQLiteCommand(sql,this.conn);  //建立执行命令语句对象
            int id = 0;
            try
            {
                id = Convert.ToInt32(mycom.ExecuteScalar());   //查询返回一个值的时候，用ExecuteScalar()更节约资源，快捷
            }
            catch { throw; }
            return id;
        }



        //*****************************查询功能*******************************

        //查询单个CX某日期的测斜数据，返回键值对<属性，数据>
        public Dictionary<string, float> SelectOneDateData(string Entity, DateTime date)
        {
            Dictionary<string, float> data = new Dictionary<string, float>();

            String sql = String.Format("select it.DATE,cast(ft.COLUMNNAME as DECIMAL(4,2)),cast(it.VALUE as DECIMAL(5,2)) from InclinationTable it,FrameTable ft,EntityTable et "
                          + " where ft.ID_TYPE=et.ID_TYPE and it.ID_ENTITY=et.ID and it.ID_FRAME=ft.ID and et.ENTITY= '{0}' and it.DATE = '{1}' ", Entity, date);

            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("EntityTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return data;
            }
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))  //建立执行命令语句对象
            {
                SQLiteDataReader reader = command.ExecuteReader();
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
                    reader.Close();  //关闭
                }
            }
        }

        //查询单个CX连续的测量结果（每个测量日期最大值），返回键值对<日期，数据>
        public Dictionary<string, float> SelectOneCXData(string entity)
        {
            Dictionary<string, float> data = new Dictionary<string, float>();

             string sql = String.Format(" select ID,DATE,VALUE from CXTable where ENTITY = '{0}' order by ID ", entity);
           // string sql = String.Format(" select ID,DATE,MAX(VALUE) from CXTable where ENTITY = '{0}' group by DATE order by ID ", entity);

            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("CXTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return data;
            }
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))  //建立执行命令语句对象
            {
                SQLiteDataReader reader = command.ExecuteReader();
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
                    reader.Close();  //关闭
                }
            }
        }

        /// <summary>
        /// 
        ///查询CX中的异常点，返回异常的entity值
        /// </summary>
        /// <param name="threshold">累计预警值threshold</param>
        /// <param name="D_value">相邻差值预警D_value</param>
        /// <returns>返回list<string></returns>
        public List<ParameterData> SelectExceptionalCX(double threshold, double D_value)
        {
            List<ParameterData> ExceptionalCX = new List<ParameterData>();
            string sql = String.Format("select distinct b.ENTITY from CXTable a, CXTable b where a.ENTITY = b.ENTITY and b.ID - a.ID = 1 and b.VALUE - a.VALUE > {0} or b.VALUE > {1} ", D_value, threshold);

            //判断数据库是否打开
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("CXTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return ExceptionalCX;
            }
           
            using (SQLiteCommand command = new SQLiteCommand(sql, conn)) //建立执行命令语句对象
            {
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            ExceptionalCX.Add(new ParameterData(reader.GetString(0), reader.GetString(0)));
                        }
                    }
                    return ExceptionalCX;

                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    reader.Close();  //关闭
                }
            }
        }

        //根据Entity名，查询基础数据和地墙数据，   //Revit.Addin.RevitTooltip.App.ParameterData
        public List<ParameterData> SelectEntityData(string entity)
        {
            List<ParameterData> list = new List<ParameterData>();
            list.Add(new ParameterData("测点编号", entity));
            string sql = String.Format("select BaseWallView.ColumnName, BaseWallView.Value,BaseWallView.FrameID from BaseWallView where BaseWallView.Entity = '{0}' order by BaseWallView.FrameID ", entity);
            string sql2 = String.Format("select ENTITYREMARK from EntityTable where ENTITY = '{0}'", entity);
            //判断数据库是否打开
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
            }
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("BaseWallView", "view"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return list;
            }
            SQLiteCommand mycom = new SQLiteCommand(sql, this.conn);  //建立执行命令语句对象
            SQLiteDataReader reader = mycom.ExecuteReader();    //需要关闭           
            SQLiteCommand mycom2 = new SQLiteCommand(sql2, this.conn);  //建立执行命令语句对象
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        list.Add(new ParameterData(reader.GetString(0), reader.GetString(1)));
                    }
                }
                reader.Close();
                list.Add(new ParameterData("备注", Convert.ToString(mycom2.ExecuteScalar())));
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

    }
}
