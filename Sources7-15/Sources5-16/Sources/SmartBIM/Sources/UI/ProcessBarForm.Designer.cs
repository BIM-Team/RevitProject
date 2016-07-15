namespace Revit.Addin.RevitTooltip.UI
{
    partial class ProcessBarForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(14, 42);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(345, 23);
            this.progressBar.TabIndex = 0;
            // 
            // progressLabel
            // 
            this.progressLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.progressLabel.Location = new System.Drawing.Point(14, 15);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.ReadOnly = true;
            this.progressLabel.Size = new System.Drawing.Size(345, 14);
            this.progressLabel.TabIndex = 2;
            // 
            // ProcessBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 83);
            this.ControlBox = false;
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ProcessBarForm";
            this.Text = "导入文件";
            this.Shown += new System.EventHandler(this.ProcessBarForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox progressLabel;
    }
}