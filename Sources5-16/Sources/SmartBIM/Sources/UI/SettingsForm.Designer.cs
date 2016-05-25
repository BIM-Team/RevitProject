namespace Revit.Addin.RevitTooltip
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxSurveyFile = new System.Windows.Forms.TextBox();
            this.buttonBrowseSurvey = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAlert = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.alertNumberAdd = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxFoundationFile = new System.Windows.Forms.TextBox();
            this.buttonBrowseFoundationFile = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxUnderWallFile = new System.Windows.Forms.TextBox();
            this.buttonBrowseUnderWallFile = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textPass = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textUser = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textDB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textServerPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "导入监测数据文件：";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(389, 450);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "退出";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(281, 450);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxSurveyFile
            // 
            this.textBoxSurveyFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSurveyFile.Location = new System.Drawing.Point(8, 36);
            this.textBoxSurveyFile.Name = "textBoxSurveyFile";
            this.textBoxSurveyFile.Size = new System.Drawing.Size(372, 21);
            this.textBoxSurveyFile.TabIndex = 0;
            // 
            // buttonBrowseSurvey
            // 
            this.buttonBrowseSurvey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSurvey.Location = new System.Drawing.Point(386, 34);
            this.buttonBrowseSurvey.Name = "buttonBrowseSurvey";
            this.buttonBrowseSurvey.Size = new System.Drawing.Size(48, 23);
            this.buttonBrowseSurvey.TabIndex = 1;
            this.buttonBrowseSurvey.Text = "...";
            this.buttonBrowseSurvey.UseVisualStyleBackColor = true;
            this.buttonBrowseSurvey.Click += new System.EventHandler(this.buttonBrowseSurvey_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "累计变化预警值：±";
            // 
            // textBoxAlert
            // 
            this.textBoxAlert.Location = new System.Drawing.Point(126, 66);
            this.textBoxAlert.Name = "textBoxAlert";
            this.textBoxAlert.Size = new System.Drawing.Size(42, 21);
            this.textBoxAlert.TabIndex = 0;
            this.textBoxAlert.Text = "30";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(179, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "相邻差值预警：";
            // 
            // alertNumberAdd
            // 
            this.alertNumberAdd.Location = new System.Drawing.Point(286, 66);
            this.alertNumberAdd.Name = "alertNumberAdd";
            this.alertNumberAdd.Size = new System.Drawing.Size(94, 21);
            this.alertNumberAdd.TabIndex = 0;
            this.alertNumberAdd.Text = "2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.alertNumberAdd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxAlert);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxSurveyFile);
            this.groupBox1.Controls.Add(this.buttonBrowseSurvey);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(443, 97);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "检测数据设置";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxFoundationFile);
            this.groupBox2.Controls.Add(this.buttonBrowseFoundationFile);
            this.groupBox2.Location = new System.Drawing.Point(14, 115);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 57);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基础数据";
            // 
            // textBoxFoundationFile
            // 
            this.textBoxFoundationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFoundationFile.Location = new System.Drawing.Point(7, 23);
            this.textBoxFoundationFile.Name = "textBoxFoundationFile";
            this.textBoxFoundationFile.Size = new System.Drawing.Size(372, 21);
            this.textBoxFoundationFile.TabIndex = 0;
            // 
            // buttonBrowseFoundationFile
            // 
            this.buttonBrowseFoundationFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseFoundationFile.Location = new System.Drawing.Point(385, 21);
            this.buttonBrowseFoundationFile.Name = "buttonBrowseFoundationFile";
            this.buttonBrowseFoundationFile.Size = new System.Drawing.Size(48, 23);
            this.buttonBrowseFoundationFile.TabIndex = 1;
            this.buttonBrowseFoundationFile.Text = "...";
            this.buttonBrowseFoundationFile.UseVisualStyleBackColor = true;
            this.buttonBrowseFoundationFile.Click += new System.EventHandler(this.buttonBrowseFoundationFile_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxUnderWallFile);
            this.groupBox3.Controls.Add(this.buttonBrowseUnderWallFile);
            this.groupBox3.Location = new System.Drawing.Point(13, 178);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(442, 57);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "地墙数据";
            // 
            // textBoxUnderWallFile
            // 
            this.textBoxUnderWallFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUnderWallFile.Location = new System.Drawing.Point(7, 23);
            this.textBoxUnderWallFile.Name = "textBoxUnderWallFile";
            this.textBoxUnderWallFile.Size = new System.Drawing.Size(372, 21);
            this.textBoxUnderWallFile.TabIndex = 0;
            // 
            // buttonBrowseUnderWallFile
            // 
            this.buttonBrowseUnderWallFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseUnderWallFile.Location = new System.Drawing.Point(385, 21);
            this.buttonBrowseUnderWallFile.Name = "buttonBrowseUnderWallFile";
            this.buttonBrowseUnderWallFile.Size = new System.Drawing.Size(48, 23);
            this.buttonBrowseUnderWallFile.TabIndex = 1;
            this.buttonBrowseUnderWallFile.Text = "...";
            this.buttonBrowseUnderWallFile.UseVisualStyleBackColor = true;
            this.buttonBrowseUnderWallFile.Click += new System.EventHandler(this.buttonBrowseUnderWallFile_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textPass);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.textUser);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.textPort);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textDB);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.textServerPath);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(12, 241);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(444, 116);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "数据库设置";
            // 
            // textPass
            // 
            this.textPass.Location = new System.Drawing.Point(280, 80);
            this.textPass.Name = "textPass";
            this.textPass.Size = new System.Drawing.Size(100, 21);
            this.textPass.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(246, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 8;
            this.label8.Text = "密码";
            // 
            // textUser
            // 
            this.textUser.Location = new System.Drawing.Point(80, 81);
            this.textUser.Name = "textUser";
            this.textUser.Size = new System.Drawing.Size(100, 21);
            this.textUser.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "用户名";
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(280, 47);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(100, 21);
            this.textPort.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(244, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "端口";
            // 
            // textDB
            // 
            this.textDB.Location = new System.Drawing.Point(80, 47);
            this.textDB.Name = "textDB";
            this.textDB.Size = new System.Drawing.Size(100, 21);
            this.textDB.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "数据库";
            // 
            // textServerPath
            // 
            this.textServerPath.Location = new System.Drawing.Point(80, 17);
            this.textServerPath.Name = "textServerPath";
            this.textServerPath.Size = new System.Drawing.Size(300, 21);
            this.textServerPath.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "服务器地址";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.progressBar);
            this.groupBox5.Location = new System.Drawing.Point(12, 363);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(444, 68);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "处理进度";
            this.groupBox5.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 26);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(422, 23);
            this.progressBar.TabIndex = 0;
            this.progressBar.Visible = false;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(476, 485);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文件导入";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxSurveyFile;
        private System.Windows.Forms.Button buttonBrowseSurvey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAlert;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox alertNumberAdd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxFoundationFile;
        private System.Windows.Forms.Button buttonBrowseFoundationFile;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxUnderWallFile;
        private System.Windows.Forms.Button buttonBrowseUnderWallFile;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textPass;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textUser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textDB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textServerPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}