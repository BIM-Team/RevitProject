using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;

namespace Revit.Addin.RevitTooltip
{
    /// <summary>
    /// Interaction logic for FamilyBrowserPanel.xaml
    /// </summary>
    public partial class ElementInfoPanel : Page, IDockablePaneProvider
    {
        private Guid m_guid = new Guid("B6949305-A575-4CB0-9364-08A5FD139A46");
        private static ElementInfoPanel s_instance = null;
        private ElementInfoPanel()
        {
            InitializeComponent();
        }

        public void Update(System.Collections.IEnumerable itemsSource)
        {
            elementInfoHost.Update( itemsSource);
        }

        public static ElementInfoPanel GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ElementInfoPanel();
            }
            return s_instance;
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Left;
        }

        public Guid Id
        {
            get { return m_guid; }
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e == null)
                return;

            double height = e.NewSize.Height;
            double width = e.NewSize.Width;
            try
            {
                float defaultDpiX = 96.0F;
                float defaultDpiY = 96.0F;

                using (Graphics graphic = Graphics.FromHwnd(IntPtr.Zero))
                {
                    float currentDpiX = graphic.DpiX;
                    width = defaultDpiX / currentDpiX * width;
                    float currentDpiY = graphic.DpiY;
                    height = defaultDpiY / currentDpiY * height;
                }
            }
            catch (Exception)
            {
                //
            }
            finally
            {
                elementInfoHost.Height = height;
                elementInfoHost.Width = width;

                elementInfoHost.BringIntoView();
            }
        }
    }
}
