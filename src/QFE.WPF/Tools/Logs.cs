using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QFE.WPF.Tools
{
    public class Logs
    {
        #region Logs
        public static void WriteLog(String StrMessage)
        {
            String Filename = null;
            string Dirs = null;
            string path = getPath();

            Filename = "TRANSLOG-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".log";
            Dirs = "\\Logs\\";

            DirectoryInfo drInfo = new DirectoryInfo(path + Dirs);
            if (!drInfo.Exists)
            {
                drInfo.Create();
            }
            Filename = path + Dirs + Filename;
            if (File.Exists(Filename))
            {
                FileWriter.AppendToFile(StrMessage, Filename);
            }
            else
            {
                FileWriter.WriteFile(StrMessage, Filename);
            }
        }
        public static string getPath()
        {
            String Pth = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            Pth = Pth.Remove(0, 6);
            return Pth;
        }
        public static void RemoveLog()
        {
            String Filename = null;
            string Dirs = null;
            string path = getPath();

            Filename = "TRANSLOG-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".log";
            Dirs = "\\Logs\\";

            DirectoryInfo drInfo = new DirectoryInfo(path + Dirs);
            if (!drInfo.Exists)
            {
                return;
            }
            FileInfo[] AllFiles = drInfo.GetFiles();
            Filename = path + Dirs + Filename;
            foreach (FileInfo MyLogFile in AllFiles)
            {
                if (MyLogFile.FullName.ToLower() != Filename.ToLower())
                {
                    MyLogFile.Delete();
                }
            }
        }
        #endregion
    }
}
