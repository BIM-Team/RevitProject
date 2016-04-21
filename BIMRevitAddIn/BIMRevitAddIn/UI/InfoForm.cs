using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMRevitAddIn.UI
{
    public partial class InfoForm : Form
    {
        //dataGridView绑定的数据集
        DataSet dataSet;     
       //用于更新数据库
        private MySqlDataAdapter adapter;
        //实体名
        //用于查询相关实体的属性及其属性值
        private string entityName="1";
        public string EntityName {
            set {
                this.entityName = value;
            }
        }
        
        //数据库连接
        //数据集
        private string serverPath;
        public string ServerPath
        {
            set
            {
                this.serverPath = value;
            }

        }
        //


        public InfoForm()
        {
            InitializeComponent();
            //初始化数据集
            dataSet = new DataSet();

        }

        //本用于添加的代码，但是这里还未实现，用于测试更新数据
        private void btOk_Click(object sender, EventArgs e)
        {
            ResetEntiy("2");
        }

        //数据绑定
        private void InfoForm_Load(object sender, EventArgs e)
        {
            this.SetDataView();

        }

        private void Properties_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataSet.HasChanges()) {
                
                adapter.Update(dataSet, "student");
                this.Properties.Update();

            }
        }

        private void Properties_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int col = e.ColumnIndex;
            int row = e.RowIndex;
            if (col< 1||row<0){
                return;
            }
            if ("1".Equals(this.Properties[0, row].Value.ToString())) {
                this.Properties[1, row].ReadOnly = false;
               


            }
        }
        //设置数据
        //绑定数据源
        private void SetDataView() {
            if (string.IsNullOrEmpty(serverPath)||string.IsNullOrEmpty(entityName))
            {
                return;
            }
            MySqlConnection conn = new MySqlConnection(serverPath);           
            adapter = new MySqlDataAdapter("select ID PropertyName, NAME PropertyValue from student where 1=1 and ID='"+ entityName+"'", conn);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
           
            dataSet.Clear();
            adapter.Fill(dataSet, "student");
            Properties.DataSource = dataSet;
            Properties.DataMember = "student";
        }
        //
        //用于更新实体
        //重新绑定数据源
        public void ResetEntiy(string entity) {
            this.entityName = entity;
            SetDataView();
            Properties.Update();
        }

    }
}
