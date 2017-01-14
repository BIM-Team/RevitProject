using System;
using System.Collections.Generic;
using System.Linq;
using Revit.Addin.RevitTooltip.Dto;
using MySql.Data.MySqlClient;
using Revit.Addin.RevitTooltip.Intface;
using System.Text;
using System.Windows;

namespace Revit.Addin.RevitTooltip.Impl
{
    public class MysqlUtil : IMysqlUtil
    {
        /// <summary>
        /// 保存一个实例供其他类使用
        /// </summary>
        private static MysqlUtil _mysql=null;
        /// <summary>
        /// 获取一个实例供其他类使用
        /// </summary>
        public static MysqlUtil Instance {
            get { return _mysql; }
        }
        /// <summary>
        /// 保存连接信息
        /// 后续用这连接信息创建连接，不是每个类创建一个连接
        /// </summary>
        private string connectMessage;
        ///// <summary>
        ///// 返回连接信息
        ///// </summary>
        private bool isReady = false;
       
        /// <summary>
        /// 返回连接信息
        /// </summary>
        public string ConnectionMessage {
            get { return this.connectMessage; }
                }
        /// <summary>
        /// 返回此时Mysql的可用状态
        /// </summary>
        public bool IsReady
        {
            get
            {
                return isReady;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="settings"></param>
        internal MysqlUtil(RevitTooltip settings)
        {
            this.connectMessage = "server=" + settings.DfServer +
                ";user=" + settings.DfUser +
                ";database=" + settings.DfDB +
                ";port=" + settings.DfPort +
                ";password=" + settings.DfPassword +
                ";charset=" + settings.DfCharset;
            //测试Mysql的可用状态
            this.isReady=CheckReady();
            _mysql = this;
        }
        /// <summary>
        /// 初始化默认本地连接
        /// </summary>
        private MysqlUtil()
        {
            this.connectMessage = "server= 127.0.0.1 ;user= root; database= hzj ;port= 3306;password= root;charset= utf8";
            //测试Mysql的可用状态
            this.isReady = CheckReady();
            _mysql = this;
        }
        /// <summary>
        /// 查看数据库能否链接
        /// </summary>
        /// <returns></returns>
        private bool CheckReady()
        {
            bool result = false;
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            try
            {
                conn.Open();
                result = true;
            }
            catch (Exception) { }
            finally {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 插入一个SheetInfo，只插入以前表中不存在的数据
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <returns></returns>
        public void InsertSheetInfo(SheetInfo sheetInfo)
        {
            if (sheetInfo == null) {
                throw new Exception("无效的插入数据");
            }
            if (sheetInfo.Tag)
            {
                InsertInfoData(sheetInfo);
            }
            else {
                InsertDrawData(sheetInfo);
            }
        }
        /// <summary>
        /// 插入Info数据
        /// </summary>
        /// <param name="sheetInfo"></param>
        private void InsertInfoData(SheetInfo sheetInfo)
        {
            string newTimeStamp = Convert.ToInt64((DateTime.Now - new DateTime(2016, 12, 14, 0, 0, 0)).TotalMilliseconds).ToString();
            //是否已经处理过
            bool hasDone = false;
            string signal = sheetInfo.ExcelTableData.Signal;
            //该连接仅用于修改数据库
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            string reset_auto_increment = "alter table ExcelTable auto_increment =1;"
                         + "alter table KeyTable auto_increment =1;"
                         + "alter table EntityTable auto_increment =1;"
                         + "alter table GroupTable auto_increment =1;"
                         + "alter table InfoTable auto_increment =1;"
                         + "alter table DrawTable auto_increment =1";
            MySqlTransaction tran=null;
            try
            {
            conn.Open();
            //事务开始
            tran = conn.BeginTransaction();
                //重置自增
                new MySqlCommand(reset_auto_increment, conn, tran).ExecuteNonQuery();
                //插入到ExcelTable表,并返回ID
                if (sheetInfo.ExcelTableData == null)
                {
                    throw new Exception("无效的插入数据");
                }
                //判断是否该Signal已存在
                ExcelTable exist = SelectExcelTable(signal, conn);
                if (exist == null)
                {
                    //插入ExcelTable
                     new MySqlCommand(string.Format("insert into ExcelTable (CurrentFile,ExcelSignal,IsInfo,History,Version) values ('{0}', '{1}', {2},'{3}','{4}')",
                          sheetInfo.ExcelTableData.CurrentFile, signal, sheetInfo.Tag, sheetInfo.ExcelTableData.CurrentFile,newTimeStamp),tran.Connection,tran).ExecuteNonQuery();
                    exist = SelectExcelTable(signal, conn);
                    //插入表结构KeyNames
                    new MySqlCommand(InsertIntoKeyTable(sheetInfo.KeyNames, sheetInfo.ExcelTableData.Signal, newTimeStamp),tran.Connection,tran).ExecuteNonQuery();
                }
                else
                {
                    string[] his = exist.History.Split(';');
                    string currentFile = sheetInfo.ExcelTableData.CurrentFile;
                    //判断是否已经做过处理
                    foreach (string s in his) {
                        if (currentFile.Equals(s)) {
                            hasDone = true;
                            break;
                        }
                    }
                    //对于没有处理过的，添加到History中
                    if (!hasDone) {
                    string history = exist.History + ";" + sheetInfo.ExcelTableData.CurrentFile;
                    //更新已有的数据表
                    new MySqlCommand(string.Format("update ExcelTable set CurrentFile='{0}',History='{1}',Version='{3}' where ExcelSignal='{2}'",
                       sheetInfo.ExcelTableData.CurrentFile, history, signal,newTimeStamp), tran.Connection, tran).ExecuteNonQuery();
                    }
                }
                    if (!hasDone) {
                    List<CKeyName> inMysqls = SelectKeyNames(signal, conn);
                if (inMysqls == null || inMysqls.Count == 0) {
                    throw new Exception("数据异常");
                }
                //构造Map
                Dictionary<string, int> KeyMap = new Dictionary<string, int>();
                foreach (CKeyName one in inMysqls) {
                    KeyMap.Add(one.KeyName,one.Id);
                }
                    InsertInfoTable(sheetInfo, KeyMap,tran,newTimeStamp);
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
        /// 插入绘图数据表
        /// </summary>
        /// <param name="sheetInfo"></param>
        private void InsertDrawData(SheetInfo sheetInfo)
        {
            //时间戳
            string newTimeStamp = Convert.ToInt64((DateTime.Now - new DateTime(2016, 12, 14, 0, 0, 0)).TotalMilliseconds).ToString();
            //是否已经处理过
            bool hasDone = false;
            string signal = sheetInfo.ExcelTableData.Signal;
            //该连接仅用于修改数据库
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            MySqlTransaction tran = null;
            try
            {
                conn.Open();
            //事务开始
            tran = conn.BeginTransaction();
            string reset_auto_increment = "alter table ExcelTable auto_increment =1;"
                         + "alter table KeyTable auto_increment =1;"
                         + "alter table EntityTable auto_increment =1;"
                         + "alter table GroupTable auto_increment =1;"
                         + "alter table InfoTable auto_increment =1;"
                         + "alter table DrawTable auto_increment =1";
            //主命令
            MySqlCommand command = new MySqlCommand(reset_auto_increment, conn, tran);
                //重置自增
                command.ExecuteNonQuery();
                //插入到ExcelTable表,并返回ID
                if (sheetInfo.ExcelTableData == null)
                {
                    throw new Exception("无效的插入数据");
                }
                //判断是否该Signal已存在
                ExcelTable exist = SelectExcelTable(signal, tran.Connection);
                if (exist == null)
                {
                    command.CommandText = string.Format("insert into ExcelTable (CurrentFile,ExcelSignal,IsInfo,History,Version) values ('{0}', '{1}', {2},'{3}','{4}')",
                          sheetInfo.ExcelTableData.CurrentFile, signal, sheetInfo.Tag, sheetInfo.ExcelTableData.CurrentFile,newTimeStamp);
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
                    if (!hasDone) {
                        string history = exist.History + ";" + sheetInfo.ExcelTableData.CurrentFile;
                        command.CommandText = string.Format("update ExcelTable set CurrentFile='{0}',History='{1},Version='{4}' where ExcelSignal='{3}'",
                        sheetInfo.ExcelTableData.CurrentFile, history, signal,newTimeStamp);
                    //更新已有的数据表
                    command.ExecuteNonQuery();
                    }
                }
                if (!hasDone)
                {
                    InsertDrawDataTable(sheetInfo, tran,newTimeStamp);
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
        /// 返回插入到KeyTable的Sql语句
        /// </summary>
        /// <param name="KeyNames"></param>
        /// <param name="Signal"></param>
        /// <returns></returns>
        private string InsertIntoKeyTable(List<string> KeyNames, string Signal,string timeStamp)
        {
            if (KeyNames == null || KeyNames.Count == 0) {
                throw new Exception("无效的参数");
            }
            StringBuilder sql = new StringBuilder("insert into KeyTable(ExcelSignal,KeyName,Version) values");
            foreach (string one in KeyNames)
            {
                sql.AppendFormat(" ('{0}','{1}','{2}'),", Signal, one, timeStamp);
            }
            sql.Remove(sql.Length-1,1);
            return sql.ToString();
        }
        
        /// <summary>
        /// 插入EntityInfo数据
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="KeyMap"></param>
        /// <param name="command"></param>
        private void InsertInfoTable(SheetInfo sheetInfo, Dictionary<string, int> KeyMap,MySqlTransaction tran,string timeStamp)
        {
            if (sheetInfo.InfoRows == null || sheetInfo.InfoRows.Count == 0) {

                throw new Exception("无效的数据");
            }
            List<InfoEntityData> rows = sheetInfo.InfoRows;
            string signal = sheetInfo.ExcelTableData.Signal;

            foreach (InfoEntityData one in rows) {
                string sql = string.Format("insert into EntityTable(ExcelSignal,EntityName,Version) values ('{0}','{1}','{2}')", signal,one.EntityName,timeStamp);
                //插入Entity
                new MySqlCommand(sql,tran.Connection,tran).ExecuteNonQuery();
                CEntityName entity = selectEntity(one.EntityName, tran.Connection);
                Dictionary<string, string> data = one.Data;
                StringBuilder buider = null;
                if (data != null && data.Count != 0) {
                    buider = new StringBuilder("insert into InfoTable(Key_ID,Entity_ID,Value,Version) values");
                }
                foreach (string s in data.Keys) {
                    buider.AppendFormat(" ({0},{1},'{2}','{3}'),", KeyMap[s], entity.Id, data[s],timeStamp);
                }
                buider.Remove(buider.Length - 1, 1);
                new MySqlCommand(buider.ToString(),tran.Connection,tran).ExecuteNonQuery();
            }
         }

        /// <summary>
        /// 查询CEntityName
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="command"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private CEntityName selectEntity(string entityName,MySqlConnection OpenedConn,bool err=false) {
            CEntityName result = null;
            //不需要Err信息
            if (!err) {
            string sql = string.Format("select ID,EntityName from EntityTable where EntityName='{0}'", entityName);
                MySqlCommand command = new MySqlCommand(sql, OpenedConn);
            MySqlDataReader reader= command.ExecuteReader();
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
                finally {
                    reader.Close();
                }
            }
                return result;
        }
        /// <summary>
        /// 插入DrawDataTable
        /// </summary>
        /// <param name="sheetInfo"></param>
        /// <param name="command"></param>
        private void InsertDrawDataTable(SheetInfo sheetInfo,MySqlTransaction tran,string timeStamp)
        {
            List<DrawEntityData> rows = sheetInfo.DrawRows;
            string signal = sheetInfo.ExcelTableData.Signal;
            foreach (DrawEntityData one in rows) {
                CEntityName entity = selectEntity(one.EntityName, tran.Connection);
                DateTime ?maxDate = null;
                if (entity == null)
                {
                    string sql = string.Format("insert into EntityTable(ExcelSignal,EntityName,Version) values ('{0}','{1}','{2}')", signal, one.EntityName,timeStamp);
                    //插入Entity
                    new MySqlCommand(sql,tran.Connection,tran).ExecuteNonQuery();
                    entity = selectEntity(one.EntityName, tran.Connection);
                }
                else {
                string sql = string.Format("select Max(Date) from DrawTable where Entity_ID={0}", entity.Id);
                maxDate = Convert.ToDateTime(new MySqlCommand(sql,tran.Connection).ExecuteScalar());
                }
                List<DrawData> data = one.Data;
                StringBuilder buider = null;
                if (data != null&&data.Count!=0) {
                    buider = new StringBuilder("insert into DrawTable(Entity_ID,Date,EntityMaxValue,EntityMidValue,EntityMinValue,Detail,Version) values");
                }
                foreach (DrawData p in data) {
                    if (maxDate==null||p.Date > maxDate) {
                    buider.AppendFormat(" ({0},'{1}','{2}','{3}','{4}','{5}','{6}'),",entity.Id,p.Date,p.MaxValue,p.MidValue,p.MinValue,p.Detail,timeStamp);
                    }
                }
                buider.Remove(buider.Length - 1, 1);
                new MySqlCommand(buider.ToString(), tran.Connection, tran).ExecuteNonQuery();
            }

        }
        /// <summary>
        /// 查询某signal的所有KeyName
        /// </summary>
        /// <param name="Signal"></param>
        /// <param name="mycom"></param>
        /// <returns></returns>
        private List<CKeyName> SelectKeyNames(string  Signal,MySqlConnection OpenedConn)
        {
            string sql = string.Format("select ID,KeyName,Odr from KeyTable where ExcelSignal='{0}' order by Odr", Signal);
            List<CKeyName> keyNames = null;
            //需要关闭
            MySqlDataReader reader = new MySqlCommand(sql, OpenedConn).ExecuteReader(); 
            try
            {
                if (reader.HasRows) {
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
        
      
        
        private ExcelTable SelectExcelTable(String signal,MySqlConnection OpenedConn)
        {
            string sql = String.Format("select ID,CurrentFile,ExcelSignal,Total_hold,Diff_hold,History from ExcelTable where ExcelSignal = '{0}'", signal);
            MySqlDataReader reader=new MySqlCommand(sql, OpenedConn).ExecuteReader();
            ExcelTable result = null;
            try
            {
                while (reader.Read())
                {
                    result = new ExcelTable();
                    result.Id = reader.GetInt32(0);
                    result.CurrentFile = reader.GetString(1);
                    result.Signal = reader.GetString(2);
                    result.Total_hold = reader.GetFloat(3);
                    result.Diff_hold = reader.GetFloat(4);
                    result.History = reader.GetString(5);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                reader.Close();
            }
            return result;
        }
        
       /// <summary>
       /// 修改阈值
       /// </summary>
       /// <param name="signal"></param>
       /// <param name="Total_hold"></param>
       /// <param name="Diff_hold"></param>
       /// <returns></returns>
        public bool ModifyThreshold(string signal, float Total_hold, float Diff_hold)
        {
            
            bool flag = false;
            string sql= String.Format("Update ExcelTable set Total_hold = '{0}', Diff_hold = '{1}' where ExcelSignal = '{2}'", Total_hold, Diff_hold, signal);
           
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            MySqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                MySqlCommand mycom = new MySqlCommand(sql,conn,tran);
                mycom.ExecuteNonQuery();
                tran.Commit();
                flag = true;
            }
            catch (Exception)
            {
                tran.Rollback();
            }
            finally {
                conn.Close();
                conn.Dispose();
            }
            return flag;
        }
        /// <summary>
        /// 查询当前的Excel表信息
        /// </summary>
        /// <param name="isInfo"></param>
        /// <returns></returns>
        public List<ExcelTable> ListExcelsMessage(bool isInfo)
        {
            List<ExcelTable> result = null;
            MySqlConnection conn = new MySqlConnection(this.connectMessage);

            string sql = String.Format("select ID,CurrentFile,ExcelSignal,Total_hold,Diff_hold,History from ExcelTable where IsInfo = {0}", isInfo);
            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = command.ExecuteReader();
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
                    one.Total_hold = reader.GetFloat(3);
                    one.Diff_hold = reader.GetFloat(4);
                    one.History = reader.GetString(5);
                    result.Add(one);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        public List<Group> loadGroupForAExcel(string signal)
        {
            List<Group> groups = new List<Group>() ;
            Group newOne = new Group();
            //newOne.Id = -1;
            //newOne.GroupName = "新建";
            //groups.Add(newOne);
            string sql = string.Format("select ID,GroupName from GroupTable where ExcelSignal='{0}'", signal);
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(sql, conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Group one = new Group();
                    one.Id = reader.GetInt32(0);
                    one.GroupName = reader.GetString(1);
                    groups.Add(one);
                }

            }
            catch (Exception) {
                throw;
            }
            finally
            {
                reader.Close();
                conn.Close();
                conn.Dispose();
            }
            return groups;
        }


        public List<CKeyName> loadKeyNameForExcelAndGroup(string signal, int Group_id=-1)
        {
            List<CKeyName> result = null;
            string sql = string.Format("Select ID,KeyName,Group_ID={1} from KeyTable where ExcelSignal='{0}'", signal, Group_id);
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(sql, conn);
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
            finally {
                reader.Close();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

       

        public Group AddNewGroup(string Signal, string GroupName)
        {
            Group result = null;
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            MySqlTransaction tran = null;
            string sql = string.Format("alter table GroupTable auto_increment =1;Insert into GroupTable(ExcelSignal,GroupName) values ('{0}','{1}')", Signal, GroupName);
            string select_sql = string.Format("Select ID from GroupTable where GroupName='{0}' and ExcelSignal='{1}'", GroupName,Signal);
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                MySqlCommand command = new MySqlCommand(sql, conn, tran);
                command.ExecuteNonQuery();
                tran.Commit();
                command.CommandText = select_sql;
                int id = Convert.ToInt32(command.ExecuteScalar());
                result = new Group();
                result.Id = id;
                result.GroupName = GroupName;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
            finally {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        public bool DeleteGroup(int Group_ID)
        {
            bool result = false;
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            try
            {
                conn.Open();
                string sql = string.Format("Update keyTable Set Group_ID=NULL Where Group_ID={0};Delete From GroupTable Where ID={0}", Group_ID);
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                conn.Close();
                conn.Dispose();
            }
            return result;
        }


        public bool AddKeysToGroup(int Group_ID, List<int> Key_Ids)
        {
            bool result = false;
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            MySqlTransaction tran = null;
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string select_sql = string.Format("Select ID from KeyTable where Group_ID={0};", Group_ID);
                MySqlCommand command = new MySqlCommand(select_sql, conn, tran);
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
                else {
                    reader.Close();
                }

                if (Key_Ids != null && Key_Ids.Count != 0) {
                    StringBuilder buider = new StringBuilder();
                    foreach (int i in Key_Ids) {
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
                conn.Dispose();
            }
            return result;
        }

        public Dictionary<string, string> ListExcelToGroup()
        {
            throw new NotImplementedException();
        }

        public bool ModifyEntityRemark(string EntityName, string Remark)
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(EntityName)||string.IsNullOrWhiteSpace(Remark)) {
                return false;
            }
            string sql = string.Format("Update EntityTable Set Remark='{0}' Where EntityName='{1}'", Remark, EntityName);
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(sql, conn);
                if (command.ExecuteNonQuery() > 0) {
                result = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                conn.Close();
                conn.Dispose();
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
            MySqlConnection conn = new MySqlConnection(this.connectMessage);
            try
            {
                conn.Open();
                //回退ExcelTable
                MySqlCommand command = new MySqlCommand(sql, conn);
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
            finally {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}