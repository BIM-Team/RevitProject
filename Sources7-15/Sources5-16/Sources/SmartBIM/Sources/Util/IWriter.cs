using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revit.Addin.RevitTooltip.Util
{
    public interface IWriter
    {
        int InsertSheetInfo(SheetInfo sheetInfo);
    }
}
