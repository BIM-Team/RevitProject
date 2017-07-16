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
            _child = new ChildForm();
            _child.FatherForm = this;
        }
        /// <summary>
        /// 引用子窗口
        /// </summary>
        private ChildForm _child;
        public ChildForm Child
        {
            get
            {
                if (_child == null || _child.IsDisposed)
                {
                    _child = new ChildForm();
                    _child.FatherForm = this;
                }
                return _child;
            }
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
            float startX = width / 10, endX = width - 10;
            float startY = height - 30, endY = 10;
            Font font = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
            if (null == _entityData || _entityData.Data.Count == 0)
            {
                g.DrawString("没有数据", font, Brushes.Black, (startX + endX - g.MeasureString("没有数据", font).Width) / 2, (startY + endY) / 2);
                return;
            }
            List<DrawData> data2 = _entityData.Data;
            string[] diff_holds = _entityData.Diff_hold.Split(new char[] { ',', '，' });
            string[] total_holds = _entityData.Total_hold.Split(new char[] { ',', '，' });
            float diff_hold1 = Convert.ToSingle(diff_holds[0]);
            float diff_hold2 = 0;
            float total_hold1 = Convert.ToSingle(total_holds[0]);
            float total_hold2 = 0;
            if (diff_holds.Length > 1)
            {
                diff_hold2 = Convert.ToSingle(diff_holds[1]);
            }
            if (total_holds.Length > 1)
            {
                total_hold2 = Convert.ToSingle(total_holds[1]);
            }
            string totalOpr = _entityData.TotalOperator;
            string diffOpr = _entityData.DiffOperator;
            int length = data2.Count;

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
            //y轴的梯度值
            float div_y10 = (Max - Min) / 10;
            //对梯度值做尾数处理，处理成0.5,0.05,0.005,0.0005,0.00005形式
            //处理成对等精度
            float tail = 0.5f;
            float tail_add = 0.4f;
            float tail_reduce = 0.1f;
            float _v = div_y10;
            int fix_len = 1;
            while (_v * 10.0f < 1)
            {
                _v *= 10.0f;
                tail /= 10.0f;
                tail_add /= 10.0f;
                tail_reduce /= 10.0f;
                fix_len++;
            }
            div_y10 = ((int)((div_y10 + tail_add) / tail)) * tail;


            if (Max >= 0)
            {
                Max = ((int)((Max + div_y10 - tail_reduce) / div_y10)) * div_y10;
            }
            else
            {
                Max = ((int)(Max / div_y10)) * div_y10;
            }
            if (Min >= 0)
            {
                Min = ((int)(Min / div_y10)) * div_y10;
            }
            else
            {
                Min = ((int)((Min - div_y10 + tail_reduce) / div_y10)) * div_y10;
            }
            if ((Min + 10 * div_y10) > Max)
            {
                Max = Min + 10 * div_y10;
            }

            


            float divX = (endX - startX) / length;
            float divY = (Max - Min) == 0 ? 0 : (startY - endY) / (Max - Min);
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
                float div_value = div_y10;
                //画10*10网格
                for (int i = 0; i <= 10; i++)
                {
                    float y10 = startY - i * div_height;
                    float x10 = startX + i * div_width;
                    string str_va = Math.Round((div_value * i + Min), fix_len, MidpointRounding.AwayFromZero).ToString("F" + fix_len);
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
                    string str = data2[i].Date.ToShortDateString();
                    string str_value = data2[i].MaxValue.ToString();

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


                    bool totalResult = false;
                    if (totalOpr.Equals(">"))
                    {
                        totalResult = value - total_hold1 > 0.00001f || value_b - total_hold1 > 0.000001f;
                    }
                    else if (totalOpr.Equals(">="))
                    {
                        totalResult = value - total_hold1 >= 0.00001f || value_b - total_hold1 >= 0.00001f;
                    }
                    else if (totalOpr.Equals("<"))
                    {
                        totalResult = value - total_hold1 < -0.00001f || value_b - total_hold1 < 0.00001f;
                    }
                    else if (totalOpr.Equals("<="))
                    {
                        totalResult = value - total_hold1 <= -0.00001f || value_b - total_hold1 <= 0.00001f;
                    }
                    else
                    {
                        float total_max = Math.Max(total_hold1, total_hold2);
                        float total_min = Math.Min(total_hold1, total_hold2);
                        if (totalOpr.Equals("IN"))
                        {
                            totalResult = value > total_min && value < total_max;
                            totalResult = totalResult || value_b > total_min && value_b < total_max;
                        }
                        else
                        {
                            totalResult = value < total_min || value > total_max;
                            totalResult = totalResult || value_b < total_min || value_b > total_max;
                        }

                    }
                    bool diffResult = false;
                    float diff = Math.Abs(value - value_b);
                    if (diffOpr.Equals(">"))
                    {
                        diffResult = diff - diff_hold1 > 0.00001f;
                    }
                    else if (diffOpr.Equals(">="))
                    {
                        diffResult = diff - diff_hold1 >= 0.00001f;
                    }
                    else if (diffOpr.Equals("<"))
                    {
                        diffResult = diff - diff_hold1 < -0.00001f;
                    }
                    else if (diffOpr.Equals("<="))
                    {
                        diffResult = diff - diff_hold1 <= -0.00001f;
                    }
                    else
                    {
                        float diff_max = Math.Max(diff_hold1, diff_hold2);
                        float diff_min = Math.Min(diff_hold1, diff_hold2);
                        if (totalOpr.Equals("IN"))
                        {
                            diffResult = diff > diff_min && diff < diff_max;
                        }
                        else
                        {
                            diffResult = diff < diff_min || diff > diff_max;
                        }

                    }
                    if (totalResult)
                    {
                        if (total_hold1 > Min && (value_b - total_hold1) * (value - total_hold1) < 0)
                        {
                            float y_mid = startY - (total_hold1 - Min) * divY;
                            float x_mid = x_b;
                            if (y - y_b != 0)
                            {
                                x_mid = (y_mid - y_b) * (x - x_b) / (y - y_b) + x_b;
                            }
                            if (totalOpr.Equals("<") || totalOpr.Equals("<=") || totalOpr.Equals("OUT"))
                            {
                                g.DrawLine(pen_error, x_b, y_b, x_mid, y_mid);
                                g.DrawLine(mypen, x_mid, y_mid, x, y);
                            }
                            else
                            {
                                g.DrawLine(mypen, x_b, y_b, x_mid, y_mid);
                                g.DrawLine(pen_error, x_mid, y_mid, x, y);
                            }

                        }
                        else if (total_hold2 > Min && (value_b - total_hold2) * (value - total_hold2) < 0)
                        {
                            float y_mid = startY - (total_hold2 - Min) * divY;
                            float x_mid = x_b;
                            if (y - y_b != 0)
                            {
                                x_mid = (y_mid - y_b) * (x - x_b) / (y - y_b) + x_b;
                            }
                            if (totalOpr.Equals("OUT"))
                            {
                                g.DrawLine(mypen, x_b, y_b, x_mid, y_mid);
                                g.DrawLine(pen_error, x_mid, y_mid, x, y);
                            }
                            else if (totalOpr.Equals("IN"))
                            {
                                g.DrawLine(pen_error, x_b, y_b, x_mid, y_mid);
                                g.DrawLine(mypen, x_mid, y_mid, x, y);
                            }

                        }
                        else
                        {
                            g.DrawLine(pen_error, x_b, y_b, x, y);
                        }



                    }
                    else if (diffResult && i > 0)
                    {
                        g.DrawLine(pen_error1, x_b, y_b, x, y);
                    }
                    else
                    {
                        g.DrawLine(mypen, x_b, y_b, x, y);
                    }

                    y_b = y;
                    x_b = x;
                    value_b = value;

                }
                if (total_hold1 - Min > 0 && total_hold1 < Max)
                {
                    float alert = (float)(startY - (total_hold1 - Min) * divY);
                    string str_alert = total_hold1.ToString();
                    g.DrawString(str_alert, font, Brushes.Red, (startX + endX) / 2 -
                        g.MeasureString(str_alert, font).Width, alert - g.MeasureString(str_alert, font).Height / 2);
                    g.DrawLine(dotPen1, startX, alert, endX, alert);
                }
                if (total_hold2 - Min > 0 && total_hold2 < Max)
                {
                    float alert = (float)(startY - (total_hold2 - Min) * divY);
                    string str_alert = total_hold2.ToString();
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
