using Res = Revit.Addin.RevitTooltip.Properties.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

using BIMCoder.OfficeHelper.ExcelCommon;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Addin.RevitTooltip.Properties;
using System.Windows.Forms;
using System.Windows.Data;
using System.Globalization;

namespace Revit.Addin.RevitTooltip
{
    class Utils
    {
        /// <summary>
        /// Convert Bitmap to BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ConvertFromBitmap(System.Drawing.Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Return currently active UIView or null.
        /// </summary>
        public static UIView GetActiveUiView(
          UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Autodesk.Revit.DB.View view = doc.ActiveView;
            IList<UIView> uiviews = uidoc.GetOpenUIViews();
            UIView uiview = null;

            foreach (UIView uv in uiviews)
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiview = uv;
                    break;
                }
            }
            return uiview;
        }

       

        public static Parameter GetParameter(Element elem, string param)
        {
#if(Since2016)
            return elem.LookupParameter(param);
#else
            return elem.get_Parameter(param);
#endif
        }

        public static string GetParameterValueAsString(Element elem, string parameterName)
        {
#if(Since2016)
            Parameter _param = elem.LookupParameter(parameterName);
#else
            Parameter _param = elem.get_Parameter(parameterName);
#endif
            if (null == _param || _param.StorageType != StorageType.String)
                return null;

            return _param.AsString();
        }
    }
    
}
