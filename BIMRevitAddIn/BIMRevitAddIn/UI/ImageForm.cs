using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMRevitAddIn.UI
{
    public partial class ImageForm : Form
    {
        //实体名称
        private string entityName;
        public string EntityName {//初始化使用
            set {
                this.entityName = value;
            }
        }
        //数据
        private Dictionary<string, float> data;
        public Dictionary<string, float> Data {
            set {
                this.data = value;
            }
        }

        public ImageForm()
        {
            InitializeComponent();
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {
            this.Invalidate(this.ClientRectangle);
        }
       

        private void ImageForm_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            int height = this.ClientRectangle.Height;
            int width = this.ClientRectangle.Width;
            int startX = 30, endX = width - 10;
            int startY = 10, endY = height - 30;
            int length = 8;
            int divX = (endX - startX) / length;
            int divY = (endY - startY) / length;
            try
            {
                g.Clear(Color.White);
                Font font = new Font("Arial", 9, FontStyle.Regular);

                g.FillRectangle(Brushes.AliceBlue, 0, 0, width, height);
                //
                //
                //画图片的边框线
                Pen mypen = new Pen(Color.Blue, 1);
                mypen.DashStyle = DashStyle.DashDot;
                g.DrawRectangle(mypen, 2, 2, width - 3, height - 3);
                //绘制线条
                //绘制纵向线条
                int x = startX;
                for (int i = 0; i < length; i++)
                {
                    g.DrawLine(mypen, x, startY, x, endY);
                    x = x + divX;
                }
                g.DrawLine(mypen, x, startY, x, endY);
                //
                Pen mypen1 = new Pen(Color.Blue, 3);
                x = startX;
                g.DrawLine(mypen1, x, startY, x, endY);
                //绘制横向线条
                int y = startY;
                for (int i = 0; i < length; i++)
                {
                    g.DrawLine(mypen, startX, y, endX, y);
                    y = y + divY;
                }

                g.DrawLine(mypen1, startX, endY, endX, endY);
                //
                String[] n = { "第一期", "第二期", "第三期", "第四期", "上半年", "下半年", "全年统计" };
                x = startX - 5;
                for (int i = 0; i < 7; i++)
                {
                    g.DrawString(n[i].ToString(), font, Brushes.Red, x, endY + 5); //设置文字内容及输出位置
                    x = x + divX;
                }
                //
                //y轴
                String[] m = { "220人", " 200人", " 175人", "150人", " 125人", " 100人", " 75人", " 50人",
" 25人"};
                y = startY;
                for (int i = 0; i < 8; i++)
                {
                    g.DrawString(m[i].ToString(), font, Brushes.Red, startX - 5, y); //设置文字内容及输出位置
                    y = y + divY;
                }
                mypen.Dispose();
                mypen1.Dispose();
                g.Dispose();

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        
    }
}
