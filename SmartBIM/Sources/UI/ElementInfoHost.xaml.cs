using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Revit.Addin.RevitTooltip.Dto;

namespace Revit.Addin.RevitTooltip
{
    /// <summary>
    /// Interaction logic for FamilyHost.xaml
    /// </summary>
    public partial class ElementInfoHost : UserControl
    {
        public ElementInfoHost()
        {
            InitializeComponent();
        }

        public void Update( System.Collections.IEnumerable itemsSource)
        {
            dg.ItemsSource = itemsSource;
            
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var parameters = (List<ParameterData>)dg.ItemsSource;
                //确保第一例是测点编号列
                //存放实体编号，用于更新该实体的备注
                string entity = parameters.FirstOrDefault(p=>p.Name.Equals("构件名称")).Value;
                string comment = parameters.FirstOrDefault(p => p.Name == "备注").Value;
                if (comment != null)
                {
                    bool isOKMysql = App.Instance.MySql.ModifyEntityRemark(entity, comment);
                    bool isOKSqlite= App.Instance.Sqlite.ModifyEntityRemark(entity, comment);
                    if (isOKMysql && isOKSqlite)
                    {
                        MessageBox.Show("更新成功");
                    }
                    else if (isOKMysql)
                    {
                        MessageBox.Show("更新成功,请重新刷新本地文件");
                    }
                    else if (isOKSqlite) {
                        MessageBox.Show("本地更新成功,刷新本地文件数据将丢失");
                    }else{
                        MessageBox.Show("更新失败");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnDGSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParameterData pd = dg.SelectedItem as ParameterData;
            if (pd.Name == "备注")
            {
                var content = dg.Columns[0].GetCellContent(pd);
                dg.Columns[1].SetValue(DataGridColumn.IsReadOnlyProperty, false);
            }
            else if ((bool)dg.Columns[1].GetValue(DataGridColumn.IsReadOnlyProperty)== false)
            {
                dg.Columns[1].SetValue(DataGridColumn.IsReadOnlyProperty, true);
            }
        }
    }
}
