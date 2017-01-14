using Res = Revit.Addin.RevitTooltip.Properties.Resources;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Revit.Addin.RevitTooltip.Intface;
using Revit.Addin.RevitTooltip.Impl;
using Revit.Addin.RevitTooltip.Dto;

namespace Revit.Addin.RevitTooltip.UI
{
    public partial class NewSettings : Form
    {
        private bool IsThresholdChanged = false;
        /// <summary>
        /// 记录当前的设置是否有更改
        /// </summary>
        private bool IsSettingChanged = false;
        /// <summary>
        /// 记录待处理的文件（excel）列表
        /// </summary>
        private string[] fileNames = null;
        /// <summary>
        /// Excel的读取工具，该工具只在这里使用，外部使用
        /// </summary>
        private IExcelReader excelReader = null;
        public NewSettings()
        {
            excelReader = new ExcelReader();
            InitializeComponent();
        }
        internal NewSettings(RevitTooltip settings)
        {
            InitializeComponent();
            if (settings != null)
            {
                this.textAddr.Text = settings.DfServer;
                this.textDB.Text = settings.DfDB;
                this.textPort.Text = settings.DfPort;
                this.textUser.Text = settings.DfUser;
                this.textPass.Text = settings.DfPassword;
                this.textSqlitePath.Text = settings.SqliteFilePath;
                this.textSqliteName.Text = settings.SqliteFileName;
            }
            dataGridView2.AutoGenerateColumns = false;
            dataGridView1.AutoGenerateColumns = false;
            excelReader = new ExcelReader();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Res.String_SelectExcelFile;
            ofd.DefaultExt = ".xls";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;
            ofd.Filter = "Excel 97-2003 Workbook(*.xls)|*.xls";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.fileNames = ofd.FileNames;
                StringBuilder buider = new StringBuilder();
                foreach (string s in this.fileNames)
                {
                    buider.Append(s).Append(";");
                }
                buider.Remove(buider.Length - 1, 1);
                this.textFilePath.Text = buider.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectUrl = "server = " + this.textAddr.Text.Trim() +
                "; user =" + this.textUser.Text.Trim() +
                "; database =" + this.textDB.Text.Trim() +
                "; port = " + this.textPort.Text.Trim() +
                "; password =" + this.textPass.Text.Trim() +
                "; charset = utf8";
            MySqlConnection conn = new MySqlConnection(connectUrl);

            try
            {
                conn.Open();
                MessageBox.Show("连接成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败：" + ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            this.IsSettingChanged = true;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Sqlite文件（*.db）|*.db";
            saveDialog.FileName = "SqliteDfFile";
            saveDialog.DefaultExt = "db";
            saveDialog.AddExtension = false;
            saveDialog.RestoreDirectory = true;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                String fullPath = saveDialog.FileName;
                String path = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                String fileName = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);
                this.textSqlitePath.Text = path;
                this.textSqliteName.Text = fileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.IsSettingChanged)
            {
                RevitTooltip newSetting = RevitTooltip.Default;
                newSetting.DfServer = this.textAddr.Text.Trim();
                newSetting.DfDB = this.textDB.Text.Trim();
                newSetting.DfPort = this.textPort.Text.Trim();
                newSetting.DfUser = this.textUser.Text.Trim();
                newSetting.DfPassword = this.textPass.Text.Trim();
                newSetting.SqliteFilePath = this.textSqlitePath.Text.Trim();
                newSetting.SqliteFileName = this.textSqliteName.Text.Trim();
                App.Instance.Settings = newSetting;
                this.IsSettingChanged = false;
                MessageBox.Show("成功");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonInMysql_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定导入到Mysql?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.progressBar1.Value = 0;
                int i = 0;
                try
                {
                    for (; i < fileNames.Length; i++)
                    {
                        SheetInfo info = this.excelReader.loadExcelData(fileNames[i]);
                        App.Instance.MySql.InsertSheetInfo(info);
                        this.progressBar1.Value = (int)((i + 1.0) / fileNames.Length) * progressBar1.Maximum;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("导入异常，成功处理" + i + "个文件");
                    return;
                }
                MessageBox.Show("导入成功");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定导入到本地？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.progressBar1.Value = 0;
                int i = 0;
                try
                {
                    for (; i < fileNames.Length; i++)
                    {
                        SheetInfo info = this.excelReader.loadExcelData(fileNames[i]);
                        App.Instance.Sqlite.InsertSheetInfo(info);
                        this.progressBar1.Value = (int)((i + 1.0) / fileNames.Length) * progressBar1.Maximum;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("导入异常，成功处理" + i + "文件");
                    return;
                }
                MessageBox.Show("导入成功");
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            TabPage p = e.TabPage;
            if (p == this.tabPageFile)
            {
                if (App.Instance.MySql.IsReady)
                {
                    this.buttonInMysql.Enabled = true;
                }
                else
                {
                    this.buttonInMysql.Enabled = false;
                }
            }
            if (p == this.tabPageThreshold)
            {
                List<ExcelTable> tables = new List<ExcelTable>();
                if (useSqliteThreshold.Checked)
                {
                    tables = App.Instance.Sqlite.ListExcelsMessage(false);
                }
                else if (App.Instance.MySql.IsReady)
                {
                    tables = App.Instance.MySql.ListExcelsMessage(false);
                }
                this.dataGridView1.DataSource = tables;

            }
            if (p == this.tabPagePro)
            {
                List<ExcelTable> tables = new List<ExcelTable>();
                if (useSqlitePro.Checked)
                {
                    tables = App.Instance.Sqlite.ListExcelsMessage(true);
                }
                else if (App.Instance.MySql.IsReady)
                {
                    tables = App.Instance.MySql.ListExcelsMessage(true);
                }
                this.combExcel.DataSource = tables;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            string Signal = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
            try
            {
                float Total_hold = float.Parse(this.dataGridView1.CurrentRow.Cells[3].Value.ToString());
                float Diff_hold = float.Parse(this.dataGridView1.CurrentRow.Cells[4].Value.ToString());
                if (useSqliteThreshold.Checked)
                {
                    App.Instance.Sqlite.ModifyThreshold(Signal, Total_hold, Diff_hold);
                    this.IsThresholdChanged = true;
                }
                else if (App.Instance.MySql.IsReady)
                {
                    App.Instance.MySql.ModifyThreshold(Signal, Total_hold, Diff_hold);
                }
            }
            catch (Exception)
            {
                this.dataGridView1.CancelEdit();
                MessageBox.Show("无效的编辑");

            }
        }



        private void combExcel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string signal = combExcel.SelectedValue.ToString();

            if (!string.IsNullOrWhiteSpace(signal))
            {
                List<Group> groups = null;
                List<CKeyName> keyNames = null;
                if (useSqlitePro.Checked)
                {
                    groups = App.Instance.Sqlite.loadGroupForAExcel(signal);
                    keyNames = App.Instance.Sqlite.loadKeyNameForExcelAndGroup(signal);
                }
                else if (App.Instance.MySql.IsReady)
                {
                    groups = App.Instance.MySql.loadGroupForAExcel(signal);
                    keyNames = App.Instance.MySql.loadKeyNameForExcelAndGroup(signal);
                }
                this.combGroup.DataSource = groups;
                this.dataGridView2.DataSource = keyNames;
            }
            else
            {
                throw new Exception("无效的选择");
            }
        }

        private void combGroup_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string signal = this.combExcel.SelectedValue.ToString();
            int id = Convert.ToInt32(this.combGroup.SelectedValue.ToString());
            if (useSqlitePro.Checked)
            {
                this.dataGridView2.DataSource = App.Instance.Sqlite.loadKeyNameForExcelAndGroup(signal, id);
            }
            else if (App.Instance.MySql.IsReady)
            {
                this.dataGridView2.DataSource = App.Instance.MySql.loadKeyNameForExcelAndGroup(signal, id);
            }

        }


        private void button5_Click(object sender, EventArgs e)
        {
            int group_id;
            string signal = this.combExcel.SelectedValue.ToString();
            if (this.combGroup.SelectedValue == null)
            {
                string newGroupName = combGroup.Text;
                if (string.IsNullOrWhiteSpace(newGroupName))
                {
                    MessageBox.Show("无效的组名");
                    return;
                }
                Group newOne = null;
                if (useSqlitePro.Checked)
                {
                    newOne = App.Instance.Sqlite.AddNewGroup(signal, newGroupName);
                }
                else if (App.Instance.MySql.IsReady)
                {
                    newOne = App.Instance.MySql.AddNewGroup(signal, newGroupName);
                }
                else {
                    MessageBox.Show("Mysql暂时不可用");
                    return;
                }
                group_id = newOne.Id;
            }
            else
            {
                group_id = Convert.ToInt32(this.combGroup.SelectedValue.ToString());

            }
            List<int> OK_ids = new List<int>();
            foreach (DataGridViewRow row in this.dataGridView2.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value.ToString()))
                {
                    OK_ids.Add(Convert.ToInt32(row.Cells[1].Value.ToString()));
                }
            }
            bool tag = false;
            if (useSqlitePro.Checked)
            {
                tag = App.Instance.Sqlite.AddKeysToGroup(group_id, OK_ids);
                this.combGroup.DataSource= App.Instance.Sqlite.loadGroupForAExcel(signal);
                this.dataGridView2.DataSource= App.Instance.Sqlite.loadKeyNameForExcelAndGroup(signal);
            }
            else if (App.Instance.MySql.IsReady)
            {
                tag = App.Instance.MySql.AddKeysToGroup(group_id, OK_ids);
                this.combGroup.DataSource = App.Instance.MySql.loadGroupForAExcel(signal);
                this.dataGridView2.DataSource = App.Instance.MySql.loadKeyNameForExcelAndGroup(signal);
            }
            if (tag)
            {

                MessageBox.Show("修改成功");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void useSqliteThreshold_CheckedChanged(object sender, EventArgs e)
        {
            List<ExcelTable> tables = new List<ExcelTable>();
            if (useSqliteThreshold.Checked)
            {
                tables = App.Instance.Sqlite.ListExcelsMessage(false);
            }
            else if (App.Instance.MySql.IsReady)
            {
                tables = App.Instance.MySql.ListExcelsMessage(false);
            }
            this.dataGridView1.DataSource = tables;
        }

        private void useSqlitePro_CheckedChanged(object sender, EventArgs e)
        {
            List<ExcelTable> tables = new List<ExcelTable>();
            if (useSqlitePro.Checked)
            {
                tables = App.Instance.Sqlite.ListExcelsMessage(true);
            }
            else if (App.Instance.MySql.IsReady)
            {
                tables = App.Instance.MySql.ListExcelsMessage(true);
            }
            this.combExcel.DataSource = tables;
            this.combGroup.DataSource = new List<Group>();
            this.combGroup.Text = "";
            this.dataGridView2.DataSource = new List<CKeyName>();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            Group select_Group = combGroup.SelectedItem as Group;
            if (select_Group != null)
            {
                bool tag = false;
                try
                {
                    if (useSqlitePro.Checked)
                    {
                        App.Instance.Sqlite.DeleteGroup(select_Group.Id);
                        tag = true;
                    }
                    else if (App.Instance.MySql.IsReady)
                    {
                        App.Instance.MySql.DeleteGroup(select_Group.Id);
                        tag = true;
                    }
                    if (tag)
                    {
                        string signal = combExcel.SelectedValue.ToString();
                        List<Group> groups = new List<Group>();
                        List<CKeyName> keyNames = new List<CKeyName>();
                        if (useSqlitePro.Checked)
                        {
                            groups = App.Instance.Sqlite.loadGroupForAExcel(signal);
                            keyNames = App.Instance.Sqlite.loadKeyNameForExcelAndGroup(signal);
                        }
                        else if (App.Instance.MySql.IsReady)
                        {
                            groups = App.Instance.MySql.loadGroupForAExcel(signal);
                            keyNames = App.Instance.MySql.loadKeyNameForExcelAndGroup(signal);
                        }
                        //CombGroup
                        this.combGroup.DataSource = groups;
                        this.combGroup.Text = "";
                        ////dataGrid
                        this.dataGridView2.DataSource = keyNames;
                        MessageBox.Show("删除成功");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("删除失败");
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "打开本地文件";
            openFile.Filter = "Sqlite文件（*.db）|*.db";
            openFile.FileName = "SqliteDfFile";
            openFile.DefaultExt = "db";
            openFile.AddExtension = false;
            openFile.RestoreDirectory = true;
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                String fullPath = openFile.FileName;
                String path = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                String fileName = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);
                this.textSqlitePath.Text = path;
                this.textSqliteName.Text = fileName;
            }
        }

        private void NewSettings_Deactivate(object sender, EventArgs e)
        {
            if (this.IsThresholdChanged) {
                App.Instance.ThresholdChanged = true;
            }
        }
    }
}
