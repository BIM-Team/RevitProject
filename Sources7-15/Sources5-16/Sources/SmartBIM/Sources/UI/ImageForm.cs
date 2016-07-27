using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Addin.RevitTooltip.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static Revit.Addin.RevitTooltip.App;
using Res = Revit.Addin.RevitTooltip.Properties.Resources;

namespace Revit.Addin.RevitTooltip.UI
{
    public partial class ImageForm : System.Windows.Forms.Form
    {
        private static ImageForm _form = null;
        private ExternalCommandData commandData = null;
        public ExternalCommandData CommandData
        {
            set
            {
                this.commandData = value;
            }
        }
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
                Pen dotPen = new Pen(System.Drawing.Color.Black, 0.5f);
                dotPen.DashStyle = DashStyle.Dot;
                Pen dotPen1 = new Pen(System.Drawing.Color.Red, 0.5f);
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
                    float value = data[str];
                    x += divX;
                    y = startY - value * divY;
                    //
                    ////y轴的字
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

     
   
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string selection = string.Empty;
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != null)
            {
                selection = dataGridView1.CurrentCell.Value.ToString();
            }
            if (!string.IsNullOrEmpty(selection))
            {
                try
                {
                    this.EntityName = selection;

                    //
                    BuiltInParameter testParam = BuiltInParameter.ALL_MODEL_TYPE_NAME;
                    ParameterValueProvider pvp = new ParameterValueProvider(new ElementId(testParam));
                    FilterStringEquals eq = new FilterStringEquals();
                    FilterRule rule = new FilterStringRule(pvp, eq, Res.String_ParameterSurveyType, false);
                    ElementParameterFilter paramFilter = new ElementParameterFilter(rule);


                    //ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
                    Document document = this.commandData.Application.ActiveUIDocument.Document;
                    Autodesk.Revit.DB.View view = this.commandData.Application.ActiveUIDocument.ActiveView;

                    IList<UIView> uiViews = this.commandData.Application.ActiveUIDocument.GetOpenUIViews();
                    UIView currentUIView = null;
                    foreach (UIView ui in uiViews)
                    {
                        if (ui.ViewId.Equals(view.Id))
                        {
                            currentUIView = ui;
                        }
                    }

                    FilteredElementCollector elementCollector = new FilteredElementCollector(document);
                    IList<Element> elems = elementCollector.WherePasses(paramFilter).ToElements();
                    foreach (var elem in elems)
                    {
                        string param_value = string.Empty;
                        Parameter param = elem.get_Parameter(Res.String_ParameterName);
                        if (null != param && param.StorageType == StorageType.String)
                        {
                            param_value = param.AsString();
                        }
                        if (selection.Equals(param_value))
                        {
                            this.commandData.Application.ActiveUIDocument.ShowElements(elem.Id);
                            this.commandData.Application.ActiveUIDocument.Selection.Elements.Clear();
                            this.commandData.Application.ActiveUIDocument.Selection.Elements.Add(elem);
                            IList<XYZ> corners = currentUIView.GetZoomCorners();
                            XYZ center = (corners[0] + corners[1]) / 2;
                            XYZ div = new XYZ(25, 25, 25);
                            currentUIView.ZoomAndCenterRectangle(center - div, center + div);
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("异常" + ex.Message);
                }
            }
            }

        private void ImageForm_VisibleChanged(object sender, EventArgs e)
        {
            if (((ImageForm)sender).Visible == true) {
            List<ParameterData> errorCXs = SQLiteHelper.CreateInstance().SelectExceptionalCX(App.settings.AlertNumber, App.settings.AlertNumberAdd);
            this.dataGridView1.DataSource = errorCXs;
            List<string> listCX = new List<string>();
            foreach (ParameterData param in errorCXs)
            {
                listCX.Add(param.Name);
            }
            try
            {
                //
                BuiltInParameter testParam = BuiltInParameter.ALL_MODEL_TYPE_NAME;
                ParameterValueProvider pvp = new ParameterValueProvider(new ElementId(testParam));
                FilterStringEquals eq = new FilterStringEquals();
                FilterRule rule = new FilterStringRule(pvp, eq, Res.String_ParameterSurveyType, false);
                ElementParameterFilter paramFilter = new ElementParameterFilter(rule);


                Document document = this.commandData.Application.ActiveUIDocument.Document;
                Autodesk.Revit.DB.View view = this.commandData.Application.ActiveUIDocument.ActiveView;

                IList<UIView> uiViews = this.commandData.Application.ActiveUIDocument.GetOpenUIViews();
                UIView currentUIView = null;
                foreach (UIView ui in uiViews)
                {
                    if (ui.ViewId.Equals(view.Id))
                    {
                        currentUIView = ui;
                    }
                }

                FilteredElementCollector elementCollector = new FilteredElementCollector(document);
                IList<Element> elems = elementCollector.WherePasses(paramFilter).ToElements();
                this.commandData.Application.ActiveUIDocument.Selection.Elements.Clear();
                ICollection<ElementId> elemIds = new List<ElementId>();
                foreach (var elem in elems)
                {
                    string param_value = string.Empty;
                    Parameter param = elem.get_Parameter(Res.String_ParameterName);
                    if (null != param && param.StorageType == StorageType.String)
                    {
                        param_value = param.AsString();
                    }
                    if (listCX.Contains(param_value))
                    {
                        this.commandData.Application.ActiveUIDocument.Selection.Elements.Add(elem);
                        elemIds.Add(elem.Id);
                    }
                }
                this.commandData.Application.ActiveUIDocument.ShowElements(elemIds);
                //IList<XYZ> corners = currentUIView.GetZoomCorners();
                //XYZ center = (corners[0] + corners[1]) / 2;
                //XYZ div = new XYZ(25, 25, 25);
                //currentUIView.ZoomAndCenterRectangle(center - div, center + div);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("异常" + ex.Message);
            }
        }
      }
    }
}
