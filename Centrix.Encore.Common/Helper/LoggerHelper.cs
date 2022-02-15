using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Centrix.Encore.Common.Helper
{
    public class LoggerHelper
    {
        private static object locker = new object();
        private static void WriteLog(string Message)
        {
            string locationLog = AppDomain.CurrentDomain.BaseDirectory;// ConfigurationManager.AppSettings["LocationLog"].ToString();
            string root = locationLog + @"\FerreyrosLog\";
            string lsYear = DateTime.Today.Year.ToString().PadLeft(4, '0');
            string lsMonth = DateTime.Today.Month.ToString().PadLeft(2, '0');
            string lsDay = DateTime.Today.Day.ToString().PadLeft(2, '0');
            string lsHour = DateTime.Now.ToLongTimeString();
            root = root + @"\" + lsYear + @"\" + lsMonth + @"\";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            root += "Log" + lsDay + ".txt";
            lock (locker)
            {
                using (StreamWriter Write = new StreamWriter(root, true))
                {
                    Write.WriteLine(lsHour + "-" + Message);
                    Write.Flush();
                    Write.Close();
                    Write.Dispose();
                }
            }
        }
        public static void PutLine(string Message)
        {
            WriteLog(Message);
        }

        public static void PutStackTrace(System.Exception ex)
        {
            WriteLog("----------------------------------------------");

            WriteLog("----------------------------------------------");
            WriteLog("Exception Message :" + ex.Message);
            WriteLog("Exception Detail :" + ex.StackTrace);
            WriteLog("----------------------------------------------");

        }
    }
}
