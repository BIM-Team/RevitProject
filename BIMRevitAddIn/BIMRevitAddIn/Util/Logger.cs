using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMRevitAddIn.Util
{
    public class Logger
    {
        //日志文件的路径
        private string m_logFile;
        public string LogFilePath {
            get {
                return this.m_logFile;
            }
        }
        //日志文件的输出流
        private StreamWriter m_logWriter;

        //
        public Logger(string logFile) {
            //设置文件路径
            this.m_logFile = Path.Combine(Path.GetDirectoryName(logFile),Path.GetFileNameWithoutExtension(logFile)+"-"
                +DateTime.Now.ToShortDateString().Replace("/","-")+
                Path.GetExtension(logFile));
            this.Log("start to run the BIMRebitAddIn.........");

        }
        //
        //写日志
        public void Log(string message) {
            if (null == m_logWriter) {
                try {
                    //目录不存在则创建目录
                    if (!Directory.Exists(Path.GetDirectoryName(m_logFile))) {
                        Directory.CreateDirectory(Path.GetDirectoryName(m_logFile));
                    }
                    m_logWriter = new StreamWriter(m_logFile,true);
                    m_logWriter.AutoFlush = true;

                } catch (IOException) {

                    return;
                }
            }
            m_logWriter.WriteLine(DateTime.Now.ToLongDateString()+DateTime.Now.ToLongTimeString()+":"+message+"\r\n");

        }
        //
        //
        //关闭logger
        public void Dispose() {
            try {
                if (null != m_logWriter) {
                    this.Log("Exit the BIMRevitAddIn......\r\n\r\n");

                    //关闭输出流
                    m_logWriter.Flush();
                    m_logWriter.Close();
                    m_logWriter = null;
                    GC.SuppressFinalize(this);
                }

            } catch (Exception){
                Trace.WriteLine("failed to close logger");
            }
        }


    }
}
