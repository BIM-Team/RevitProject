using Revit.Addin.RevitTooltip.Dto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Revit.Addin.RevitTooltip.UI
{
    public partial class NewImageForm : Form
    {
        private static NewImageForm _form;
        public static NewImageForm Instance()
        {
            if (_form == null || _form.IsDisposed)
            {
                _form = new NewImageForm();
            }
            return _form;
        }
        private NewImageForm()
        {
            InitializeComponent();
        }

        private DrawEntityData _entityData;
        /// <summary>
        /// 保存某一个实体的数据
        /// </summary>
        public DrawEntityData EntityData
        {
            set
            {
                if (null == value || !value.Equals(this._entityData))
                {
                    this._entityData = value;
                    this.panel1.Invalidate(this.panel1.ClientRectangle);
                }
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float height = this.panel1.ClientRectangle.Height;
            float width = this.panel1.ClientRectangle.Width;
            float startX = width/10, endX = width - 10;
            float startY = height - 30, endY = 10;
            Font font = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
            if (null == _entityData || _entityData.Data.Count == 0)
            {
                g.DrawString("没有数据", font, Brushes.Black, (startX + endX - g.MeasureString("没有数据", font).Width) / 2, (startY + endY) / 2);
                return;
            }
            List<DrawData> data1 = _entityData.Data;
            List<DrawData> data2 = null;
            bool isNegative = false;
            if (_entityData.Total_hold < 0)
            {
                isNegative = true;
                data2 = new List<DrawData>();
                foreach (DrawData one in data1)
                {
                    DrawData newOne = new DrawData();
                    newOne.Date = one.Date;
                    newOne.Detail = one.Detail;
                    newOne.MaxValue = -one.MaxValue;
                    newOne.MidValue = -one.MidValue;
                    newOne.MinValue = -one.MinValue;
                    data2.Add(newOne);
                }
            }
            else
            {
                data2 = data1;
            }

            float diff_hold = Math.Abs(_entityData.Diff_hold);
            float total_hold = Math.Abs(_entityData.Total_hold);
            int length = data2.Count;
            int div = length / 5;

            float Max = data2[0].MaxValue;
            float Min = Max;
            foreach (DrawData row in data2)
            {
                if (Max < row.MaxValue)
                {
                    Max = row.MaxValue;
                }
                if (Min > row.MaxValue)
                {
                    Min = row.MaxValue;
                }
            }
            float divX = (endX - startX) / length;
            float divY = (startY - endY) / (Max - Min);
            try
            {
                //清除屏幕
                g.Clear(System.Drawing.Color.White);
                //

                //用于画正常的线段
                Pen mypen = new Pen(System.Drawing.Color.Blue, 1);
                mypen.DashStyle = DashStyle.Dash;
                //画坐标轴使用
                Pen mypen1 = new Pen(System.Drawing.Color.Blue, 2);
                //用于画错误的线段
                Pen pen_error = new Pen(System.Drawing.Color.Red, 2);
                pen_error.DashStyle = DashStyle.Dash;
                //
                Pen pen_error1 = new Pen(System.Drawing.Color.Green, 2);
                pen_error1.DashStyle = DashStyle.Dash;
                //用于连接xy轴
                Pen dotPen = new Pen(Color.FromArgb(128, Color.Black), 0.3f);
                dotPen.DashStyle = DashStyle.Dot;
                Pen dotPen1 = new Pen(System.Drawing.Color.Red, 0.5f);
                dotPen1.DashStyle = DashStyle.Dot;
                //画X轴
                g.DrawLine(mypen1, startX, startY, endX, startY);
                //画Y轴
                g.DrawLine(mypen1, startX, endY, startX, startY);

                float div_height = (startY - endY) / 10;
                float div_width = (endX - startX) / 10;
                float div_value = (Max - Min) / 10;
                //画10*10网格
                for (int i = 0; i <= 10; i++)
                {
                    float y10 = startY - i * div_height;
                    float x10 = startX + i * div_width;
                    string str_va = Math.Round((div_value * i + Min)*(isNegative?-1:1), 2, MidpointRounding.AwayFromZero).ToString();
                    g.DrawLine(dotPen, startX, y10, endX, y10);
                    g.DrawLine(dotPen, x10, startY, x10, endY);
                    g.DrawString(str_va, font, Brushes.Black, startX - g.MeasureString(str_va, font).Width, y10);
                }
                float y_b = startY;
                float x_b = startX;
                float x = startX - divX;
                float y = startY;
                float value_b = 0;
                //int num = 0;
                for (int i = 0; i < data2.Count; i++)
                {
                    ////x轴的字
                    float value = data2[i].MaxValue;
                    string str = data1[i].Date.ToShortDateString();
                    string str_value = data1[i].MaxValue.ToString();

                    x += divX;
                    y = startY - (value - Min) * divY;
                    //
                    ////y轴的字
                    if (i == 0)
                    {
                        g.DrawString(str, font, Brushes.Black, x, startY + g.MeasureString(str, font).Height / 2);
                    }
                    if (i == length - 1)
                    {
                        g.DrawString(str, font, Brushes.Black, endX - g.MeasureString(str, font).Width, startY + g.MeasureString(str, font).Height / 2);
                    }

                    if (value_b != 0)
                    {
                        if (value > total_hold)
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
                        else if (Math.Abs(value - value_b) > diff_hold)
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
                if (total_hold - Min > 0&&total_hold<Max)
                {
                    float alert = (float)(startY - (total_hold - Min) * divY);
                    string str_alert = _entityData.Total_hold.ToString();
                    g.DrawString(str_alert, font, Brushes.Red, (startX + endX) / 2 -
                        g.MeasureString(str_alert, font).Width, alert - g.MeasureString(str_alert, font).Height / 2);
                    g.DrawLine(dotPen1, startX, alert, endX, alert);
                }

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

        private void NewImageForm_Resize(object sender, EventArgs e)
        {
            this.panel1.Invalidate(this.panel1.ClientRectangle);
        }
    }
}
