using System;
using System.Windows.Forms;
using System.IO;

namespace Revit.Addin.RevitTooltip
{
    
    public partial class SettingsForm : Form
    {
        private Autodesk.Revit.DB.Document m_doc = null;
        public SettingsForm(Autodesk.Revit.DB.Document doc)
        {
            InitializeComponent();

            m_doc = doc;
            RevitTooltip settings = ExtensibleStorage.GetTooltipInfo(m_doc.ProjectInformation);
            if (null != settings)
            {
                 //
                textBoxSurveyFile.Text = settings.SurveyFile;
                textBoxAlert.Text = settings.AlertNumber.ToString();
                alertNumberAdd.Text = settings.AlertNumberAdd.ToString();
                 //
                textBoxFoundationFile.Text = settings.FoundationFile;
                //
                textBoxUnderWallFile.Text = settings.UnderWallFile;
                //
                textServerPath.Text = settings.DfServer;
                textDB.Text = settings.DfDB;
                textPort.Text = settings.DfPort;
                textUser.Text = settings.DfUser;
                textPass.Text = settings.DfPassword;
                sqliteFilePath.Text = settings.SqliteFilePath;
                sqliteFileName.Text = settings.SqliteFileName;


            }
            else {
                settings = RevitTooltip.Default;
                string dir = Path.GetDirectoryName(this.GetType().Assembly.Location);


                textBoxSurveyFile.Text = Path.Combine(dir, settings.SurveyFile) ;
                textBoxAlert.Text = settings.AlertNumber.ToString();
                alertNumberAdd.Text = settings.AlertNumberAdd.ToString();

                textBoxFoundationFile.Text = Path.Combine(dir, settings.FoundationFile);
                textBoxUnderWallFile.Text = Path.Combine(dir, settings.UnderWallFile);
                textServerPath.Text = settings.DfServer;
                textDB.Text = settings.DfDB;
                textPort.Text = settings.DfPort;
                textUser.Text = settings.DfUser;
                textPass.Text = settings.DfPassword;
                sqliteFilePath.Text = settings.SqliteFilePath;
                sqliteFileName.Text = settings.SqliteFileName;


            }
            this.ActiveControl = textBoxSurveyFile;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            buttonOK.Enabled = false;

            try
            {
                RevitTooltip settings = App.settings;
                if (null == settings)
                {
                    settings = RevitTooltip.Default;
                }
                    settings.AlertNumber = double.Parse(textBoxAlert.Text);
                    settings.AlertNumberAdd = double.Parse(alertNumberAdd.Text);
                    settings.SurveyFile = textBoxSurveyFile.Text;
                    settings.FoundationFile = textBoxFoundationFile.Text;
                    settings.UnderWallFile = textBoxUnderWallFile.Text;
                    settings.DfServer = textServerPath.Text;
                    settings.DfDB = textDB.Text;
                    settings.DfPort = textPort.Text;
                    settings.DfUser = textUser.Text;
                    settings.DfPassword = textPass.Text;
                    settings.SqliteFilePath = sqliteFilePath.Text;
                    settings.SqliteFileName = sqliteFileName.Text;
                ExtensibleStorage.StoreTooltipInfo(m_doc.ProjectInformation, settings);
                //保存文档
                m_doc.Save(); 
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonBrowseSurvey_Click(object sender, EventArgs e)
        {
            string excelFile = CmdLoadFile.LoadFromFile();
            if (File.Exists(excelFile))
            {
                textBoxSurveyFile.Text = excelFile;
            }
        }

        private void buttonBrowseFoundationFile_Click(object sender, EventArgs e)
        {
            string excelFile = CmdLoadFile.LoadFromFile();
            if (File.Exists(excelFile))
            {
                textBoxFoundationFile.Text = excelFile;
            }
        }

        private void buttonBrowseUnderWallFile_Click(object sender, EventArgs e)
        {
            string excelFile = CmdLoadFile.LoadFromFile();
            if (File.Exists(excelFile))
            {
                textBoxUnderWallFile.Text = excelFile;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Sqlite文件（*.db）|*.db";
            saveDialog.FileName = "SqliteDfFile";
            saveDialog.DefaultExt = "db";
            saveDialog.AddExtension = false;
            saveDialog.RestoreDirectory = true;
            if (saveDialog.ShowDialog() == DialogResult.OK) {
                String fullPath = saveDialog.FileName;
                String path = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                String fileName = fullPath.Substring(fullPath.LastIndexOf("\\")+1);
                this.sqliteFilePath.Text = path;
                this.sqliteFileName.Text = fileName;
            }
        }
    }
}
