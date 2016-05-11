namespace BIMRevitAddIn.UI
{
    partial class FileImportDialog
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
            this.filePathData = new System.Windows.Forms.TextBox();
            this.btData = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.filePathBase = new System.Windows.Forms.TextBox();
            this.btBase = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.filePathWall = new System.Windows.Forms.TextBox();
            this.btWall = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "文件导入";
            // 
            // filePathData
            // 
            this.filePathData.Location = new System.Drawing.Point(113, 43);
            this.filePathData.Name = "filePathData";
            this.filePathData.Size = new System.Drawing.Size(316, 21);
            this.filePathData.TabIndex = 2;
            // 
            // btData
            // 
            this.btData.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btData.Location = new System.Drawing.Point(453, 42);
            this.btData.Name = "btData";
            this.btData.Size = new System.Drawing.Size(75, 23);
            this.btData.TabIndex = 3;
            this.btData.Text = "...";
            this.btData.UseVisualStyleBackColor = true;
            this.btData.Click += new System.EventHandler(this.btData_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(212, 81);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "累计报警阈值：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "相邻报警阈值：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(427, 81);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "基础数据路径：";
            // 
            // filePathBase
            // 
            this.filePathBase.Location = new System.Drawing.Point(113, 137);
            this.filePathBase.Name = "filePathBase";
            this.filePathBase.Size = new System.Drawing.Size(316, 21);
            this.filePathBase.TabIndex = 9;
            // 
            // btBase
            // 
            this.btBase.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btBase.Location = new System.Drawing.Point(453, 137);
            this.btBase.Name = "btBase";
            this.btBase.Size = new System.Drawing.Size(75, 23);
            this.btBase.TabIndex = 10;
            this.btBase.Text = "...";
            this.btBase.UseVisualStyleBackColor = true;
            this.btBase.Click += new System.EventHandler(this.btBase_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "城墙数据路径：";
            // 
            // filePathWall
            // 
            this.filePathWall.Location = new System.Drawing.Point(113, 199);
            this.filePathWall.Name = "filePathWall";
            this.filePathWall.Size = new System.Drawing.Size(316, 21);
            this.filePathWall.TabIndex = 12;
            // 
            // btWall
            // 
            this.btWall.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btWall.Location = new System.Drawing.Point(453, 199);
            this.btWall.Name = "btWall";
            this.btWall.Size = new System.Drawing.Size(75, 23);
            this.btWall.TabIndex = 13;
            this.btWall.Text = "...";
            this.btWall.UseVisualStyleBackColor = true;
            this.btWall.Click += new System.EventHandler(this.btWall_Click);
            // 
            // btOk
            // 
            this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOk.Location = new System.Drawing.Point(326, 256);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 14;
            this.btOk.Text = "确认";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(453, 256);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 15;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "测量数据路径：";
            // 
            // FileImportDialog
            // 
            this.AcceptButton = this.btOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(587, 307);
            this.ControlBox = false;
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.btWall);
            this.Controls.Add(this.filePathWall);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btBase);
            this.Controls.Add(this.filePathBase);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btData);
            this.Controls.Add(this.filePathData);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileImportDialog";
            this.Text = "设置";
            this.Load += new System.EventHandler(this.FileImportDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox filePathData;
        private System.Windows.Forms.Button btData;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox filePathBase;
        private System.Windows.Forms.Button btBase;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox filePathWall;
        private System.Windows.Forms.Button btWall;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label2;
    }
}