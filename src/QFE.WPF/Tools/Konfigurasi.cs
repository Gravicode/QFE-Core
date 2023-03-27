using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.WPF.Tools
{
    public class Konfigurasi
    {
        public int ShutdownTime { set; get; }
        public int VerseSize { set; get; }
        public int ClickMode { set; get; }
        public int PlayMode { set; get; }
        public bool isVoiceEnable { set; get; }
        public bool isGestureEnable { set; get; }
        public bool isAutoShutdownEnable { set; get; }
        public double Volume { set; get; }
        public int AyahLastOpen { set; get; }
        public int SurahLastOpen { set; get; }
        public int ReciterLastOpen { set; get; }
        public int LanguageLastOpen { set; get; }
        public int JuzLastOpen { set; get; }
        public string UrlRecitation { set; get; }

        public string TargetFile { set; get; }
        const string FileConfig = "quran.ini";

        public Konfigurasi()
        {
            TargetFile = Logs.getPath() + "\\" + FileConfig;
            if (!File.Exists(TargetFile))
            {
                DefaultSetting();
                WriteSettings();
            }
            else
            {
                ReadSettings();
            }
        }

        ~Konfigurasi()
        {
            WriteSettings();
        }

        private void ReadSettings()
        {
            try
            {
                IniFile ini = new IniFile(TargetFile);
                Volume = double.Parse(ini.IniReadValue("config", "Volume"));
                AyahLastOpen = int.Parse(ini.IniReadValue("config", "AyahLastOpen"));
                SurahLastOpen = int.Parse(ini.IniReadValue("config", "SurahLastOpen"));
                ReciterLastOpen = int.Parse(ini.IniReadValue("config", "ReciterLastOpen"));
                LanguageLastOpen = int.Parse(ini.IniReadValue("config", "LanguageLastOpen"));
                JuzLastOpen = int.Parse(ini.IniReadValue("config", "JuzLastOpen"));
                UrlRecitation = ini.IniReadValue("config", "UrlRecitation");

                VerseSize = int.Parse(ini.IniReadValue("config", "VerseSize"));
                ClickMode = int.Parse(ini.IniReadValue("config", "ClickMode"));
                PlayMode = int.Parse(ini.IniReadValue("config", "PlayMode"));
                isVoiceEnable = bool.Parse(ini.IniReadValue("config", "isVoiceEnable"));
                isGestureEnable = bool.Parse(ini.IniReadValue("config", "isGestureEnable"));
                isAutoShutdownEnable = bool.Parse(ini.IniReadValue("config", "isAutoShutdownEnable"));
                 ShutdownTime = int.Parse(ini.IniReadValue("config", "ShutdownTime"));
                
               
            }
            catch
            {
                throw;
            }
        }

        public void WriteSettings()
        {
            try
            {
                IniFile ini = new IniFile(TargetFile);
                ini.IniWriteValue("config", "Volume", Volume.ToString());
                ini.IniWriteValue("config", "AyahLastOpen", AyahLastOpen.ToString());
                ini.IniWriteValue("config", "SurahLastOpen", SurahLastOpen.ToString());
                ini.IniWriteValue("config", "ReciterLastOpen", ReciterLastOpen.ToString());
                ini.IniWriteValue("config", "LanguageLastOpen", LanguageLastOpen.ToString());
                ini.IniWriteValue("config", "JuzLastOpen", JuzLastOpen.ToString());
                ini.IniWriteValue("config", "UrlRecitation", UrlRecitation);

                ini.IniWriteValue("config", "VerseSize", VerseSize.ToString());
                ini.IniWriteValue("config", "ClickMode", ClickMode.ToString());
                ini.IniWriteValue("config", "PlayMode", PlayMode.ToString());
                ini.IniWriteValue("config", "isVoiceEnable", isVoiceEnable.ToString());
                ini.IniWriteValue("config", "isGestureEnable", isGestureEnable.ToString());
                ini.IniWriteValue("config", "isAutoShutdownEnable", isAutoShutdownEnable.ToString());
                ini.IniWriteValue("config", "ShutdownTime", ShutdownTime.ToString());

            }
            catch
            {
                throw;
            }
        }

        private void DefaultSetting()
        {
            Volume = 0.5;
            AyahLastOpen = 1;
            SurahLastOpen = 1;
            ReciterLastOpen = 1;
            LanguageLastOpen = 11;
            JuzLastOpen = 1;
            UrlRecitation = "http://www.everyayah.com/";

            ShutdownTime = 30;
            VerseSize = 30;
            ClickMode = 0;
            PlayMode = 2;
            isVoiceEnable = true;
            isGestureEnable = false;
            isAutoShutdownEnable = false;
        }
    }
}
