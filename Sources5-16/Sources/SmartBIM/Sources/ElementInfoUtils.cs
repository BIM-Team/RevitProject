using Autodesk.Revit.DB;
using Res = Revit.Addin.RevitTooltip.Properties.Resources;

namespace Revit.Addin.RevitTooltip
{
    public class ElementInfoUtils
    {
        public static bool IsSurvey(Element elem)
        {
#if(Since2016)
            Parameter _param = elem.LookupParameter(Res.String_ParameterWallTypeName);
#else
            Parameter _param = elem.get_Parameter(Res.String_ParameterSurveyType);
#endif
            if (null == _param || _param.StorageType != StorageType.String)
                return false;

            return 0 == string.Compare(_param.AsString(), Res.String_UnderWallName);
        }
    }
}
