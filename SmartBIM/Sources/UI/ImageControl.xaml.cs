using Autodesk.Revit.UI;
using Revit.Addin.RevitTooltip.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Res = Revit.Addin.RevitTooltip.Properties.Resources;


namespace Revit.Addin.RevitTooltip.UI
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl : Page, IDockablePaneProvider
    {
        private Guid m_guid = new Guid("502805E8-5698-4428-A15B-0E8BADE393E0");
        public Guid Id
        {
            get { return m_guid; }
        }
        private ImageControl()
        {
            InitializeComponent();
        }
        private static ImageControl _image;
        public static ImageControl Instance()
        {
            if (_image == null)
            {
                _image = new ImageControl();
            }
            return _image;
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CEntityName selectedItem = this.dataGrid.SelectedItem as CEntityName;
            if (selectedItem == null)
            {
                //NewImageForm.Instance().EntityData = null;
                return;
            }
            DateTime? start = this.startTime.SelectedDate as DateTime?;
            DateTime? end = this.endTime.SelectedDate as DateTime?;
            NewImageForm.Instance().EntityData = App.Instance.Sqlite.SelectDrawEntityData(selectedItem.EntityName, start, end);
            NewImageForm.Instance().Text = (this.comboBox.SelectedItem as ExcelTable)==null? "测点" + selectedItem.EntityName + "的测量数据":(this.comboBox.SelectedItem as ExcelTable).CurrentFile + ": 测点" + selectedItem.EntityName + "的测量数据";
            if (!NewImageForm.Instance().Visible)
            {
                NewImageForm.Instance().Show();
            }
            if (!string.IsNullOrWhiteSpace(selectedItem.EntityName))
            {
                App.Instance.SelectedNoInfoEntity = selectedItem.EntityName;
            }
        }
        //public void setDataSource(IEnumerable itemsSource)
        //{
        //    dataGrid.ItemsSource = itemsSource;
        //}

        private void startBox_GotFocus(object sender, RoutedEventArgs e)
        {
            startTime.Visibility = Visibility.Visible;
        }

        private void endBox_GotFocus(object sender, RoutedEventArgs e)
        {
            endTime.Visibility = Visibility.Visible;
        }

        private void startTime_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? start = startTime.SelectedDate;
            DateTime? end = endTime.SelectedDate;
            startBox.Text = Convert.ToDateTime(start).ToString("yyyy/MM/dd");
            startTime.Visibility = Visibility.Hidden;
            ExcelTable excel = this.comboBox.SelectedItem as ExcelTable;

            List<CEntityName> all_entity = new List<CEntityName>();
            if (excel != null)
            {

                all_entity = App.Instance.Sqlite.SelectAllEntitiesAndErr(excel.Signal,start,end);
            }
            this.dataGrid.ItemsSource = all_entity;

        }

        private void endTime_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? start = startTime.SelectedDate;
            DateTime? end = endTime.SelectedDate;
            endBox.Text = Convert.ToDateTime(end).ToString("yyyy/MM/dd");
            endTime.Visibility = Visibility.Hidden;
            ExcelTable excel = this.comboBox.SelectedItem as ExcelTable;

            List<CEntityName> all_entity = new List<CEntityName>();
            if (excel != null)
            {
                all_entity = App.Instance.Sqlite.SelectAllEntitiesAndErr(excel.Signal,start,end);
            }
            this.dataGrid.ItemsSource = all_entity;

        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Left;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExcelTable excel = this.comboBox.SelectedItem as ExcelTable;

            List<CEntityName> all_entity = null;
            if (excel != null)
            {
                all_entity = App.Instance.Sqlite.SelectAllEntitiesAndErr(excel.Signal);
                //初始化绘图面板
                NewImageForm.Instance().EntityData = null;
                NewImageForm.Instance().Text = "";

                //添加测斜详情的逻辑
                if (excel.Signal.Trim().Equals("CX"))
                {
                    detail.Visibility = Visibility.Visible;
                }
                else {
                    detail.Visibility = Visibility.Hidden;
                }

            }
            this.dataGrid.ItemsSource = all_entity;
            this.startTime.SelectedDate = null;
            this.startBox.Text = "";
            this.endTime.SelectedDate = null;
            this.endBox.Text = "";
        }
        public void setExcelType(System.Collections.IEnumerable itemsSource)
        {
            this.comboBox.ItemsSource = itemsSource;
            if (itemsSource != null) {
                this.comboBox.SelectedIndex = 0;
            }
        }


        private void detail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NewImageForm.Instance().Child.Show();
        }

        private void endCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            endTime.SelectedDate = null;
            endBox.Text = "";
        }

        private void startCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            startTime.SelectedDate = null;
            startBox.Text = "";

        }
    }

}
