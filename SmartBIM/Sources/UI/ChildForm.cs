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
            this.dataGridView3.AutoGenerateColumns = false;
            List<CEntityName> items = App.Instance.Sqlite.SelectAllEntitiesAndErr("CX");
            this.comboBox1.DisplayMember = "EntityName";
            this.comboBox1.ValueMember = "EntityName";
            this.comboBox1.DataSource = items;
            if (items.Count != 0)
            {
                this.comboBox1.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 引用父窗口
        /// </summary>
        public NewImageForm FatherForm { get; set; }
        //private List<CEntityName> all_entity;
        public List<CEntityName> All_Entities
        {
            set
            {
                comboBox1.DataSource = value;
            }
        }

        private ISet<DrawData> details = new HashSet<DrawData>();

        private int selectedItem = -1;
        private int pre_select = -1;




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CEntityName item = comboBox1.SelectedItem as CEntityName;
            if (item != null)
            {
                DrawEntityData drawEntityData = App.Instance.Sqlite.SelectDrawEntityData(item.EntityName, null, null);
                this.dataGridView1.DataSource = drawEntityData.Data;
                this.dataGridView2.DataSource = null;
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {
            details = new HashSet<DrawData>();
            this.splitContainer3.Panel1.Invalidate(this.splitContainer3.Panel1.ClientRectangle);

        }

        private void ChildForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.FatherForm.Visible)
            {
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
                this.splitContainer3.Panel1.Invalidate(this.splitContainer3.Panel1.ClientRectangle);
                this.dataGridView2.DataSource = App.Instance.Sqlite.SelectDrawData("CX", dataViewSource[this.dataGridView1.CurrentRow.Index].Date);
                App.Instance.CurrentElementDayInfo= dataViewSource[index];
            }

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            List<DrawData> dataViewSource = this.dataGridView2.DataSource as List<DrawData>;
            int index = this.dataGridView2.CurrentRow.Index;
            if (dataViewSource != null && index < dataViewSource.Count && index >= 0)
            {
                this.details.Add(dataViewSource[index]);
                this.splitContainer3.Panel1.Invalidate(this.splitContainer3.Panel1.ClientRectangle);
                App.Instance.CurrentElementDayInfo = dataViewSource[index];
            }
        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float height = this.splitContainer3.Panel1.ClientRectangle.Height;
            float width = this.splitContainer3.Panel1.ClientRectangle.Width;
            float startX = width / 10, endX = width - 15;
            float startY = height - 15, endY = 30;
            Font font = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
            if (details.Count == 0)
            {
                g.DrawString("没有数据", font, Brushes.Black, (startX + endX - g.MeasureString("没有数据", font).Width) / 2, (startY + endY) / 2);
                this.dataGridView3.DataSource = null;
                return;
            }
            StringFormat format = new StringFormat();
            format.FormatFlags = StringFormatFlags.DirectionVertical;
            float MaxValue = 0f;
            float MinValue = float.MaxValue;
            int CountX = 0;
            List<DrawData> newDetails = new List<DrawData>(details);
            //更新模型
            if (this.selectedItem == -1)
            {
                this.dataGridView3.DataSource = null;
                this.dataGridView3.DataSource = newDetails;
                this.pre_select = -1;
            }
            foreach (DrawData one in newDetails)
            {
                String[] arr = one.Detail.Split(';');
                int len = arr.Count();
                float v_max = one.MaxValue;
                float v_min = one.MinValue;
                if (v_max > MaxValue) { MaxValue = v_max; }
                if (v_min < MinValue) { MinValue = v_min; }
                if (len > CountX) { CountX = len; }
            }
            float divY = (startY - endY) / CountX;

            float divXX = (endX - startX) / 10;
            float divXV = (MaxValue - MinValue) / 10;
            //对尾数进行处理，处理成尾数以5结尾
            float tail = 0.5f;
            float tail_add = 0.4f;
            float tail_reduce = 0.1f;
            float _v = divXV;
            int fix_len = 1;
            while (_v * 10.0f < 1)
            {
                _v *= 10.0f;
                tail /= 10.0f;
                tail_add /= 10.0f;
                tail_reduce /= 10.0f;
                fix_len++;
            }
            divXV = ((int)((divXV + tail_add) / tail)) * tail;


            if (MaxValue >= 0)
            {
                MaxValue = ((int)((MaxValue + divXV - tail_reduce) / divXV)) * divXV;

            }
            else
            {
                MaxValue = ((int)(MaxValue / divXV)) * divXV;
            }
            if (MinValue >= 0)
            {
                MinValue = ((int)(MinValue / divXV)) * divXV;
            }
            else
            {
                MinValue = ((int)((MinValue - divXV + tail_reduce) / divXV)) * divXV;
            }
            if ((MinValue + 10 * divXV) > MaxValue)
            {
                MaxValue = MinValue + 10 * divXV;
            }
            float divX = MaxValue - MinValue == 0f ? 0f : (endX - startX) / (MaxValue - MinValue);
            Pen mypen = new Pen(System.Drawing.Color.Blue, 1);
            //画坐标轴使用
            Pen mypen1 = new Pen(System.Drawing.Color.Blue, 2);
            Pen dotPen = new Pen(Color.FromArgb(128, Color.Black), 0.3f);
            SolidBrush brush_unselect = new SolidBrush(Color.Black);
            Pen pen_unselect = new Pen(Color.Black, 1);
            SolidBrush brush_select = new SolidBrush(Color.Red);
            Pen pen_select = new Pen(Color.Red, 3);
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
                    g.DrawLine(dotPen, newX, startY, newX, endY);
                    String s_Y = Math.Round(MinValue + i * divXV, fix_len, MidpointRounding.AwayFromZero).ToString("F" + fix_len);
                    g.DrawString(s_Y, font, Brushes.Black, newX - g.MeasureString(s_Y, font).Width / 2, endY - g.MeasureString(s_Y, font).Height);
                }
                //画横线
                for (int j = 0; j <= CountX; j++)
                {
                    float newY = endY + j * divY;
                    g.DrawLine(dotPen, startX, newY, endX, newY);
                    String s_X = (j * 0.5).ToString("F1");
                    if (j % 5 == 0 && CountX - j > 3)
                    {
                        g.DrawString(s_X, font, Brushes.Black, startX - g.MeasureString(s_X, font).Width, newY);
                    }
                    if (j == CountX)
                    {
                        g.DrawString(s_X, font, Brushes.Black, startX - g.MeasureString(s_X, font).Width, newY - g.MeasureString(s_X, font).Height / 2);
                    }
                }

                for (int k = 0; k < newDetails.Count; k++)
                {
                    DrawData one = newDetails[k];
                    String[] arr = one.Detail.Split(';');
                    int len = arr.Count();
                    float pre_x = 0;
                    float pre_y = 0;
                    SolidBrush brush_temp = brush_unselect;
                    Pen pen_temp = pen_unselect;
                    if (k == this.selectedItem)
                    {
                        brush_temp = brush_select;
                        pen_temp = pen_select;
                        this.selectedItem = -1;
                        this.pre_select = k;
                    }

                    for (int h = 0; h < len; h++)
                    {
                        String[] s_arr = arr[h].Split(':');
                        int y_index = (int)(Convert.ToSingle(s_arr[0]) / 0.5);
                        float v_x = Convert.ToSingle(s_arr[1]);
                        float curr_x = startX + (v_x - MinValue) * divX;
                        float curr_y = endY + y_index * divY;

                        if (h != 0)
                        {
                            g.DrawLine(pen_temp, pre_x, pre_y, curr_x, curr_y);
                            if (h == len / 2)
                            {
                                g.DrawString(one.UniId, font, brush_temp, (pre_x + curr_x - g.MeasureString(one.UniId, font).Width) / 2, (pre_y + curr_y - g.MeasureString(one.UniId, font).Width) / 2, format);
                            }
                        }
                        pre_x = curr_x;
                        pre_y = curr_y;
                    }

                }
                
                
                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                g.Dispose();
                mypen.Dispose();
                mypen1.Dispose();
                dotPen.Dispose();
                pen_unselect.Dispose();
                pen_select.Dispose();
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (this.pre_select != e.RowIndex)
            {
                this.selectedItem = e.RowIndex;
                this.splitContainer3.Panel1.Invalidate(this.splitContainer3.Panel1.ClientRectangle);
            }

        }
    }
}
