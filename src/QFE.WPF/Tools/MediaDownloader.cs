using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QFE.WPF.Tools
{
    public class MediaDownloader
    {
        public MediaDownloader()
        {

        }

        public static void DownloadOnly(string UrlFile, string subFolder, string NamaFile)
        {
            // Create a new WebClient instance.
            using (WebClient myWebClient = new WebClient())
            {
                string AudioFile = QFE.WPF.Tools.Logs.getPath() + "\\audio\\" + subFolder;
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(AudioFile);
                if (!dir.Exists) dir.Create();
                // Download the Web resource and save it into the current filesystem folder.
                myWebClient.DownloadFileAsync(new Uri(UrlFile, UriKind.RelativeOrAbsolute), AudioFile + "\\" + NamaFile);
            }
        }

        public static string DownloadAndPlay(string UrlFile, string subFolder, string NamaFile)
        {
            string TargetFileName = string.Empty;
            try
            {
                // Create a new WebClient instance.
                using (WebClient myWebClient = new WebClient())
                {
                    string AudioFile = QFE.WPF.Tools.Logs.getPath() + "\\audio\\" + subFolder;
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(AudioFile);
                    if (!dir.Exists) dir.Create();
                    // Download the Web resource and save it into the current filesystem folder.
                    TargetFileName = AudioFile + "\\" + NamaFile;
                    if (System.IO.File.Exists(TargetFileName)) return TargetFileName;
                    myWebClient.DownloadFile(new Uri(UrlFile, UriKind.RelativeOrAbsolute), TargetFileName);

                }
                return TargetFileName;
            }
            catch
            {
                return null;
            }
        }

        public static string CheckOfflineMedia(string UrlFile, string subFolder, string NamaFile)
        {
            string TargetFileName = string.Empty;
            try
            {

                string AudioFile = QFE.WPF.Tools.Logs.getPath() + "\\audio\\" + subFolder;
                TargetFileName = AudioFile + "\\" + NamaFile;
                if (System.IO.File.Exists(TargetFileName)) return TargetFileName;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
