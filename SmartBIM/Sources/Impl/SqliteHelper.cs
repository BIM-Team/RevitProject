using Res = Revit.Addin.RevitTooltip.Properties.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using Revit.Addin.RevitTooltip.Intface;
using Revit.Addin.RevitTooltip.Dto;
using System.Windows;
using System.IO;
using System.Text;
using System.Linq;

namespace Revit.Addin.RevitTooltip.Impl
{
    public class SQLiteHelper : ISQLiteHelper, IDisposable
    {
        /// <summary>
        /// 保存当前的一个实例
        /// </summary>
        private static SQLiteHelper _sqliteHelper = null;
        /// <summary>
        /// 获取实例
        /// </summary>
        public static SQLiteHelper Instance { get { return _sqliteHelper; } }

        //连接
        private SQLiteConnection conn;


        private string dbName = null;
        private string dbPath = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLiteHelper()
        {
            this.dbName = "SqliteDB.db";
            this.dbPath = "E:\\revit数据文档";
            if (!Directory.Exists(this.dbPath))
            {
                Directory.CreateDirectory(this.dbPath);
            }
            Directory.SetCurrentDirectory(this.dbPath);
            this.conn = new SQLiteConnection("Data Source = " + dbName);
            _sqliteHelper = this;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="settings"></param>
        internal SQLiteHelper(RevitTooltip settings)
        {
            this.dbName = settings.SqliteFileName;
            this.dbPath = settings.SqliteFilePath;
            if (!Directory.Exists(this.dbPath))
            {
                Directory.CreateDirectory(this.dbPath);
            }
            Directory.SetCurrentDirectory(this.dbPath);
            this.conn = new SQLiteConnection("Data Source=" + dbName);
            _sqliteHelper = this;
        }
        /// <summary>
        /// 销毁当前对象
        /// </summary>
        public void Dispose()
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            conn.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 从Mysql数据库中导入数据
        /// 如果本地有同名的Sqlite数据库文件，先删除，再创建
        /// </summary>
        public bool LoadDataToSqlite()
        {
            bool result = false;
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            string file_path = Path.Combine(this.dbPath, this.dbName);
            if (File.Exists(file_path))
            {
                File.Delete(file_path);
            }
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (DbTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    CreateDB();
                    InsertDBfromMysql();
                    transaction.Commit();
                    result = true;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
            return result;
        }
        //创建SQLite数据库文件
        public void CreateDB()
        {
            //如果连接没有打开则打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (SQLiteCommand command = new SQLiteCommand(conn))
            {
                if (!isExist("ExcelTable", "table"))
                {
                    command.CommandText = "CREATE TABLE ExcelTable(ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,CurrentFile VARCHAR(30) NOT NULL,ExcelSignal VARCHAR(20) UNIQUE, IsInfo BOOLEAN NOT NULL, Total_hold VARCHAR(20) NOT NULL default '0', Diff_hold VARCHAR(20) NOT NULL default '0',History VARCHAR(100) NOT NULL ,Version VARCHAR(20) NOT NULL ,Total_operator VARCHAR(10) NOT NULL default '>' ,Diff_operator VARCHAR(10) NOT NULL default '>')";
                    command.ExecuteNonQuery();
                }
                if (!isExist("KeyTable", "table"))
                {
                    command.CommandText = "CREATE TABLE KeyTable(ID integer PRIMARY KEY AUTOINCREMENT, ExcelSignal VARCHAR(20) NOT NULL, Group_ID integer , KeyName VARCHAR(20) NOT NULL,Odr integer NOT NULL default 0 ,Version VARCHAR(20) NOT NULL)";
                    command.ExecuteNonQuery();
                }
                if (!isExist("EntityTable", "table"))
                {
                    command.CommandText = "CREATE TABLE EntityTable(ID integer PRIMARY KEY AUTOINCREMENT, ExcelSignal VARCHAR(20) NOT NULL, EntityName VARCHAR(20) NOT NULL, Remark VARCHAR(100) ,Version VARCHAR(20) NOT NULL)";
                    command.ExecuteNonQuery();
                }
                if (!isExist("GroupTable", "table"))
                {
                    command.CommandText = "CREATE TABLE GroupTable(ID integer PRIMARY KEY AUTOINCREMENT, ExcelSignal VARCHAR(20) NOT NULL, GroupName VARCHAR(20) NOT NULL)";
                    command.ExecuteNonQuery();
                }
                if (!isExist("InfoTable", "table"))
                {
                    command.CommandText = "CREATE TABLE InfoTable(ID integer PRIMARY KEY AUTOINCREMENT, Key_ID integer NOT NULL REFERENCES KeyTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, Entity_ID integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, Value VARCHAR(20) NOT NULL ,Version VARCHAR(20) NOT NULL)";
                    command.ExecuteNonQuery();
                }
                if (!isExist("DrawTable", "table"))
                {
                    command.CommandText = "CREATE TABLE DrawTable(ID integer PRIMARY KEY AUTOINCREMENT,  Entity_ID integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, Date VARCHAR(20) NOT NULL, EntityMaxValue float NOT NULL, EntityMidValue float NOT NULL, EntityMinValue float NOT NULL, Detail TEXT NOT NULL ,Version VARCHAR(20) NOT NULL)";
                    command.ExecuteNonQuery();
                }
            }
        }

        //插入数据
        private void InsertDBfromMysql()
        {
            if (!MysqlUtil.Instance.IsReady)
            {
                return;
            }
            string conn_string = MysqlUtil.Instance.ConnectionMessage;
            string newTimeStamp = Convert.ToInt64((DateTime.Now - new DateTime(2016, 12, 14, 0, 0, 0)).TotalMilliseconds).ToString();
            MySqlConnection mysql_conn = new MySqlConnection(conn_string);
            MySqlDataReader mysql_reader = null;
            try
            {
                //sqlite
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SQLiteCommand sqlite_command = new SQLiteCommand(conn);
                //mysql
                mysql_conn.Open();
                string select_sql = "Select ID,CurrentFile,ExcelSignal,IsInfo,Total_hold,Diff_hold,History,Total_operator,Diff_operator From ExcelTable";
                MySqlCommand mysql_command = new MySqlCommand(select_sql, mysql_conn);
                //ExcelTable
                mysql_reader = mysql_command.ExecuteReader();
                if (mysql_reader.HasRows)
                {
                    while (mysql_reader.Read())
                    {
                        sqlite_command.CommandText = string.Format("INSERT OR IGNORE INTO ExcelTable(ID, CurrentFile, ExcelSignal, IsInfo, Total_hold, Diff_hold, History,Version,Total_operator,Diff_operator)values({0},'{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}')",
                            mysql_reader.GetInt32(0), mysql_reader.GetString(1), mysql_reader.GetString(2), mysql_reader.GetInt32(3), mysql_reader.GetString(4), mysql_reader.GetString(5), mysql_reader.GetString(6), newTimeStamp, mysql_reader.GetString(7), mysql_reader.GetString(8));
                        sqlite_command.ExecuteNonQuery();
                    }
                }
                mysql_reader.Close();
                //KeyTable
                mysql_command.CommandText = "Select ID,ExcelSignal,Group_ID,KeyName,Odr From KeyTable";
                mysql_reader = mysql_command.ExecuteReader();
                if (mysql_reader.HasRows)
                {
                    while (mysql_reader.Read())
                    {
                        sqlite_command.CommandText = string.Format("INSERT OR IGNORE INTO KeyTable(ID,ExcelSignal,Group_ID,KeyName,Odr,Version) Values({0},'{1}',{2},'{3}',{4},'{5}')",
                           mysql_reader.GetInt32(0), mysql_reader.GetString(1), mysql_reader.IsDBNull(2) ? "null" : mysql_reader.GetInt32(2).ToString(), mysql_reader.GetString(3), mysql_reader.GetInt32(4), newTimeStamp);
                        sqlite_command.ExecuteNonQuery();
                    }
                }
                mysql_reader.Close();
                //EntityTable
                mysql_command.CommandText = "Select ID, ExcelSignal,EntityName,Remark From EntityTable";
                mysql_reader = mysql_command.ExecuteReader();
                if (mysql_reader.HasRows)
                {
                    while (mysql_reader.Read())
                    {
                        sqlite_command.CommandText = string.Format("INSERT OR IGNORE INTO EntityTable(ID, ExcelSignal,EntityName,Remark,Version) Values({0},'{1}','{2}','{3}','{4}')",
                           mysql_reader.GetInt32(0), mysql_reader.GetString(1), mysql_reader.GetString(2), mysql_reader.IsDBNull(3) ? "null" : mysql_reader.GetString(3), newTimeStamp);
                        sqlite_command.ExecuteNonQuery();
                    }
                }
                mysql_reader.Close();
                //GroupTable
                mysql_command.CommandText = "Select ID,ExcelSignal,GroupName From GroupTable";
                mysql_reader = mysql_command.ExecuteReader();
                if (mysql_reader.HasRows)
                {
                    while (mysql_reader.Read())
                    {
                        sqlite_command.CommandText = string.Format("INSERT OR IGNORE INTO GroupTable(ID,ExcelSignal,GroupName) Values({0},'{1}','{2}')",
                           mysql_reader.GetInt32(0), mysql_reader.GetString(1), mysql_reader.GetString(2));
                        sqlite_command.ExecuteNonQuery();
                    }
                }
                mysql_reader.Close();
                //InfoTable
                mysql_command.CommandText = "Select ID, Key_ID,Entity_ID,Value From InfoTable";
                mysql_reader = mysql_command.ExecuteReader();
                if (mysql_reader.HasRows)
                {
                    while (mysql_reader.Read())
                    {
                        sqlite_command.CommandText = string.Format("INSERT OR IGNORE INTO InfoTable(ID, Key_ID,Entity_ID,Value,Version) Values({0},{1},{2},'{3}','{4}')",
                           mysql_reader.GetInt32(0), mysql_reader.GetInt32(1), mysql_reader.GetInt32(2), mysql_reader.GetString(3), newTimeStamp);
                        sqlite_command.ExecuteNonQuery();
                    }
                }
                mysql_reader.Close();
                //DrawTable
                mysql_command.CommandText = "Select ID,Entity_ID, Date ,EntityMaxValue,EntityMidValue,EntityMinValue,Detail From DrawTable ";
                mysql_reader = mysql_command.ExecuteReader();
                if (mysql_reader.HasRows)
                {
                    while (mysql_reader.Read())
                    {
                        sqlite_command.CommandText = string.Format("INSERT OR IGNORE INTO DrawTable(ID,Entity_ID,Date ,EntityMaxValue,EntityMidValue,EntityMinValue,Detail,Version) Values({0},{1},'{2}',{3},{4},{5},'{6}','{7}')",
                            mysql_reader.GetInt32(0), mysql_reader.GetInt32(1), mysql_reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss"), mysql_reader.GetFloat(3), mysql_reader.GetFloat(4), mysql_reader.GetFloat(5), mysql_reader.GetString(6), newTimeStamp);
                        sqlite_command.ExecuteNonQuery();
                    }
                }
                mysql_reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (!mysql_reader.IsClosed)
                {
                    mysql_reader.Close();
                }
                mysql_conn.Close();
                mysql_conn.Dispose();
            }
        }

        /// <summary>
        /// 判断Sqlite数据库中表是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool isExist(string table, string type)
        {
            bool flag = false;
            //连接没有打开
            if (this.conn.State != ConnectionState.Open)
            {
                this.conn.Open();
            }
            string sql = String.Format("select count(*) from sqlite_master where type='{0}' and name = '{1}' ", type, table);
            using (SQLiteCommand command = new SQLiteCommand(sql, this.conn))
            {
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                    flag = true;
            }
            return flag;
        }

        //*****************************插入Excel数据***************************

        /// <summary>
        /// 插入SheetInfo
        /// </summary>
        public void InsertSheetInfo(SheetInfo sheetInfo)
        {
            if (sheetInfo == null)
            {
                throw new Exception("无效的传入参数");
            }
            if (!File.Exists(Path.Combine(dbPath, dbName)))
            {
                CreateDB();
            }
            if (sheetInfo.Tag)
            {
                InsertInfoData(sheetInfo);
            }
            else
            {
                InsertDrawData(sheetInfo);
            }
        }
        /// <summary>
        /// 插入基础数据表
        /// </summary>
        /// <param name="sheetInfo"></param>
        private void InsertInfoData(SheetInfo sheetInfo)
        {
            string newTimeStamp = Convert.ToInt64((DateTime.Now - new DateTime(2016, 12, 14, 0, 0, 0)).TotalMilliseconds).ToString();
            //是否已经处理过
            bool hasDone = false;
            string signal = sheetInfo.ExcelTableData.Signal;
            string reset_auto_increment = "DELETE FROM sqlite_sequence";
            SQLiteTransaction tran = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                //事务开始
                tran = conn.BeginTransaction();
                //重置自增
                new SQLiteCommand(reset_auto_increment, conn, tran).ExecuteNonQuery();
                //插入到ExcelTable表,并返回ID
                if (sheetInfo.ExcelTableData == null)
                {
                    throw new Exception("无效的插入数据");
                }
                //判断是否该Signal已存在
                ExcelTable exist = SelectExcelTable(signal);
                if (exist == null)
                {
                    //插入ExcelTable
                    new SQLiteCommand(string.Format("insert into ExcelTable (CurrentFile,ExcelSignal,IsInfo,History,Version) values ('{0}', '{1}', {2},'{3}','{4}')",
                         sheetInfo.ExcelTableData.CurrentFile, signal, sheetInfo.Tag ? 1 : 0, sheetInfo.ExcelTableData.CurrentFile, newTimeStamp), tran.Connection, tran).ExecuteNonQuery();
                    exist = SelectExcelTable(signal);
                    //插入表结构KeyNames
                    new SQLiteCommand(InsertIntoKeyTable(sheetInfo.KeyNames, sheetInfo.ExcelTableData.Signal, newTimeStamp), tran.Connection, tran).ExecuteNonQuery();
                }
                else
                {
                    string[] his = exist.History.Split(';');
                    string currentFile = sheetInfo.ExcelTableData.CurrentFile;
                    //判断是否已经做过处理
                    foreach (string s in his)
                    {
                        if (currentFile.Equals(s))
                        {
                            hasDone = true;
                            break;
                        }
                    }
                    //对于没有处理过的，添加到History中
                    if (!hasDone)
                    {
                        string history = exist.History + ";" + sheetInfo.ExcelTableData.CurrentFile;
                        //更新已有的数据表
                        new SQLiteCommand(string.Format("update ExcelTable set CurrentFile='{0}',History='{1}',Version='{3}' where ExcelSignal='{2}'",
                           sheetInfo.ExcelTableData.CurrentFile, history, signal, newTimeStamp), tran.Connection, tran).ExecuteNonQuery();
                    }
                }
                if (!hasDone)
                {
                    List<CKeyName> inMysqls = SelectKeyNames(signal, conn);
                    if (inMysqls == null || inMysqls.Count == 0)
                    {
                        throw new Exception("数据异常");
                    }
                    //构造Map
                    Dictionary<string, int> KeyMap = new Dictionary<string, int>();
                    foreach (CKeyName one in inMysqls)
                    {
                        KeyMap.Add(one.KeyName, one.Id);
                    }
                    InsertInfoTable(sheetInfo, KeyMap, tran, newTimeStamp);

                }
                tran.Commit();    //事务提交
            }
            catch (Exception e)
            {
                tran.Rollback();    // 事务回滚
                this.RollBack(newTimeStamp);
                throw e;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 插入EntityInfo数据
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="KeyMap"></param>
        /// <param name="command"></param>
        private void InsertInfoTable(SheetInfo sheetInfo, Dictionary<string, int> KeyMap, SQLiteTransaction tran, string timeStamp)
        {
            if (sheetInfo.InfoRows == null || sheetInfo.InfoRows.Count == 0)
            {

                throw new Exception("无效的数据");
            }
            List<InfoEntityData> rows = sheetInfo.InfoRows;
            string signal = sheetInfo.ExcelTableData.Signal;

            foreach (InfoEntityData one in rows)
            {
                string sql = string.Format("insert into EntityTable(ExcelSignal,EntityName,Version) values ('{0}','{1}','{2}')", signal, one.EntityName, timeStamp);
                //插入Entity
                new SQLiteCommand(sql, tran.Connection, tran).ExecuteNonQuery();
                CEntityName entity = selectEntity(one.EntityName);
                Dictionary<string, string> data = one.Data;
                StringBuilder buider = null;
                if (data != null && data.Count != 0)
                {
                    buider = new StringBuilder("insert into InfoTable(Key_ID,Entity_ID,Value,Version) values");
                }
                foreach (string s in data.Keys)
                {
                    buider.AppendFormat(" ({0},{1},'{2}','{3}'),", KeyMap[s], entity.Id, data[s], timeStamp);
                }
                buider.Remove(buider.Length - 1, 1);
                new SQLiteCommand(buider.ToString(), tran.Connection, tran).ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 查询CEntityName
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="command"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private CEntityName selectEntity(string entityName)
        {
            CEntityName result = null;
            string sql = string.Format("select ID,EntityName from EntityTable where EntityName='{0}'", entityName);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    result = new CEntityName();
                    result.Id = reader.GetInt32(0);
                    result.EntityName = reader.GetString(1);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
            }

            return result;
        }
        /// <summary>
        /// 查询某signal的所有KeyName
        /// </summary>
        /// <param name="Signal"></param>
        /// <param name="mycom"></param>
        /// <returns></returns>
        private List<CKeyName> SelectKeyNames(string Signal, SQLiteConnection OpenedConn)
        {
            string sql = string.Format("select ID,KeyName,Odr from KeyTable where ExcelSignal='{0}' order by Odr", Signal);
            List<CKeyName> keyNames = null;
            //需要关闭
            SQLiteDataReader reader = new SQLiteCommand(sql, OpenedConn).ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    keyNames = new List<CKeyName>();
                    while (reader.Read())
                    {
                        CKeyName one = new CKeyName();
                        one.Id = reader.GetInt32(0);
                        one.KeyName = reader.GetString(1);
                        one.Odr = reader.GetInt32(2);
                        keyNames.Add(one);
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
            return keyNames;
        }


        /// <summary>
        /// 获取KeyTable的SQL
        /// </summary>
        /// <param name="KeyNames"></param>
        /// <param name="Signal"></param>
        /// <returns></returns>
        private string InsertIntoKeyTable(List<string> KeyNames, string Signal, string timeStamp)
        {
            StringBuilder sql = new StringBuilder("insert into KeyTable(ExcelSignal,KeyName,Version) values");
            foreach (string one in KeyNames)
            {
                sql.AppendFormat(" ('{0}','{1}','{2}'),", Signal, one, timeStamp);
            }
            sql.Remove(sql.Length - 1, 1);
            return sql.ToString();
        }
        /// <summary>
        /// 查询现有的ExcelTable
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="OpenedConn"></param>
        /// <returns></returns>
        private ExcelTable SelectExcelTable(String signal)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = String.Format("select ID,CurrentFile,ExcelSignal,Total_hold,Diff_hold,History from ExcelTable where ExcelSignal = '{0}'", signal);
            SQLiteDataReader reader = new SQLiteCommand(sql, conn).ExecuteReader();
            ExcelTable result = null;
            try
            {
                while (reader.Read())
                {
                    result = new ExcelTable();
                    result.Id = reader.GetInt32(0);
                    result.CurrentFile = reader.GetString(1);
                    result.Signal = reader.GetString(2);
                    result.Total_hold = reader.GetString(3);
                    result.Diff_hold = reader.GetString(4);
                    result.History = reader.GetString(5);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
            }
            return result;
        }
        /// <summary>
        /// 插入绘图数据表
        /// </summary>
        /// <param name="sheetInfo"></param>
        private void InsertDrawData(SheetInfo sheetInfo)
        {
            string newTimeStamp = Convert.ToInt64((DateTime.Now - new DateTime(2016, 12, 14, 0, 0, 0)).TotalMilliseconds).ToString();
            //是否已经处理过
            bool hasDone = false;
            string signal = sheetInfo.ExcelTableData.Signal;
            //该连接仅用于修改数据库
            SQLiteTransaction tran = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                //事务开始
                tran = conn.BeginTransaction();
                string reset_auto_increment = "DELETE FROM sqlite_sequence";
                //主命令
                SQLiteCommand command = new SQLiteCommand(reset_auto_increment, conn, tran);
                //重置自增
                command.ExecuteNonQuery();
                //插入到ExcelTable表,并返回ID
                if (sheetInfo.ExcelTableData == null)
                {
                    throw new Exception("无效的插入数据");
                }
                //判断是否该Signal已存在
                ExcelTable exist = SelectExcelTable(signal);
                if (exist == null)
                {
                    command.CommandText = string.Format("insert into ExcelTable (CurrentFile,ExcelSignal,IsInfo,History,Version) values ('{0}', '{1}', {2},'{3}','{4}')",
                          sheetInfo.ExcelTableData.CurrentFile, signal, sheetInfo.Tag ? 1 : 0, sheetInfo.ExcelTableData.CurrentFile, newTimeStamp);
                    //插入新的数据表
                    command.ExecuteNonQuery();
                }
                else
                {
                    string[] his = exist.History.Split(';');
                    string currentFile = sheetInfo.ExcelTableData.CurrentFile;
                    foreach (string s in his)
                    {
                        if (currentFile.Equals(s))
                        {
                            hasDone = true;
                            break;
                        }
                    }
                    if (!hasDone)
                    {
                        string history = exist.History + ";" + sheetInfo.ExcelTableData.CurrentFile;
                        command.CommandText = string.Format("update ExcelTable set CurrentFile='{0}',History='{1}',Version='{3}' where ExcelSignal='{2}'",
                        sheetInfo.ExcelTableData.CurrentFile, history, signal, newTimeStamp);
                        //更新已有的数据表
                        command.ExecuteNonQuery();
                    }
                }
                if (!hasDone)
                {
                    InsertDrawDataTable(sheetInfo, tran, newTimeStamp);
                }
                tran.Commit();    //事务提交
            }
            catch (Exception)
            {
                // 事务回滚
                tran.Rollback();

                this.RollBack(newTimeStamp);

                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 插入DrawDataTable
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="command"></param>
        private void InsertDrawDataTable(SheetInfo sheetInfo, SQLiteTransaction tran, string timeStamp)
        {
            List<DrawEntityData> rows = sheetInfo.DrawRows;
            string signal = sheetInfo.ExcelTableData.Signal;
            foreach (DrawEntityData one in rows)
            {
                CEntityName entity = selectEntity(one.EntityName);
                DateTime? maxDate = null;
                if (entity == null)
                {
                    string sql = string.Format("insert into EntityTable(ExcelSignal,EntityName,Version) values ('{0}','{1}','{2}')", signal, one.EntityName, timeStamp);
                    //插入Entity
                    new SQLiteCommand(sql, tran.Connection, tran).ExecuteNonQuery();
                    entity = selectEntity(one.EntityName);
                }
                else
                {
                    string sql = string.Format("select Max(Date) from DrawTable where Entity_ID={0}", entity.Id);
                    maxDate = Convert.ToDateTime(new SQLiteCommand(sql, tran.Connection).ExecuteScalar());
                }
                List<DrawData> data = one.Data;
                StringBuilder buider = null;
                if (data != null && data.Count != 0)
                {
                    buider = new StringBuilder("insert into DrawTable(Entity_ID,Date,EntityMaxValue,EntityMidValue,EntityMinValue,Detail,Version) values");
                }
                bool hasValue = false;
                foreach (DrawData p in data)
                {
                    if (maxDate == null || p.Date > maxDate)
                    {
                        hasValue = true;
                        buider.AppendFormat(" ({0},'{1}','{2}','{3}','{4}','{5}','{6}'),", entity.Id, p.Date.ToString("yyyy-MM-dd HH:mm:ss"), p.MaxValue, p.MidValue, p.MinValue, p.Detail, timeStamp);
                    }
                }
                if (hasValue)
                {
                    buider.Remove(buider.Length - 1, 1);
                    new SQLiteCommand(buider.ToString(), tran.Connection, tran).ExecuteNonQuery();
                }
            }

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
                conn.Open();
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
                conn.Open();
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
        /// 查询InfoTable
        ///返回的数据是已分组数据
        /// </summary>
        public InfoEntityData SelectInfoData(string EntityName)
        {
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("InfoTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return null;
            }
            Dictionary<string, string> Data = new Dictionary<string, string>();
            Dictionary<string, List<string>> GroupMsg = new Dictionary<string, List<string>>();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string getDataSql = String.Format("select KeyName, Value from KeyTable kt, EntityTable et,InfoTable it where it.Key_ID = kt.ID and it.Entity_ID = et.ID and et.EntityName = '{0}'", EntityName);
            string getGroupMsgSql = String.Format("select GroupName, KeyName from GroupTable gt, KeyTable kt, EntityTable et where kt.Group_ID = gt.ID and gt.ExcelSignal = et.ExcelSignal and et.EntityName = '{0}' order by gt.ID ", EntityName);
            using (SQLiteCommand command = new SQLiteCommand(getDataSql, conn))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Data.Add(reader.GetString(0), reader.GetString(1));
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
            //查询备注
            string sql_remark = string.Format("Select Remark From EntityTable Where EntityName='{0}'", EntityName);
            string remark = null;
            using (SQLiteCommand command = new SQLiteCommand(sql_remark, conn))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            remark = reader.GetString(0);
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
            using (SQLiteCommand command = new SQLiteCommand(getGroupMsgSql, conn))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    string groupName = "";
                    while (reader.Read())
                    {
                        if (!groupName.Equals(reader.GetString(0)))
                        {
                            groupName = reader.GetString(0);
                            GroupMsg.Add(groupName, new List<string>());
                            GroupMsg[groupName].Add(reader.GetString(1));
                        }
                        else
                        {
                            GroupMsg[groupName].Add(reader.GetString(1));
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
            InfoEntityData infoData = new InfoEntityData();
            infoData.EntityName = EntityName;
            infoData.Data = Data;
            infoData.GroupMsg = GroupMsg;
            infoData.Remark = remark;
            return infoData;
        }

        /// <summary>
        /// 查询DrawDataTable
        ///查询Entity时间序列数据
        ///根据传入的起始时间查询
        /// </summary>
        public DrawEntityData SelectDrawEntityData(string EntityName, DateTime? StartDate, DateTime? EndDate)
        {
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("DrawTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return null;
            }
            DrawEntityData drawEntityData = new DrawEntityData();
            drawEntityData.EntityName = EntityName;
            drawEntityData.Data = new List<DrawData>();
            string sql = null;
            string start = null;
            string end = null;
            if (StartDate == null && EndDate == null)
            {
                sql = string.Format("select Date,EntityMaxValue,EntityMidValue,EntityMinValue,Detail from DrawTable dt, EntityTable et where dt.Entity_ID = et.ID and et.EntityName = '{0}' order by Date", EntityName);
            }
            else if (StartDate == null)
            {
                end = ((DateTime)EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                sql = String.Format("select Date,EntityMaxValue,EntityMidValue,EntityMinValue,Detail from DrawTable dt, EntityTable et where dt.Entity_ID = et.ID and et.EntityName = '{0}' and Date <= '{1}' order by Date", EntityName, end);
            }
            else if (EndDate == null)
            {
                start = ((DateTime)StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                sql = String.Format("select Date,EntityMaxValue,EntityMidValue,EntityMinValue,Detail from DrawTable dt, EntityTable et where dt.Entity_ID = et.ID and et.EntityName = '{0}' and Date >= '{1}' order by Date", EntityName, start);
            }
            else
            {
                end = ((DateTime)EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                start = ((DateTime)StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                sql = String.Format("select Date,EntityMaxValue,EntityMidValue,EntityMinValue,Detail from DrawTable dt, EntityTable et where dt.Entity_ID = et.ID and et.EntityName = '{0}' and Date between '{1}' and '{2}' order by Date", EntityName, start, end);
            }
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))  //建立执行命令语句对象
            {
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        DrawData drawData = new DrawData();

                        drawData.Date = Convert.ToDateTime(reader.GetString(0));
                        drawData.MaxValue = reader.GetFloat(1);
                        drawData.MidValue = reader.GetFloat(2);
                        drawData.MinValue = reader.GetFloat(3);
                        drawData.Detail = reader.GetString(4);
                        drawData.EntityName = EntityName;
                        //drawData.UniId = EntityName +":"+drawData.Date.ToString("yy/MM/dd-HH时");
                        drawEntityData.Data.Add(drawData);
                    }
                    reader.Close();
                    command.CommandText = string.Format("Select Total_hold,Diff_hold,Total_operator,Diff_operator From ExcelTable,EntityTable Where EntityTable.EntityName='{0}' and EntityTable.ExcelSignal=ExcelTable.ExcelSignal", EntityName);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        drawEntityData.Total_hold = reader.GetString(0);
                        drawEntityData.Diff_hold = reader.GetString(1);
                        drawEntityData.TotalOperator = reader.GetString(2);
                        drawEntityData.DiffOperator = reader.GetString(3);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    conn.Close();
                }
            }
            return drawEntityData;
        }


        /// <summary>
        /// 查询DrawDataTable
        ///查询Entity某日期的数据
        /// </summary>
        public List<DrawData> SelectDrawData(String signal, DateTime Date)
        {
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("DrawTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return null;
            }
            List<DrawData> resluts = new List<DrawData>();

            string datestr = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = String.Format("select EntityMaxValue,EntityMidValue,EntityMinValue,Detail,et.EntityName from DrawTable dt, EntityTable et where dt.Entity_ID = et.ID and et.ExcelSignal = '{0}' and dt.date = '{1}'", signal, datestr);

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))  //建立执行命令语句对象
            {
                SQLiteDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        DrawData drawData = new DrawData();
                        drawData.Date = Date;
                        drawData.MaxValue = reader.GetFloat(0);
                        drawData.MidValue = reader.GetFloat(1);
                        drawData.MinValue = reader.GetFloat(2);
                        drawData.Detail = reader.GetString(3);
                        drawData.EntityName = reader.GetString(4);
                        //drawData.UniId = drawData.EntityName + ":" + drawData.Date.ToString("yy/MM/dd-HH时");
                        resluts.Add(drawData);
                    }

                    return resluts;
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
        /// <summary>
        ///通过传入的Signal，查询与之对应的所有的测点
        ///传入的Signal应该是测量数据的signal
        ///ErrMsg:Total,TotalDiff,No,NoDiff
        /// </summary>
        public List<CEntityName> SelectAllEntitiesAndErr(string ExcelSignal, DateTime? start = null, DateTime? end = null)
        {
            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("EntityTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return null;
            }
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string select_Threshold = string.Format("Select Total_hold,Diff_hold,Total_operator,Diff_operator From ExcelTable Where ExcelSignal='{0}'", ExcelSignal);
            List<CEntityName> Entities = new List<CEntityName>();
            Dictionary<string, CEntityName> maps = new Dictionary<string, CEntityName>();

            using (SQLiteCommand command = new SQLiteCommand(select_Threshold, conn))  //建立执行命令语句对象
            {
                SQLiteDataReader reader = null;
                try
                {
                    reader = command.ExecuteReader();
                    float Total_hold1 = 0;
                    float Total_hold2 = 0;
                    float Diff_hold1 = 0;
                    float Diff_hold2 = 0;
                    string TotalOpr = null;
                    string DiffOpr = null;
                    if (reader.Read())
                    {
                        string Total_hold_str = reader.GetString(0);
                        string[] totalHolds = Total_hold_str.Split(new char[] { ',', '，' });
                        Total_hold1 = Convert.ToSingle(totalHolds[0]);
                        if (totalHolds.Length > 1)
                        {
                            Total_hold2 = Convert.ToSingle(totalHolds[1]);
                        }
                        string Diff_hold_str = reader.GetString(1);
                        string[] diffs = Diff_hold_str.Split(new char[] { ',', '，' });
                        Diff_hold1 = Convert.ToSingle(diffs[0]);
                        if (diffs.Length > 1)
                        {
                            Diff_hold2 = Convert.ToSingle(diffs[1]);
                        }
                        TotalOpr = reader.GetString(2);
                        DiffOpr = reader.GetString(3);
                    }
                    reader.Close();
                    if (TotalOpr == null || DiffOpr == null)
                    {
                        return Entities;
                    }
                    string sql_Total = null;
                    if (TotalOpr.Equals(">") || TotalOpr.Equals(">="))
                    {
                        sql_Total = String.Format("select EntityTable.ID,EntityTable.EntityName,Max(DrawTable.EntityMaxValue){2}{0} From  EntityTable,DrawTable where DrawTable.Entity_ID=EntityTable.ID and EntityTable.ExcelSignal = '{1}' ", Total_hold1, ExcelSignal, TotalOpr);
                    }
                    else if (TotalOpr.Equals("<") || TotalOpr.Equals("<="))
                    {
                        sql_Total = String.Format("select EntityTable.ID,EntityTable.EntityName,Min(DrawTable.EntityMaxValue){2}{0} From  EntityTable,DrawTable where DrawTable.Entity_ID=EntityTable.ID and EntityTable.ExcelSignal = '{1}' ", Total_hold1, ExcelSignal, TotalOpr);
                    }
                    else
                    {
                        float min = Math.Min(Total_hold1, Total_hold2);
                        float max = Math.Max(Total_hold1, Total_hold2);
                        if (TotalOpr.Equals("IN"))
                        {
                            sql_Total = String.Format("select EntityTable.ID,EntityTable.EntityName,NOT (Min(DrawTable.EntityMaxValue)>{0} or Max(DrawTable.EntityMaxValue)<{1}) From  EntityTable,DrawTable where DrawTable.Entity_ID=EntityTable.ID and EntityTable.ExcelSignal = '{2}' ", max, min, ExcelSignal);
                        }
                        else
                        {
                            sql_Total = String.Format("select EntityTable.ID,EntityTable.EntityName,NOT (Min(DrawTable.EntityMaxValue)>{0} and Max(DrawTable.EntityMaxValue)<{1}) From  EntityTable,DrawTable where DrawTable.Entity_ID=EntityTable.ID and EntityTable.ExcelSignal = '{2}' ", min, max, ExcelSignal);
                        }
                    }
                    if (start != null)
                    {
                        sql_Total += String.Format(" and DrawTable.Date>='{0}' ", ((DateTime)start).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (end != null)
                    {
                        sql_Total += String.Format(" and DrawTable.Date<='{0}' ", ((DateTime)end).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    sql_Total += " GROUP BY EntityTable.EntityName ORDER BY EntityTable.ID";
                    command.CommandText = sql_Total;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CEntityName one = new CEntityName();
                        one.Id = reader.GetInt32(0);
                        one.EntityName = reader.GetString(1);
                        one.ErrMsg = reader.GetBoolean(2) ? Res.String_Err1 : Res.String_No;
                        Entities.Add(one);
                        maps.Add(one.EntityName, one);
                    }
                    reader.Close();
                    string sql_Diff = string.Format("SELECT DrawTable.EntityMaxValue,EntityTable.EntityName From DrawTable ,EntityTable WHERE DrawTable.Entity_ID = EntityTable.ID and EntityTable.ExcelSignal='{0}' ", ExcelSignal);
                    if (start != null)
                    {
                        sql_Diff += String.Format(" and DrawTable.Date>='{0}' ", ((DateTime)start).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (end != null)
                    {
                        sql_Diff += String.Format(" and DrawTable.Date<='{0}' ", ((DateTime)end).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    sql_Diff += " Order BY EntityTable.ID,DrawTable.Date";
                    command.CommandText = sql_Diff;
                    reader = command.ExecuteReader();
                    Diff_hold1 = Math.Abs((float)Diff_hold1);
                    Diff_hold2 = Math.Abs((float)Diff_hold2);
                    float first = 0;
                    float next = 0;
                    float diff = 0;
                    bool isErr = false;
                    string entityName = null;
                    if (reader.Read())
                    {
                        first = reader.GetFloat(0);
                        entityName = reader.GetString(1);
                    }
                    while (reader.Read())
                    {
                        next = reader.GetFloat(0);
                        diff = Math.Abs((float)(next - first));
                        if (entityName.Equals(reader.GetString(1)))
                        {
                            bool result = false;
                            if (DiffOpr.Equals(">"))
                            {
                                result = diff > Diff_hold1;
                            }
                            else if (DiffOpr.Equals(">="))
                            {
                                result = diff >= Diff_hold1;
                            }
                            else if (DiffOpr.Equals("<"))
                            {
                                result = diff < Diff_hold1;
                            }
                            else if (DiffOpr.Equals("<="))
                            {
                                result = diff <= Diff_hold1;
                            }
                            else
                            {
                                float diff_min = Math.Min(Diff_hold1, Diff_hold2);
                                float diff_max = Math.Max(Diff_hold1, Diff_hold2);
                                if (DiffOpr.Equals("IN"))
                                {
                                    result = diff > diff_min && diff < diff_max;
                                }
                                else
                                {
                                    result = diff > diff_max || diff < diff_min;
                                }
                            }
                            if (result)
                            {
                                isErr = true;
                            }
                        }
                        else if (isErr)
                        {
                            if (Res.String_Err1.Equals(maps[entityName].ErrMsg))
                            {
                                maps[entityName].ErrMsg = Res.String_Err1Err2;
                            }
                            else
                            {
                                maps[entityName].ErrMsg = Res.String_Err2;
                            }
                            isErr = false;
                            entityName = reader.GetString(1);
                        }
                        else
                        {
                            entityName = reader.GetString(1);
                        }
                        first = next;
                    }
                    //最后一个
                    if (isErr)
                    {
                        if (Res.String_Err1.Equals(maps[entityName].ErrMsg))
                        {
                            maps[entityName].ErrMsg = Res.String_Err1Err2;
                        }
                        else
                        {
                            maps[entityName].ErrMsg = Res.String_Err2;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    conn.Close();
                }
            }
            return Entities;
        }
        public List<ExcelTable> SelectDrawTypes()
        {
            List<ExcelTable> result = null;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = "Select ID,CurrentFile,ExcelSignal,Total_hold,Diff_hold,History From ExcelTable Where IsInfo=0";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                SQLiteDataReader reader = null;
                try
                {
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        result = new List<ExcelTable>();
                    }
                    while (reader.Read())
                    {
                        ExcelTable one = new ExcelTable();
                        one.Id = reader.GetInt32(0);
                        one.CurrentFile = reader.GetString(1);
                        one.Signal = reader.GetString(2);
                        one.Total_hold = reader.GetString(3);
                        one.Diff_hold = reader.GetString(4);
                        one.History = reader.GetString(5);
                        result.Add(one);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    reader.Close();
                    conn.Close();
                }
            }
            return result;
        }

        public bool ModifyEntityRemark(string EntityName, string Remark)
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(EntityName) || string.IsNullOrWhiteSpace(Remark))
            {
                return false;
            }
            string sql = string.Format("Update EntityTable Set Remark='{0}' Where EntityName='{1}'", Remark, EntityName);
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                if (command.ExecuteNonQuery() > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public void RollBack(string timeStamp)
        {
            if (string.IsNullOrWhiteSpace(timeStamp))
            {
                throw new Exception("无效的参数");
            }
            StringBuilder buider = new StringBuilder();
            string sql = string.Format("Select History From ExcelTable Where Version='{0}'", timeStamp);
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                //回退ExcelTable
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                object result = command.ExecuteScalar();
                string His = null;
                if (result != null)
                {
                    His = result.ToString();
                }
                if (!string.IsNullOrWhiteSpace(His))
                {
                    string[] files = His.Split(';');
                    if (files.Count() > 1)
                    {
                        string newTimeStamp = Convert.ToInt64((DateTime.Now - new DateTime(2016, 12, 14, 0, 0, 0)).TotalMilliseconds).ToString();
                        string newHis = His.Substring(0, His.LastIndexOf(';'));
                        string newCurrentFile = files[files.Count() - 2];
                        buider.AppendFormat("Update ExcelTable Set CurrentFile='{0}',Version='{1}' Where Version='{2}';", newCurrentFile, newTimeStamp, timeStamp);
                    }
                    else if (files.Count() == 1)
                    {
                        buider.AppendFormat("Delete From ExcelTable Where Version='{0}';", timeStamp);
                    }
                }
                //InfoTable
                buider.AppendFormat("Delete From InfoTable Where Version='{0}';", timeStamp);
                //DrawTable
                buider.AppendFormat("Delete From DrawTable Where Version='{0}';", timeStamp);
                //EntityTable
                buider.AppendFormat("Delete From EntityTable Where Version='{0}';", timeStamp);
                //KeyTable
                buider.AppendFormat("Delete From KeyTable Where Version='{0}'", timeStamp);
                command.CommandText = buider.ToString();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ExcelTable> ListExcelsMessage(bool isInfo)
        {
            List<ExcelTable> result = null;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = String.Format("select ID,CurrentFile,ExcelSignal,Total_hold,Diff_hold,History,Total_operator,Diff_operator from ExcelTable where IsInfo = {0}", isInfo ? 1 : 0);
            SQLiteDataReader reader = null;
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<ExcelTable>();
                }
                while (reader.Read())
                {
                    ExcelTable one = new ExcelTable();
                    one.Id = reader.GetInt32(0);
                    one.CurrentFile = reader.GetString(1);
                    one.Signal = reader.GetString(2);
                    one.Total_hold = reader.GetString(3);
                    one.Diff_hold = reader.GetString(4);
                    one.History = reader.GetString(5);
                    one.TotalOperator = reader.GetString(6);
                    one.DiffOperator = reader.GetString(7);
                    result.Add(one);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
            return result;
        }

        public List<Group> loadGroupForAExcel(string signal)
        {
            List<Group> groups = new List<Group>();
            string sql = string.Format("select ID,GroupName from GroupTable where ExcelSignal='{0}'", signal);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SQLiteDataReader reader = null;
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Group one = new Group();
                    one.Id = reader.GetInt32(0);
                    one.GroupName = reader.GetString(1);
                    groups.Add(one);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
            Group newOne = new Group();
            newOne.Id = -1;
            newOne.GroupName = "新建分组";
            groups.Add(newOne);
            return groups;
        }

        public List<CKeyName> loadKeyNameForExcelAndGroup(string signal, int Group_id = -1)
        {
            List<CKeyName> result = null;
            string sql = string.Format("Select ID,KeyName,Group_ID={1} from KeyTable where ExcelSignal='{0}'", signal, Group_id);
            SQLiteDataReader reader = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<CKeyName>();
                }
                while (reader.Read())
                {
                    CKeyName one = new CKeyName();
                    one.Id = reader.GetInt32(0);
                    one.KeyName = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        one.IsCheck = reader.GetBoolean(2);
                    }
                    result.Add(one);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
            return result;
        }

        public bool AddKeysToGroup(int Group_ID, List<int> Key_Ids)
        {
            bool result = false;
            SQLiteTransaction tran = null;
            SQLiteDataReader reader = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                tran = conn.BeginTransaction();
                string select_sql = string.Format("Select ID from KeyTable where Group_ID={0};", Group_ID);
                SQLiteCommand command = new SQLiteCommand(select_sql, conn, tran);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    StringBuilder buider1 = new StringBuilder();
                    while (reader.Read())
                    {
                        buider1.Append(reader.GetInt32(0)).Append(",");
                    }
                    reader.Close();
                    buider1.Remove(buider1.Length - 1, 1);
                    string sql_reset = string.Format("update KeyTable set Group_ID=NULL where ID in ({0});", buider1.ToString());
                    command.CommandText = sql_reset;
                    command.ExecuteNonQuery();
                }
                else
                {
                    reader.Close();
                }

                if (Key_Ids != null && Key_Ids.Count != 0)
                {
                    StringBuilder buider = new StringBuilder();
                    foreach (int i in Key_Ids)
                    {
                        buider.Append(i).Append(",");
                    }
                    buider.Remove(buider.Length - 1, 1);
                    command.CommandText = string.Format("update KeyTable set Group_ID={0} where ID in ({1});", Group_ID, buider.ToString()); ;
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                result = true;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public bool ModifyThreshold(string signal, string Total_hold, string Diff_hold, string TotalOpr, string DiffOpr)
        {
            bool flag = false;
            string sql = String.Format("Update ExcelTable set Total_hold = '{0}', Diff_hold = '{1}',Total_operator='{3}',Diff_operator='{4}' where ExcelSignal = '{2}'", Total_hold, Diff_hold, signal, TotalOpr, DiffOpr);
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SQLiteCommand mycom = new SQLiteCommand(sql, conn);
                mycom.ExecuteNonQuery();
                flag = true;
            }
            catch (Exception)
            {
                MessageBox.Show("修改出错");
            }
            finally
            {
                conn.Close();
            }
            return flag;
        }
        public bool DeleteGroup(int Group_ID)
        {
            bool result = false;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                string sql = string.Format("Update keyTable Set Group_ID=NULL Where Group_ID={0};Delete From GroupTable Where ID={0}", Group_ID);
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
        public Group AddNewGroup(string Signal, string GroupName)
        {
            Group result = null;
            string sql = string.Format("DELETE FROM sqlite_sequence;Insert into GroupTable(ExcelSignal,GroupName) values ('{0}','{1}')", Signal, GroupName);
            string select_sql = string.Format("Select ID from GroupTable where GroupName='{0}' and ExcelSignal='{1}'", GroupName, Signal);
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                command.CommandText = select_sql;
                int id = Convert.ToInt32(command.ExecuteScalar());
                result = new Group();
                result.Id = id;
                result.GroupName = GroupName;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public List<CEntityName> SelectAllEntitiesAndErrIgnoreSignal()
        {

            //判断是否创建该查询的表（一定要先打开数据库）
            if (!isExist("EntityTable", "table"))
            {
                MessageBox.Show("本地数据库不存在，建议更新本地数据库！");
                return null;
            }
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string select_Signal = "Select ExcelSignal From ExcelTable Where IsInfo=0 ";
            List<CEntityName> Entities = new List<CEntityName>();
            Dictionary<string, CEntityName> maps = new Dictionary<string, CEntityName>();
            List<string> Signals = new List<string>();

            using (SQLiteCommand command = new SQLiteCommand(select_Signal, conn))  //建立执行命令语句对象
            {
                SQLiteDataReader reader = null;
                try
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Signals.Add(reader.GetString(0));
                    }
                    reader.Close();
                    foreach (string signal in Signals)
                    {
                        command.CommandText = string.Format("Select Total_hold,Diff_hold,Total_operator,Diff_operator From ExcelTable Where ExcelSignal='{0}'", signal);
                        reader = command.ExecuteReader();
                        float? Total_hold = null;
                        float? Diff_hold = null;
                        string TotalOpr = null;
                        string DiffOpr = null;
                        if (reader.Read())
                        {
                            Total_hold = Convert.ToSingle(reader.GetString(0));
                            Diff_hold = Convert.ToSingle(reader.GetString(1));
                            TotalOpr = reader.GetString(2);
                            DiffOpr = reader.GetString(3);

                        }
                        reader.Close();
                        if (Total_hold == null || Diff_hold == null || TotalOpr == null || DiffOpr == null)
                        {
                            throw new Exception("无效的阈值或操作符");
                        }
                        string sql_Total = null;
                        if (TotalOpr.Equals(">=") || TotalOpr.Equals(">"))
                        {
                            sql_Total = String.Format("select EntityTable.ID,EntityTable.EntityName,Max(DrawTable.EntityMaxValue){2}{0},Max(DrawTable.EntityMaxValue),Min(DrawTable.EntityMaxValue) From  EntityTable,DrawTable where DrawTable.Entity_ID=EntityTable.ID and EntityTable.ExcelSignal = '{1}' GROUP BY EntityTable.EntityName ORDER BY EntityTable.ID", Total_hold, signal, TotalOpr);
                        }
                        else
                        {
                            sql_Total = String.Format("select EntityTable.ID,EntityTable.EntityName,Min(DrawTable.EntityMaxValue){2}{0},Max(DrawTable.EntityMaxValue),Min(DrawTable.EntityMaxValue) From  EntityTable,DrawTable where DrawTable.Entity_ID=EntityTable.ID and EntityTable.ExcelSignal = '{1}' GROUP BY EntityTable.EntityName ORDER BY EntityTable.ID", Total_hold, signal, TotalOpr);
                        }
                        command.CommandText = sql_Total;
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            CEntityName one = new CEntityName();
                            one.Id = reader.GetInt32(0);
                            one.EntityName = reader.GetString(1);
                            one.ErrMsg = reader.GetBoolean(2) ? "Total" : "No";
                            one.maxValue = reader.GetString(3);
                            one.minValue = reader.GetString(4);
                            Entities.Add(one);
                            maps.Add(one.EntityName, one);
                        }
                        reader.Close();
                        string sql_Diff = string.Format("SELECT DrawTable.EntityMaxValue,EntityTable.EntityName From DrawTable ,EntityTable WHERE DrawTable.Entity_ID = EntityTable.ID and EntityTable.ExcelSignal='{0}' Order BY EntityTable.ID,DrawTable.Date", signal);
                        command.CommandText = sql_Diff;
                        reader = command.ExecuteReader();
                        Diff_hold = Math.Abs((float)Diff_hold);
                        float first = 0;
                        float next = 0;
                        float diff = 0;
                        bool isErr = false;
                        string entityName = null;
                        if (reader.Read())
                        {
                            first = reader.GetFloat(0);
                            entityName = reader.GetString(1);
                        }
                        while (reader.Read())
                        {
                            next = reader.GetFloat(0);
                            diff = Math.Abs((float)(next - first));
                            if (entityName.Equals(reader.GetString(1)))
                            {
                                bool result = false;
                                if (DiffOpr.Equals(">"))
                                {
                                    result = diff > Diff_hold;
                                }
                                else if (DiffOpr.Equals(">="))
                                {
                                    result = diff >= Diff_hold;
                                }
                                else if (DiffOpr.Equals("<"))
                                {
                                    result = diff < Diff_hold;
                                }
                                else if (DiffOpr.Equals("<="))
                                {
                                    result = diff <= Diff_hold;
                                }
                                if (result)
                                {
                                    isErr = true;
                                }
                            }
                            else if (isErr)
                            {
                                maps[entityName].ErrMsg += "Diff";
                                isErr = false;
                                entityName = reader.GetString(1);
                            }
                            else
                            {
                                entityName = reader.GetString(1);
                            }
                            first = next;
                        }
                        //最后一个
                        if (isErr)
                        {
                            maps[entityName].ErrMsg += "Diff";
                        }
                        reader.Close();
                        maps.Clear();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    conn.Close();
                }
            }
            return Entities;
        }

        public ExcelTable SelectADrawType(string EntityName)
        {
            ExcelTable result = null;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            string sql = String.Format("Select ExcelTable.ID,CurrentFile,ExcelTable.ExcelSignal,Total_hold,Diff_hold,History From ExcelTable,EntityTable Where ExcelTable.IsInfo=0 and EntityTable.ExcelSignal=ExcelTable.ExcelSignal and EntityTable.EntityName='{0}'", EntityName);
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                SQLiteDataReader reader = null;
                try
                {
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = new ExcelTable();
                        result.Id = reader.GetInt32(0);
                        result.CurrentFile = reader.GetString(1);
                        result.Signal = reader.GetString(2);
                        result.Total_hold = reader.GetString(3);
                        result.Diff_hold = reader.GetString(4);
                        result.History = reader.GetString(5);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    reader.Close();
                    conn.Close();
                }
            }
            return result;
        }
    }
}
