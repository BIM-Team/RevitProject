using BIMRevitAddIn.Util;
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
        private MysqlUtil mysql;
        //实体名称
        private string entityName;
        public string EntityName {//初始化使用
            set {
             
                if (null != value && !value.Equals(entityName)) {
                    try
                    {
                        mysql = MysqlUtil.CreateInstance();
                        mysql.OpenConnect();
                        data = mysql.SelectOneCXData(value);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally {
                        mysql.Close();
                        //mysql.Dispose();
                            }
                this.Text = value + "的测试数据";
                this.entityName = value;
                    this.Invalidate(this.ClientRectangle);
                }
              
            }
        }
        //数据
        private Dictionary<string, float> data;
        //public Dictionary<string, float> Data {
        //    set {
        //        this.data = value;
        //        this.Invalidate(this.ClientRectangle);
        //    }
        //}

        public ImageForm()
        {
            InitializeComponent();
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {
           // this.Invalidate(this.ClientRectangle);
        }
       

        private void ImageForm_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            float height = this.ClientRectangle.Height;
            float width = this.ClientRectangle.Width;
            float startX = 35, endX = width - 10;
            float startY = height - 30, endY =10 ;
                Font font = new Font("Arial", 9, FontStyle.Regular);
            if (null == data || data.Count == 0) {
                g.DrawString("没有数据", font, Brushes.Black,(startX+endX-g.MeasureString("没有数据",font).Width)/2,(startY+endY)/2);
                return;
            }
            int length = data.Count;
            int div = length / 5;

            float Max =data.Values.Max();
            //foreach (float value in data.Values) {
            //    if (value > Max) {
            //        Max = value;
            //    }
            //}
            float divX = (endX - startX) / length;
            float divY = (startY -endY ) / Max;
            try
            {
                //清除屏幕
                g.Clear(Color.White);
                //

                //用于画正常的线段
                Pen mypen = new Pen(Color.Blue, 1);
                mypen.DashStyle = DashStyle.Dash;
                //画坐标轴使用
                Pen mypen1 = new Pen(Color.Blue, 2);
                //用于画错误的线段
                Pen pen_error=new Pen(Color.Red, 2);
                pen_error.DashStyle = DashStyle.Dash;
                //
                Pen pen_error1 = new Pen(Color.Green, 2);
                pen_error1.DashStyle = DashStyle.Dash;
                //用于连接xy轴
                Pen dotPen = new Pen(Color.Black,0.5f);
                dotPen.DashStyle = DashStyle.Dot;
                Pen dotPen1 = new Pen(Color.Red, 0.5f);
                dotPen1.DashStyle = DashStyle.Dot;
                //画X轴
                g.DrawLine(mypen1, startX, startY, endX, startY);
                //画Y轴
                g.DrawLine(mypen1, startX, endY, startX, startY);
                float y_b = startY;
                float x_b = startX;
                float x = startX-divX;
                float y = startY;
                float value_b = 0;
                int num = 0;
                foreach(string str in data.Keys) {
                    ////x轴的字
                    //g.DrawString(".",font,Brushes.Black,x-5,startY+5);
                    float value = data[str];
                    x += divX;
                    y = startY - value * divY;
                    //
                    ////y轴的字
                    //g.DrawString("-", font, Brushes.Black, startX - 5, y);
                    if (num % div == 0&&(length-num)>=div) {
                        
                    g.DrawString(str, font, Brushes.Black, x - g.MeasureString(str, font).Width/2, startY + g.MeasureString(str, font).Height / 2);
                    g.DrawString(value.ToString(), font, Brushes.Black, startX - g.MeasureString(value.ToString(), font).Width, y- g.MeasureString(value.ToString(), font).Height/2);
                    g.DrawLine(dotPen, startX, y, x, y);
                    g.DrawLine(dotPen, x, y, x, startY);
                    }
                    if (num == length - 1) {
                        g.DrawString(str, font, Brushes.Black, endX - g.MeasureString(str, font).Width , startY + g.MeasureString(str, font).Height / 2);
                        g.DrawString(value.ToString(), font, Brushes.Black, startX - g.MeasureString(value.ToString(), font).Width, y - g.MeasureString(value.ToString(), font).Height / 2);
                        g.DrawLine(dotPen, startX, y, x, y);
                        g.DrawLine(dotPen, x, y, x, startY);
                    }
                    num++;

                    if (value_b != 0 )
                    {
                        if (value > Convert.ToDouble(Properties.Resources.Alert_Sum))
                        {
                            if (Math.Abs(x - x_b) < 1 || Math.Abs(y - y_b) < 1)
                            {
                                g.DrawLine(pen_error, x_b, y_b, x + 1, y + 1);
                            }
                            else {
                                g.DrawLine(pen_error, x_b, y_b, x, y);
                            }

                        }else  if (Math.Abs(value - value_b) > Convert.ToDouble(Properties.Resources.Alert_Value)) {

                        if (Math.Abs(x - x_b) < 1 || Math.Abs(y - y_b) < 1)
                        {
                            g.DrawLine(pen_error1, x_b, y_b, x+1, y+1);
                        }
                        else {
                        g.DrawLine(pen_error1, x_b, y_b, x, y);
                        }
                        }
                        else {
                            g.DrawLine(mypen, x_b, y_b, x, y);
                        }
                    }
                    
                    
                    y_b = y;
                    x_b = x;
                    value_b = value;

                }
                float alert = (float)(startY - Convert.ToDouble(Properties.Resources.Alert_Sum) * divY);
                g.DrawString(Properties.Resources.Alert_Sum, font, Brushes.Red, startX - 
                    g.MeasureString(Properties.Resources.Alert_Sum, font).Width, alert - g.MeasureString(Properties.Resources.Alert_Sum, font).Height / 2);
                g.DrawLine(dotPen1, startX, alert, endX, alert);
                mypen.Dispose();
                mypen1.Dispose();
                dotPen.Dispose();
                dotPen1.Dispose();
                pen_error.Dispose();
                pen_error1.Dispose();
                g.Dispose();

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        
    }
}
