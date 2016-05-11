using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMRevitAddIn.UI
{
    public partial class FileImportDialog : Form
    {
        public FileImportDialog()
        {
            InitializeComponent();
        }


        //测量数据的路径
        public string FilePathData {
            get {
                return this.filePathData.Text;
            }
        }

        //获取基础数据的路径
        public string FilePathBase {
            get {
                return this.filePathBase.Text;
            }
        }

        //获取城墙数据的路径
        public string FilePathWall {
            get
            {
                return this.filePathWall.Text;
            }
        }

        private void FileImportDialog_Load(object sender, EventArgs e)
        {

        }

        private void btOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"c:\";
            openFile.Filter = "Excel File(*.xls)|*.xls;*.xlsx";
            openFile.FilterIndex = 0;
            if (openFile.ShowDialog() == DialogResult.OK) {
                this.filePathData.Text = openFile.FileName;
            }
        }

        private void btBase_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"c:\";
            openFile.Filter = "Excel File(*.xls)|*.xls;*.xlsx|All files(*.*)|*.*";
            openFile.FilterIndex = 0;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                this.filePathBase.Text = openFile.FileName;
            }
        }

        private void btWall_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"c:\";
            openFile.Filter = "Excel File(*.xls)|*.xls;*.xlsx|All files(*.*)|*.*";
            openFile.FilterIndex = 0;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                this.filePathWall.Text = openFile.FileName;
            }
        }
    }
}
