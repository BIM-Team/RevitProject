using Revit.Addin.RevitTooltip.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static Revit.Addin.RevitTooltip.App;

namespace Revit.Addin.RevitTooltip.UI
{
    public partial class ImageForm : Form
    {
        private static ImageForm _form = null;
        // private MysqlUtil mysql;
        //实体名称
        private string entityName;
        public string EntityName
        {//初始化使用
            set
            {

                if (null != value && !value.Equals(entityName))
                {
                    try
                    {
                        //mysql = MysqlUtil.CreateInstance();
                        //data = mysql.SelectOneCXData(value);
                        data = SQLiteHelper.CreateInstance().SelectOneCXData(value);
                        this.Text = value + "的测试数据";
                        this.entityName = value;
                        this.splitContainer1.Panel2.Invalidate(this.splitContainer1.Panel2.ClientRectangle);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
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
        public static ImageForm GetInstance()
        {
            if (_form == null)
            {
                _form = new ImageForm();
            }
            return _form;
        }

        private ImageForm()
        {

            InitializeComponent();
        }

        //private void ImageForm_Load(object sender, EventArgs e)
        //{
        //   // this.Invalidate(this.ClientRectangle);
        //}


        private void ImageForm_Paint(object sender, PaintEventArgs e)
        {

            //Graphics g = e.Graphics;

            //float height = this.splitContainer1.ClientRectangle.Height;
            //float width = this.splitContainer1.ClientRectangle.Width;
            //float startX = this.splitContainer1.Panel1.ClientSize.Width+40, endX = width - 10;
            //float startY = height - 30, endY =10 ;
            //    Font font = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
            //if (null == data || data.Count == 0) {
            //    g.DrawString("没有数据", font, Brushes.Black,(startX+endX-g.MeasureString("没有数据",font).Width)/2,(startY+endY)/2);
            //    return;
            //}
            //int length = data.Count;
            //int div = length / 5;

            //float Max =data.Values.Max();
            //float divX = (endX - startX) / length;
            //float divY = (startY -endY ) / Max;
            //try
            //{
            //    //清除屏幕
            //    g.Clear(Color.White);
            //    //

            //    //用于画正常的线段
            //    Pen mypen = new Pen(Color.Blue, 1);
            //    mypen.DashStyle = DashStyle.Dash;
            //    //画坐标轴使用
            //    Pen mypen1 = new Pen(Color.Blue, 2);
            //    //用于画错误的线段
            //    Pen pen_error=new Pen(Color.Red, 2);
            //    pen_error.DashStyle = DashStyle.Dash;
            //    //
            //    Pen pen_error1 = new Pen(Color.Green, 2);
            //    pen_error1.DashStyle = DashStyle.Dash;
            //    //用于连接xy轴
            //    Pen dotPen = new Pen(Color.Black,0.5f);
            //    dotPen.DashStyle = DashStyle.Dot;
            //    Pen dotPen1 = new Pen(Color.Red, 0.5f);
            //    dotPen1.DashStyle = DashStyle.Dot;
            //    //画X轴
            //    g.DrawLine(mypen1, startX, startY, endX, startY);
            //    //画Y轴
            //    g.DrawLine(mypen1, startX, endY, startX, startY);
            //    float y_b = startY;
            //    float x_b = startX;
            //    float x = startX-divX;
            //    float y = startY;
            //    float value_b = 0;
            //    int num = 0;
            //    foreach(string str in data.Keys) {
            //        ////x轴的字
            //        //g.DrawString(".",font,Brushes.Black,x-5,startY+5);
            //        float value = data[str];
            //        x += divX;
            //        y = startY - value * divY;
            //        //
            //        ////y轴的字
            //        //g.DrawString("-", font, Brushes.Black, startX - 5, y);
            //        if (num % div == 0&&(length-num)>=div) {

            //        g.DrawString(str, font, Brushes.Black, x - g.MeasureString(str, font).Width/2, startY + g.MeasureString(str, font).Height / 2);
            //        g.DrawString(value.ToString(), font, Brushes.Black, startX - g.MeasureString(value.ToString(), font).Width, y- g.MeasureString(value.ToString(), font).Height/2);
            //        g.DrawLine(dotPen, startX, y, x, y);
            //        g.DrawLine(dotPen, x, y, x, startY);
            //        }
            //        if (num == length - 1) {
            //            g.DrawString(str, font, Brushes.Black, endX - g.MeasureString(str, font).Width , startY + g.MeasureString(str, font).Height / 2);
            //            g.DrawString(value.ToString(), font, Brushes.Black, startX - g.MeasureString(value.ToString(), font).Width, y - g.MeasureString(value.ToString(), font).Height / 2);
            //            g.DrawLine(dotPen, startX, y, x, y);
            //            g.DrawLine(dotPen, x, y, x, startY);
            //        }
            //        num++;

            //        if (value_b != 0 )
            //        {
            //            if (value > App.settings.AlertNumber)
            //            {
            //                if (Math.Abs(x - x_b) < 1 || Math.Abs(y - y_b) < 1)
            //                {
            //                    g.DrawLine(pen_error, x_b, y_b, x + 1, y + 1);
            //                }
            //                else {
            //                    g.DrawLine(pen_error, x_b, y_b, x, y);
            //                }

            //            }else  if (Math.Abs(value - value_b) > App.settings.AlertNumberAdd) {

            //            if (Math.Abs(x - x_b) < 1 || Math.Abs(y - y_b) < 1)
            //            {
            //                g.DrawLine(pen_error1, x_b, y_b, x+1, y+1);
            //            }
            //            else {
            //            g.DrawLine(pen_error1, x_b, y_b, x, y);
            //            }
            //            }
            //            else {
            //                g.DrawLine(mypen, x_b, y_b, x, y);
            //            }
            //        }


            //        y_b = y;
            //        x_b = x;
            //        value_b = value;

            //    }
            //    float alert = (float)(startY - App.settings.AlertNumber * divY);
            //    g.DrawString(App.settings.AlertNumber.ToString(), font, Brushes.Red, startX - 
            //        g.MeasureString(App.settings.AlertNumber.ToString(), font).Width, alert - g.MeasureString(App.settings.AlertNumber.ToString(), font).Height / 2);
            //    g.DrawLine(dotPen1, startX, alert, endX, alert);
            //    mypen.Dispose();
            //    mypen1.Dispose();
            //    dotPen.Dispose();
            //    dotPen1.Dispose();
            //    pen_error.Dispose();
            //    pen_error1.Dispose();
            //    g.Dispose();

            //}
            //catch (Exception exception)
            //{
            //    throw exception;
            //}
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float height = this.splitContainer1.ClientRectangle.Height;
            float width = this.splitContainer1.Panel2.ClientRectangle.Width;
            float startX = 40, endX = width - 10;
            float startY = height - 30, endY = 10;
            Font font = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
            if (null == data || data.Count == 0)
            {
                g.DrawString("没有数据", font, Brushes.Black, (startX + endX - g.MeasureString("没有数据", font).Width) / 2, (startY + endY) / 2);
                return;
            }
            int length = data.Count;
            int div = length / 5;

            float Max = data.Values.Max();
            float divX = (endX - startX) / length;
            float divY = (startY - endY) / Max;
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
                Pen pen_error = new Pen(Color.Red, 2);
                pen_error.DashStyle = DashStyle.Dash;
                //
                Pen pen_error1 = new Pen(Color.Green, 2);
                pen_error1.DashStyle = DashStyle.Dash;
                //用于连接xy轴
                Pen dotPen = new Pen(Color.Black, 0.5f);
                dotPen.DashStyle = DashStyle.Dot;
                Pen dotPen1 = new Pen(Color.Red, 0.5f);
                dotPen1.DashStyle = DashStyle.Dot;
                //画X轴
                g.DrawLine(mypen1, startX, startY, endX, startY);
                //画Y轴
                g.DrawLine(mypen1, startX, endY, startX, startY);
                float y_b = startY;
                float x_b = startX;
                float x = startX - divX;
                float y = startY;
                float value_b = 0;
                int num = 0;
                foreach (string str in data.Keys)
                {
                    ////x轴的字
                    //g.DrawString(".",font,Brushes.Black,x-5,startY+5);
                    float value = data[str];
                    x += divX;
                    y = startY - value * divY;
                    //
                    ////y轴的字
                    //g.DrawString("-", font, Brushes.Black, startX - 5, y);
                    if (num % div == 0 && (length - num) >= div)
                    {

                        g.DrawString(str, font, Brushes.Black, x - g.MeasureString(str, font).Width / 2, startY + g.MeasureString(str, font).Height / 2);
                        g.DrawString(value.ToString(), font, Brushes.Black, startX - g.MeasureString(value.ToString(), font).Width, y - g.MeasureString(value.ToString(), font).Height / 2);
                        g.DrawLine(dotPen, startX, y, x, y);
                        g.DrawLine(dotPen, x, y, x, startY);
                    }
                    if (num == length - 1)
                    {
                        g.DrawString(str, font, Brushes.Black, endX - g.MeasureString(str, font).Width, startY + g.MeasureString(str, font).Height / 2);
                        g.DrawString(value.ToString(), font, Brushes.Black, startX - g.MeasureString(value.ToString(), font).Width, y - g.MeasureString(value.ToString(), font).Height / 2);
                        g.DrawLine(dotPen, startX, y, x, y);
                        g.DrawLine(dotPen, x, y, x, startY);
                    }
                    num++;

                    if (value_b != 0)
                    {
                        if (value > App.settings.AlertNumber)
                        {
                            if (Math.Abs(x - x_b) < 1 || Math.Abs(y - y_b) < 1)
                            {
                                g.DrawLine(pen_error, x_b, y_b, x + 1, y + 1);
                            }
                            else
                            {
                                g.DrawLine(pen_error, x_b, y_b, x, y);
                            }

                        }
                        else if (Math.Abs(value - value_b) > App.settings.AlertNumberAdd)
                        {

                            if (Math.Abs(x - x_b) < 1 || Math.Abs(y - y_b) < 1)
                            {
                                g.DrawLine(pen_error1, x_b, y_b, x + 1, y + 1);
                            }
                            else
                            {
                                g.DrawLine(pen_error1, x_b, y_b, x, y);
                            }
                        }
                        else
                        {
                            g.DrawLine(mypen, x_b, y_b, x, y);
                        }
                    }


                    y_b = y;
                    x_b = x;
                    value_b = value;

                }
                float alert = (float)(startY - App.settings.AlertNumber * divY);
                g.DrawString(App.settings.AlertNumber.ToString(), font, Brushes.Red, startX -
                    g.MeasureString(App.settings.AlertNumber.ToString(), font).Width, alert - g.MeasureString(App.settings.AlertNumber.ToString(), font).Height / 2);
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

        private void ImageForm_Load(object sender, EventArgs e)
        {
           List<ParameterData> errorCXs= SQLiteHelper.CreateInstance().SelectExceptionalCX(App.settings.AlertNumber, App.settings.AlertNumberAdd);
            this.dataGridView1.DataSource = errorCXs;
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            this.EntityName=dataGridView1.CurrentCell.Value.ToString();
        }
    }
}
