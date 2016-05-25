//using System.IO;
//using Autodesk.Revit.DB;

//namespace Revit.Addin.RevitTooltip
//{
//    public class SettingInfo
//    {
//        public static RevitTooltip m_settings;
//        ////private static Document doc;
//        private SettingInfo()
//        {
//        }
//        private static SettingInfo _instance;
//        public static SettingInfo Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new SettingInfo();
//                }
//                return _instance;
//            }
//        }
//        /// <summary>
//        /// 刷新缓存的数据
//        /// </summary>
//        /// <param name="doc"></param>
//        /// <returns></returns>
//        internal bool Refresh(Document doc)
//        {
//            _instance = new SettingInfo();
//            m_settings = ExtensibleStorage.GetTooltipInfo(doc.ProjectInformation);
//            return Refresh();
//        }

//        private bool Refresh()
//        {
//            if (null == m_settings)
//            {
//                SetError("没有取到项目信息");
//                return false;
//            }
//            if (!File.Exists(m_settings.SurveyFile))
//            {
//                System.Diagnostics.Debug.WriteLine("监测数据文件不存在，请重新指定！-->");
//                return false;
//            }
//            if (!File.Exists(m_settings.FoundationFile))
//            {
//                SetError("基础BIM文件不存在，请重新指定！-->");
//                return false;
//            }
//            if (!File.Exists(m_settings.UnderWallFile))
//            {
//                SetError("地墙BIM文件不存在，请重新指定！-->");
//                return false;
//            }
//            if (string.IsNullOrEmpty(m_settings.DfServer))
//            {
//                SetError("服务器地址为空，请重新指定-->");
//                return false;
//            }
//            if (string.IsNullOrEmpty(m_settings.DfDB))
//            {
//                SetError("数据库为空，请重新指定-->");
//                return false;
//            }
//            if (string.IsNullOrEmpty(m_settings.DfPort))
//            {
//                SetError("端口号为空，请重新指定-->");
//                return false;
//            }
//            if (string.IsNullOrEmpty(m_settings.DfUser))
//            {
//                SetError("用户名为空，请重新指定-->");
//                return false;
//            }

//            //SurveyInfo = SurveyDataInfo.LoadSurveyFromFile(m_settings.SurveyFile);
//            //FoundationData = SurveyDataInfo.LoadBIMDataFromFile(m_settings.FoundationFile);
//            //UnderWallData = SurveyDataInfo.LoadBIMDataFromFile(m_settings.UnderWallFile);
//            //AlertNumber = m_settings.AlertNumber;
//            //AlertNumberAdd = m_settings.AlertNumberAdd;
//            return true;
//        }

//        private void SetError(string message)
//        {
//            ErrorMessage = message;
//        }

//        public string ErrorMessage { get; private set; }

//        /// <summary>
//        /// The survey data
//        /// </summary>
//        public SurveyDataInfo SurveyInfo { get; private set; }

//        /// <summary>
//        /// Foundation data
//        /// </summary>
//        public BIMDataTable FoundationData { get; private set; }

//        /// <summary>
//        /// under wall data
//        /// </summary>
//        public BIMDataTable UnderWallData { get; private set; }

//        /// <summary>
//        /// 报警数值
//        /// </summary>
//        public double AlertNumber { get; private set; }

//        /// <summary>
//        /// 监测数据的来源
//        /// </summary>
//        public double AlertNumberAdd { get; private set; }

//        //internal bool UpdateComment(bool m_isUnderWall, string elemCode, string comment)
//        //{
//        //    SurveyDataInfo.SaveComment(m_isUnderWall ? m_settings.UnderWallFile : m_settings.FoundationFile, elemCode, comment);
//        //    return Refresh();
//        //}

//        /// <summary>
//        /// return the data row from underwall table or foundation table in the memory
//        /// </summary>
//        /// <param name="isUnderWall"></param>
//        /// <param name="e"></param>
//        /// <returns></returns>
//        public BIMDataRow GetDataRow(bool isUnderWall, string elemCode)
//        {
//            var data = isUnderWall ? SettingInfo.Instance.UnderWallData : SettingInfo.Instance.FoundationData;
//            if (data == null)
//            {
//                return null;
//            }

//            foreach (var row in data.Rows)
//            {
//                string no = row.CellValues[0];
//                if (0 == string.Compare(no, elemCode))
//                    return row;
//            }
//            return null;
//        }
//    }
//}
