namespace Revit.Addin.RevitTooltip.UI
{
    partial class NewSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDB = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.textSqliteName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textSqlitePath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textAddr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textDB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textUser = new System.Windows.Forms.TextBox();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPageFile = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.buttonInMysql = new System.Windows.Forms.Button();
            this.textFilePath = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.tabPageThreshold = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.useSqliteThreshold = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPagePro = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.useSqlitePro = new System.Windows.Forms.CheckBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.combGroup = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.combExcel = new System.Windows.Forms.ComboBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPageDB.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPageFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageThreshold.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPagePro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageDB);
            this.tabControl1.Controls.Add(this.tabPageFile);
            this.tabControl1.Controls.Add(this.tabPageThreshold);
            this.tabControl1.Controls.Add(this.tabPagePro);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(545, 289);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPageDB
            // 
            this.tabPageDB.Controls.Add(this.groupBox2);
            this.tabPageDB.Controls.Add(this.groupBox3);
            this.tabPageDB.Controls.Add(this.button3);
            this.tabPageDB.Controls.Add(this.button2);
            this.tabPageDB.Location = new System.Drawing.Point(4, 22);
            this.tabPageDB.Name = "tabPageDB";
            this.tabPageDB.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDB.Size = new System.Drawing.Size(537, 263);
            this.tabPageDB.TabIndex = 0;
            this.tabPageDB.Text = "数据库连接";
            this.tabPageDB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button10);
            this.groupBox2.Controls.Add(this.button8);
            this.groupBox2.Controls.Add(this.textSqliteName);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textSqlitePath);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(8, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(521, 91);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "本地文件";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(405, 14);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 5;
            this.button10.Text = "打开";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(405, 52);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 4;
            this.button8.Text = "新建";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // textSqliteName
            // 
            this.textSqliteName.Location = new System.Drawing.Point(80, 54);
            this.textSqliteName.Name = "textSqliteName";
            this.textSqliteName.ReadOnly = true;
            this.textSqliteName.Size = new System.Drawing.Size(298, 21);
            this.textSqliteName.TabIndex = 3;
            this.textSqliteName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "文件名：";
            // 
            // textSqlitePath
            // 
            this.textSqlitePath.Location = new System.Drawing.Point(80, 16);
            this.textSqlitePath.Name = "textSqlitePath";
            this.textSqlitePath.ReadOnly = true;
            this.textSqlitePath.Size = new System.Drawing.Size(298, 21);
            this.textSqlitePath.TabIndex = 1;
            this.textSqlitePath.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "文件路径：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textAddr);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textDB);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.textPass);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textUser);
            this.groupBox3.Controls.Add(this.textPort);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(7, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(523, 100);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mysql设置";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器地址：";
            // 
            // textAddr
            // 
            this.textAddr.Location = new System.Drawing.Point(81, 20);
            this.textAddr.Name = "textAddr";
            this.textAddr.Size = new System.Drawing.Size(104, 21);
            this.textAddr.TabIndex = 1;
            this.textAddr.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "数据库名：";
            // 
            // textDB
            // 
            this.textDB.Location = new System.Drawing.Point(257, 20);
            this.textDB.Name = "textDB";
            this.textDB.Size = new System.Drawing.Size(101, 21);
            this.textDB.TabIndex = 3;
            this.textDB.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(406, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "测试连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textPass
            // 
            this.textPass.Location = new System.Drawing.Point(257, 57);
            this.textPass.Name = "textPass";
            this.textPass.Size = new System.Drawing.Size(101, 21);
            this.textPass.TabIndex = 9;
            this.textPass.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(362, 25);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "端口：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(213, 60);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "密码：";
            // 
            // textUser
            // 
            this.textUser.Location = new System.Drawing.Point(81, 55);
            this.textUser.Name = "textUser";
            this.textUser.Size = new System.Drawing.Size(104, 21);
            this.textUser.TabIndex = 7;
            this.textUser.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(406, 20);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(75, 21);
            this.textPort.TabIndex = 5;
            this.textPort.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 60);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "用户名：";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(343, 225);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "取消";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(120, 225);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "应用";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPageFile
            // 
            this.tabPageFile.Controls.Add(this.groupBox1);
            this.tabPageFile.Location = new System.Drawing.Point(4, 22);
            this.tabPageFile.Name = "tabPageFile";
            this.tabPageFile.Size = new System.Drawing.Size(537, 263);
            this.tabPageFile.TabIndex = 2;
            this.tabPageFile.Text = "文件导入";
            this.tabPageFile.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.buttonInMysql);
            this.groupBox1.Controls.Add(this.textFilePath);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Location = new System.Drawing.Point(14, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 226);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Excel导入";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 8;
            this.label10.Text = "进度：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(65, 115);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(403, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(269, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "文件名格式要求：文件名-简写-类型（I，C，CX）";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(377, 173);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(96, 23);
            this.button7.TabIndex = 6;
            this.button7.Text = "取消";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "文件名：";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(184, 173);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(96, 23);
            this.button6.TabIndex = 5;
            this.button6.Text = "导入至本地";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // buttonInMysql
            // 
            this.buttonInMysql.Location = new System.Drawing.Point(15, 173);
            this.buttonInMysql.Name = "buttonInMysql";
            this.buttonInMysql.Size = new System.Drawing.Size(96, 23);
            this.buttonInMysql.TabIndex = 4;
            this.buttonInMysql.Text = "导入至Mysql";
            this.buttonInMysql.UseVisualStyleBackColor = true;
            this.buttonInMysql.Click += new System.EventHandler(this.buttonInMysql_Click);
            // 
            // textFilePath
            // 
            this.textFilePath.Location = new System.Drawing.Point(65, 67);
            this.textFilePath.Name = "textFilePath";
            this.textFilePath.Size = new System.Drawing.Size(308, 21);
            this.textFilePath.TabIndex = 2;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(393, 65);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tabPageThreshold
            // 
            this.tabPageThreshold.Controls.Add(this.splitContainer2);
            this.tabPageThreshold.Location = new System.Drawing.Point(4, 22);
            this.tabPageThreshold.Name = "tabPageThreshold";
            this.tabPageThreshold.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageThreshold.Size = new System.Drawing.Size(537, 263);
            this.tabPageThreshold.TabIndex = 1;
            this.tabPageThreshold.Text = "阈值设定";
            this.tabPageThreshold.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.useSqliteThreshold);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Size = new System.Drawing.Size(531, 257);
            this.splitContainer2.SplitterDistance = 32;
            this.splitContainer2.TabIndex = 1;
            // 
            // useSqliteThreshold
            // 
            this.useSqliteThreshold.AutoSize = true;
            this.useSqliteThreshold.Location = new System.Drawing.Point(29, 7);
            this.useSqliteThreshold.Name = "useSqliteThreshold";
            this.useSqliteThreshold.Size = new System.Drawing.Size(96, 16);
            this.useSqliteThreshold.TabIndex = 1;
            this.useSqliteThreshold.Text = "使用本地文件";
            this.useSqliteThreshold.UseVisualStyleBackColor = true;
            this.useSqliteThreshold.CheckedChanged += new System.EventHandler(this.useSqliteThreshold_CheckedChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column4,
            this.Column9,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column8});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(531, 221);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.DataPropertyName = "Id";
            this.Column4.HeaderText = "Excel_ID";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Visible = false;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "Signal";
            this.Column9.HeaderText = "简称";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.DataPropertyName = "CurrentFile";
            this.Column1.HeaderText = "最新文档";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.DataPropertyName = "Total_hold";
            this.Column2.HeaderText = "累计阈值";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.DataPropertyName = "Diff_hold";
            this.Column3.HeaderText = "相邻阈值";
            this.Column3.Name = "Column3";
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "History";
            this.Column8.HeaderText = "历史导入";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            // 
            // tabPagePro
            // 
            this.tabPagePro.Controls.Add(this.splitContainer1);
            this.tabPagePro.Location = new System.Drawing.Point(4, 22);
            this.tabPagePro.Name = "tabPagePro";
            this.tabPagePro.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePro.Size = new System.Drawing.Size(537, 263);
            this.tabPagePro.TabIndex = 3;
            this.tabPagePro.Text = "属性分组";
            this.tabPagePro.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView2);
            this.splitContainer1.Size = new System.Drawing.Size(531, 257);
            this.splitContainer1.SplitterDistance = 231;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.useSqlitePro);
            this.groupBox4.Controls.Add(this.button9);
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.combGroup);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.combExcel);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(231, 257);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "分组信息";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label13.Location = new System.Drawing.Point(194, 124);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 12);
            this.label13.TabIndex = 7;
            this.label13.Text = "删除";
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // useSqlitePro
            // 
            this.useSqlitePro.AutoSize = true;
            this.useSqlitePro.Location = new System.Drawing.Point(67, 20);
            this.useSqlitePro.Name = "useSqlitePro";
            this.useSqlitePro.Size = new System.Drawing.Size(96, 16);
            this.useSqlitePro.TabIndex = 6;
            this.useSqlitePro.Text = "使用本地文件";
            this.useSqlitePro.UseVisualStyleBackColor = true;
            this.useSqlitePro.CheckedChanged += new System.EventHandler(this.useSqlitePro_CheckedChanged);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(138, 186);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 5;
            this.button9.Text = "取消";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(26, 186);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "确认";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // combGroup
            // 
            this.combGroup.DisplayMember = "GroupName";
            this.combGroup.FormattingEnabled = true;
            this.combGroup.Items.AddRange(new object[] {
            "新建"});
            this.combGroup.Location = new System.Drawing.Point(65, 120);
            this.combGroup.Name = "combGroup";
            this.combGroup.Size = new System.Drawing.Size(121, 20);
            this.combGroup.TabIndex = 3;
            this.combGroup.Text = "选择分组";
            this.combGroup.ValueMember = "ID";
            this.combGroup.SelectionChangeCommitted += new System.EventHandler(this.combGroup_SelectionChangeCommitted);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "Excel表：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 123);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "分组：";
            // 
            // combExcel
            // 
            this.combExcel.DisplayMember = "CurrentFile";
            this.combExcel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combExcel.FormattingEnabled = true;
            this.combExcel.Location = new System.Drawing.Point(65, 62);
            this.combExcel.Name = "combExcel";
            this.combExcel.Size = new System.Drawing.Size(121, 20);
            this.combExcel.TabIndex = 1;
            this.combExcel.ValueMember = "Signal";
            this.combExcel.SelectionChangeCommitted += new System.EventHandler(this.combExcel_SelectionChangeCommitted);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.Column7,
            this.Column6});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView2.Size = new System.Drawing.Size(296, 257);
            this.dataGridView2.TabIndex = 0;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.DataPropertyName = "IsCheck";
            this.Column5.FalseValue = "False";
            this.Column5.FillWeight = 50F;
            this.Column5.HeaderText = "选择";
            this.Column5.IndeterminateValue = "";
            this.Column5.Name = "Column5";
            this.Column5.TrueValue = "True";
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column7.DataPropertyName = "Id";
            this.Column7.HeaderText = "ID";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column7.Visible = false;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.DataPropertyName = "KeyName";
            this.Column6.HeaderText = "属性名";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // NewSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 289);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewSettings";
            this.ShowIcon = false;
            this.Text = "设置";
            this.Deactivate += new System.EventHandler(this.NewSettings_Deactivate);
            this.tabControl1.ResumeLayout(false);
            this.tabPageDB.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPageFile.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageThreshold.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPagePro.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDB;
        private System.Windows.Forms.TabPage tabPageThreshold;
        private System.Windows.Forms.TabPage tabPageFile;
        private System.Windows.Forms.TextBox textAddr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textPass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textFilePath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button buttonInMysql;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox textSqliteName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textSqlitePath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TabPage tabPagePro;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox combExcel;
        private System.Windows.Forms.ComboBox combGroup;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox useSqliteThreshold;
        private System.Windows.Forms.CheckBox useSqlitePro;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button10;
    }
}