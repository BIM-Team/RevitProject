using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace Revit.Addin.RevitTooltip.Util
{
    public class SQLiteHelper:IDisposable
    {
        //单例模式
        private static SQLiteHelper sqliteHelper;

        private MysqlUtil mysql;

        //连接
        private SQLiteConnection conn;

        
        private string connectionString = string.Empty;
        private static string dbPath = "SQLiteDB.db3";

        //构造函数
        private SQLiteHelper()
        {
            this.connectionString = "Data Source=" + dbPath;
            this.conn = new SQLiteConnection(this.connectionString);
        }
        //单例模式
        public static SQLiteHelper CreateInstance()
        {
            if (null == sqliteHelper)
            {
                sqliteHelper = new SQLiteHelper();
            }
            return sqliteHelper;
        }
        //建立SQLite数据库连接
        public void OpenConnect()
        {
            if (null == conn)
            {
                this.connectionString = "Data Source=" + dbPath;
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

        //定时更新SQLite数据库
        public void UpdateDBonTime()
        {
           
        }

        //如果本地有SQLiteDB.db3数据库，先删除，再创建，并从Mysql数据中导入数据 
        public void UpdateDB()
        {
            if (conn.State == ConnectionState.Open) {
                Close();
            }
            if (System.IO.File.Exists(dbPath))
            {
                System.IO.File.Delete(dbPath);
                Console.WriteLine("成功删除SQLite数据库");
            }
               
            //判断数据库是否打开           
            if (conn.State != ConnectionState.Open)
            {                
                OpenConnect();
            }
            mysql = MysqlUtil.CreateInstance();            

            using (DbTransaction transaction = conn.BeginTransaction())
            {                
                CreateDB();
                transaction.Commit();
            }
        }
        //创建SQLite数据库文件
        public void CreateDB()
        {

            using (SQLiteCommand command = new SQLiteCommand(this.conn))
            {
                if (!isExist("TypeTable", "table"))
                {
                    command.CommandText = "CREATE TABLE TypeTable(ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,TYPENAME VARCHAR(20) NOT NULL, TABLEREMARK VARCHAR(100) )";
                    command.ExecuteNonQuery();                   
                }
                InsertTypeTable();

                if (!isExist("FrameTable", "table"))
                {
                    command.CommandText = "CREATE TABLE FrameTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_TYPE integer NOT NULL REFERENCES TypeTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, COLUMNNAME VARCHAR(20) NOT NULL )";
                    command.ExecuteNonQuery();                   
                }
                InsertFrameTable();

                if (!isExist("EntityTable", "table"))
                {
                    command.CommandText = "CREATE TABLE EntityTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_TYPE integer NOT NULL REFERENCES TypeTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ENTITY VARCHAR(20) NOT NULL, ENTITYCOLUMN VARCHAR(20) DEFAULT '测点编号', ENTITYREMARK VARCHAR(100))";
                    command.ExecuteNonQuery();                    
                }
                InsertEntityTable();

                if (!isExist("BaseComponentTable", "table"))
                {
                    command.CommandText = "CREATE TABLE BaseComponentTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_FRAME integer NOT NULL REFERENCES FrameTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ID_ENTITY integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, VALUE VARCHAR(20))";
                    command.ExecuteNonQuery();                
                }
                InsertBaseComponentTable();

                if (!isExist("wylWallTable", "table"))
                {
                    command.CommandText = "CREATE TABLE wylWallTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_FRAME integer NOT NULL REFERENCES FrameTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ID_ENTITY integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, VALUE VARCHAR(20))";
                    command.ExecuteNonQuery();                    
                }
                InsertWallTable();

                if (!isExist("InclinationTable", "table"))
                {
                    command.CommandText = "CREATE TABLE InclinationTable(ID integer PRIMARY KEY AUTOINCREMENT, ID_FRAME integer NOT NULL REFERENCES FrameTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, ID_ENTITY integer NOT NULL REFERENCES EntityTable(ID) ON DELETE CASCADE ON UPDATE NO ACTION, DATE VARCHAR(20), VALUE VARCHAR(10))";
                    command.ExecuteNonQuery();                  
                }
                InsertInclinationTable();

                if (!isExist("BaseWallView", "view"))
                {
                    command.CommandText = "Create view BaseWallView (Entity, ColumnName, Value, FrameID) AS Select et.ENTITY, ft.COLUMNNAME, bt.VALUE, ft.ID from  EntityTable et, FrameTable ft, BaseComponentTable bt Where et.ID_TYPE = ft.ID_TYPE and bt.ID_ENTITY = et.ID and bt.ID_FRAME = ft.ID "
                                          +"UNION Select et.ENTITY, ft.COLUMNNAME, wt.VALUE, ft.ID from  EntityTable et, FrameTable ft, wylWallTable wt Where et.ID_TYPE = ft.ID_TYPE and wt.ID_ENTITY = et.ID and wt.ID_FRAME = ft.ID ";
                    command.ExecuteNonQuery();  
                }
            }

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

        private void InsertTypeTable()
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
        private void InsertFrameTable()
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
        private void InsertEntityTable()
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
        private void InsertBaseComponentTable()
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
        private void InsertWallTable()
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
        private void InsertInclinationTable()
        {
            //读取InclinationTable表数据并插入
            MySqlDataReader reader = this.mysql.SelectTable("InclinationTable");
            string sql = "INSERT OR IGNORE INTO InclinationTable(ID,ID_FRAME,ID_ENTITY,DATE,VALUE)values(@ID,@ID_FRAME,@ID_ENTITY,@DATE,@VALUE)";
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        DateTime dateTime = reader.GetDateTime(3);
                        string datestr = dateTime.ToString("yyyy-MM-dd");
                        if (dateTime.Hour >= 12)
                        {
                            datestr += "pm";
                        }
                        SQLiteParameter[] paramenters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@ID", reader.GetInt64(0)),
                            new SQLiteParameter("@ID_FRAME", reader.GetInt64(1)),
                            new SQLiteParameter("@ID_ENTITY", reader.GetInt64(2)),                           
                            new SQLiteParameter("@DATE", datestr),  //reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss")
                            new SQLiteParameter("@VALUE", reader.GetString(4))
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
               // transaction.Commit();                     
            return affectedRows;
        }

        ////测试查询
        //public Dictionary<int, string> selecttable()
        //{
        //    Dictionary<int, string> data = new Dictionary<int, string>();
        //    string sql = "select ID,DATE,VALUE from InclinationTable order by ID ";
        //    if (conn.State != ConnectionState.Open)
        //    {
        //        OpenConnect();
        //    }
        //    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
        //    {
        //        SQLiteDataReader reader = command.ExecuteReader();
        //        try
        //        {
        //            while (reader.Read())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    data.Add(reader.GetInt32(0), reader.GetString(1));
        //                }
        //            }
        //            return data;

        //        }
        //        catch (Exception e)
        //        {
        //            throw e;
        //        }
        //        finally
        //        {
        //            reader.Close();  //关闭
        //        }
        //    }
            
        //}

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

            //用cast()函数将string类型转换为浮点数，在用MAX()函数
            String sql = String.Format("select et.ENTITY, it.DATE, MAX(cast(it.VALUE as DECIMAL(5,2))) from InclinationTable it,EntityTable et,TypeTable tt "
                          + " where it.ID_ENTITY=et.ID and et.ID_TYPE=tt.ID and tt.TYPENAME= '测斜汇总' and et.ENTITY='{0}' "
                          + " group by ENTITY,DATE "
                          + " order by ENTITY,DATE ", entity);

            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
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

        //根据Entity名，查询基础数据和地墙数据， 
        public List<Revit.Addin.RevitTooltip.App.ParameterData> SelectEntityData(string entity)
        {
            List<Revit.Addin.RevitTooltip.App.ParameterData> list = new List<Revit.Addin.RevitTooltip.App.ParameterData>();
            list.Add(new Revit.Addin.RevitTooltip.App.ParameterData("测点编号", entity));
            string sql = String.Format("select BaseWallView.ColumnName, BaseWallView.Value,BaseWallView.FrameID from BaseWallView where BaseWallView.Entity = '{0}' order by BaseWallView.FrameID ", entity);
            string sql2 = String.Format("select ENTITYREMARK from EntityTable where ENTITY = '{0}'", entity);
            //判断数据库是否打开
            if (conn.State != ConnectionState.Open)
            {
                OpenConnect();
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
       
    }
}
