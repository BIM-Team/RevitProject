using BIMRevitAddIn.UI;
using BIMRevitAddIn.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    class PropertyTest
    {
        [STAThread]
        public static void Main(string[] arg)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //
            //InfoForm form = new InfoForm();
            //form.ServerPath = @"server=localhost;port=3306;database='hibernate';user='root';password='root';charset='utf8'";
            //连接打开数据库
            //MysqlUtil mysql = MysqlUtil.CreateInstance("127.0.0.1", "root", "hzj", "root");
            //打开连接
           // mysql.OpenConnect();
            ImageForm form = new ImageForm();
            form.EntityName = "CX1";
           // form.Data= mysql.SelectOneCXData("CX1");
           // mysql.Close();  //关闭数据库连接
           // mysql.Dispose();

            Application.Run(form);
        }
    }

}
