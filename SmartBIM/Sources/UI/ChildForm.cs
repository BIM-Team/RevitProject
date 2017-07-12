using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Revit.Addin.RevitTooltip.Dto;
using System.Drawing.Drawing2D;

namespace Revit.Addin.RevitTooltip.UI
{
    public partial class ChildForm : Form
    {
        public ChildForm()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView2.AutoGenerateColumns = false;
            List<CEntityName> items= App.Instance.Sqlite.SelectAllEntitiesAndErr("CX");
            this.comboBox1.DisplayMember = "EntityName";
            this.comboBox1.ValueMember = "EntityName";
            this.comboBox1.DataSource = items;
            if (items.Count != 0) {
                this.comboBox1.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 引用父窗口
        /// </summary>
        public NewImageForm FatherForm { get; set; }
        //private List<CEntityName> all_entity;
        public List<CEntityName> All_Entities { set {
                comboBox1.DataSource = value;
            } }

        private List<DrawData> details = new List<DrawData>();
        

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float height = this.splitContainer1.Panel2.ClientRectangle.Height;
            float width = this.splitContainer1.Panel2.ClientRectangle.Width;
            float startX = width / 10, endX = width - 15;
            float startY = height - 15, endY = 30;
            Font font = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
            if (details.Count == 0)
            {
                g.DrawString("没有数据", font, Brushes.Black, (startX + endX - g.MeasureString("没有数据", font).Width) / 2, (startY + endY) / 2);
                return;
            }
            StringFormat format = new StringFormat();
            format.FormatFlags = StringFormatFlags.DirectionVertical;
            float MaxValue = 0L;
            float MinValue = float.MaxValue;
            int CountX = 0;
            foreach (DrawData one in details) {
                String[] arr = one.Detail.Split(';');
                int len = arr.Count();
                float v_max = one.MaxValue;
                float v_min = one.MinValue;
                if (v_max - MaxValue > 0.01){ MaxValue = v_max; }
                if (v_min - MinValue < 0.01) { MinValue = v_min; }
                if (len > CountX) { CountX = len; }
            }
            float divX = (endX - startX) /(MaxValue - MinValue) ;
            float divY =  (startY-endY)/CountX ;

            float divYY = (startY - endY) / 10;
            float divYV = (MaxValue - MinValue) / 10;
            float divXX = (endX - startX) / 10;
            float divXV = (MaxValue - MinValue) / 10;
                Pen mypen = new Pen(System.Drawing.Color.Blue, 1);
                //画坐标轴使用
                Pen mypen1 = new Pen(System.Drawing.Color.Blue, 2);
                Pen dotPen = new Pen(Color.FromArgb(128, Color.Black), 0.3f);
            try
            {
                g.Clear(System.Drawing.Color.White);
                //用于画正常的线段
                dotPen.DashStyle = DashStyle.Dot;
                //画X轴
                g.DrawLine(mypen1, startX, endY, endX, endY);
                //画Y轴
                g.DrawLine(mypen1, startX, endY, startX, startY);
                //画竖线
                for (int i = 0; i <= 10; i++)
                {
                    float newX = startX + i * divXX;
                    float v_X = (float)Math.Round(MinValue + i * divXV, 2, MidpointRounding.AwayFromZero);
                    g.DrawLine(dotPen, newX, startY, newX, endY);
                    String s_Y = v_X.ToString();
                    g.DrawString(s_Y, font, Brushes.Black, newX- g.MeasureString(s_Y, font).Width/2, endY- g.MeasureString(s_Y, font).Height);
                }
                //画横线
                for (int j = 0; j <= CountX; j++)
                {
                    float newY = endY +j * divY;
                    g.DrawLine(dotPen, startX, newY, endX, newY);
                    float v_Y = (float)(j * 0.5);
                    String s_X = v_Y.ToString();
                    if (j % 5 == 0&&CountX-j>3) {
                    g.DrawString(s_X, font, Brushes.Black, startX- g.MeasureString(s_X, font).Width, newY);
                    }
                    if (j == CountX) {
                    g.DrawString(s_X, font, Brushes.Black, startX - g.MeasureString(s_X, font).Width, newY- g.MeasureString(s_X, font).Height/2);
                    }
                }
                Random radom = new Random();
                Color color_b = Color.FromArgb(radom.Next(80, 128),radom.Next(0,128), radom.Next(0, 128));
                for(int k=0;k<details.Count;k++)
                {
                    DrawData one = details[k];
                    String[] arr = one.Detail.Split(';');
                    int len = arr.Count();
                    float pre_x = 0;
                    float pre_y = 0;
                    int add = radom.Next(32, 128);
                    int which = radom.Next(0,2);
                    int c_r = color_b.R;
                    int c_g = color_b.G;
                    int c_b = color_b.B;
                    switch (which) {
                        case 0: c_r=(color_b.R + add ) % 255;break;
                        case 1: c_g = (color_b.G + add ) % 255;break;
                        case 2:c_b= (color_b.B + add ) % 255;break;
                    }
                    
                    Color color = Color.FromArgb(c_r, c_g, c_b);
                    color_b = color;
                    SolidBrush brush = new SolidBrush(color);

                    Pen temp_pen = new Pen(color, 1);
                    for (int h = 0; h < len; h++)
                    {
                        String[] s_arr = arr[h].Split(':');
                        int y_index = (int)(Convert.ToSingle(s_arr[0]) / 0.5);
                        float v_x = Convert.ToSingle(s_arr[1]);
                        float curr_x = startX + (v_x - MinValue) * divX;
                        float curr_y = endY + y_index * divY;

                        if (h != 0)
                        {
                            g.DrawLine(temp_pen, pre_x, pre_y, curr_x, curr_y);
                            if (h == len / 2) {
                                g.DrawString(one.UniId, font, brush, (pre_x+curr_x-g.MeasureString(one.UniId,font).Width)/2,(pre_y+curr_y- g.MeasureString(one.UniId,font).Width)/2,format);
                            }
                        }
                        pre_x = curr_x;
                        pre_y = curr_y;
                    }
                    temp_pen.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                g.Dispose();
                mypen.Dispose();
                mypen1.Dispose();
                dotPen.Dispose();
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CEntityName item = comboBox1.SelectedItem as CEntityName;
            if (item != null) {
                DrawEntityData drawEntityData= App.Instance.Sqlite.SelectDrawEntityData(item.EntityName,null,null);
                this.dataGridView1.DataSource = drawEntityData.Data;
                this.dataGridView2.DataSource = null;
            }
        }

        

        private void label2_Click(object sender, EventArgs e)
        {
            details = new List<DrawData>();
            this.splitContainer1.Panel2.Invalidate(this.splitContainer1.Panel2.ClientRectangle);

        }

        private void ChildForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.FatherForm.Visible) {
                this.FatherForm.Visible = false;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            List<DrawData> dataViewSource = this.dataGridView1.DataSource as List<DrawData>;
            int index = this.dataGridView1.CurrentRow.Index;
            if (dataViewSource != null && index < dataViewSource.Count && index >= 0)
            {
                this.details.Add(dataViewSource[index]);
                this.splitContainer1.Panel2.Invalidate(this.splitContainer1.Panel2.ClientRectangle);
                this.dataGridView2.DataSource = App.Instance.Sqlite.SelectDrawData("CX", dataViewSource[this.dataGridView1.CurrentRow.Index].Date);
            }

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            List<DrawData> dataViewSource = this.dataGridView2.DataSource as List<DrawData>;
            int index = this.dataGridView2.CurrentRow.Index;
            if (dataViewSource != null && index < dataViewSource.Count && index >= 0)
            {
                this.details.Add(dataViewSource[index]);
                this.splitContainer1.Panel2.Invalidate(this.splitContainer1.Panel2.ClientRectangle);
            }
        }
    }
}
