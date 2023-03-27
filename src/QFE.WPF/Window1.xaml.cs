using QFE.WPF.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace QFE.WPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            BtnTranslate.Click += BtnTranslate_Click;
            //testspeed();
            //BtnTest.Click += BtnTest_Click;
            //test5();
            /*
            StackPanel panel1 = (StackPanel)expander1.Header;
            foreach (UIElement item in panel1.Children)
            {
                if (item is Label)
                {
                    Label lbl1 = (Label)item;
                    if (lbl1.Name == "lang1")
                    {
                        lbl1.Content = "Bahasa";
                    }
                }
            }*/
        }

        void BtnTranslate_Click(object sender, RoutedEventArgs e)
        {
            test6();
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            HasilTxt.Content = "";
            testspeed();
        }

        #region sampah
        public void test()
        {
            string Con = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            QFE.BLL.quran_data.Conn = Con;

            string filename = @"C:\DEV\Experiment\Quran\quran-simple-enhanced.xml";
            //Load xml
            XDocument xdoc = XDocument.Load(filename);

            //Run query
            var surah = (from lv1 in xdoc.Descendants("sura")
                         select new
                         {
                             Index = lv1.Attribute("index").Value,
                             Name = lv1.Attribute("name").Value
                         }).ToList();
            foreach (var item in surah)
            {
                BLL.quran_data.InsertSurah(Convert.ToInt32(item.Index), item.Name);
            }
        }
        public void test2()
        {
            List<DAL.surah> surahlist = new List<DAL.surah>();
            string Con = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            QFE.BLL.quran_data.Conn = Con;
            System.Data.DataTable dt = QFE.WPF.Tools.CSVReader.OpenCSV(@"C:\sampah\surah.csv");
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                DAL.surah item = new DAL.surah();
                item.idx = Convert.ToInt32(dr["No"]);
                item.totalayah = Convert.ToInt32(dr["ayah"]);
                item.latin = dr["surah"].ToString();
                item.place = dr["place"].ToString();
                surahlist.Add(item);
            }
            BLL.quran_data.UpdateSurah(surahlist);
        }
        public void test3()
        {
            int langid = 11;

            string Con = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            QFE.BLL.quran_data.Conn = Con;
            var dt = QFE.WPF.Tools.FileWriter.ReadFile(@"C:\DEV\Experiment\Quran\QuranTransliterationEnglish.txt");
            foreach (var linestr in System.Text.RegularExpressions.Regex.Split(dt, System.Environment.NewLine))
            {
                var item = linestr.Split('|');
                if (item.Length >= 3)
                {
                    int surahidx = Convert.ToInt32(item[0]);
                    int ayahidx = Convert.ToInt32(item[1]);
                    string content = item[2].Replace("'", "''");
                    BLL.quran_data.InsertTransliteration(surahidx, ayahidx, langid, content);
                }
            }

        }
        public void test4()
        {
            string Con = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            QFE.BLL.quran_data.Conn = Con;

            //Run query
            var data = BLL.quran_data.getQuran();
            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(item.ayah_location))
                    item.ayah_id = Convert.ToInt32(item.ayah_location.Substring(4, 3));
                else
                    item.ayah_id = 0;
            }
            BLL.quran_data.UpdateQuranAyah(data);
        }
        #endregion

        public void testspeed()
        {
            string PathDB = Logs.getPath() + "\\DB\\quran.db";
            System.IO.FileInfo ConFile = new System.IO.FileInfo(PathDB);
            if (!ConFile.Exists)
            {
                MessageBox.Show("Database not found.");
                Application.Current.Shutdown();

            }
            string ConStr = string.Format("Data Source={0};Version=3;", ConFile.FullName);
            BLL.quran_data.Conn = ConStr;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            BLL.quran_data.ReadQuranWithLinq();
            //var data1 = BLL.quran_data.getVerses1(2,11,30);
            sw.Stop();
            string hasil = "verses1 :" + sw.ElapsedMilliseconds;
            sw.Reset();
            sw.Start();
            BLL.quran_data.ReadQuranWithFungsiDB();

            //var data2 = BLL.quran_data.getVerses2(2, 11, 30);
            sw.Stop();
            hasil += ", verses2 :" + sw.ElapsedMilliseconds;
            HasilTxt.Content = hasil;
        }
        public void test5()
        {
            List<DAL.reciter> reclist = new List<DAL.reciter>();
            string Con = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            QFE.BLL.quran_data.Conn = Con;
            System.Data.DataTable dt = QFE.WPF.Tools.CSVReader.OpenCSV(@"C:\sampah\reciter.csv");
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                DAL.reciter item = new DAL.reciter();
                item.idx = Convert.ToInt32(dr["No"]);
                item.name = dr["Nama"].ToString();
                item.mediaurl = dr["Url"].ToString();
                reclist.Add(item);
            }
            BLL.quran_data.InsertReciter(reclist);
        }

        public void test6()
        {
            string LangName = TxtLang.Text;
            string Con = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            QFE.BLL.quran_data.Conn = Con;
            FileInfo info = new FileInfo(TxtPath.Text);
            if (!info.Exists) return;
            string[] trans = System.IO.File.ReadAllLines(info.FullName);
            int CurSurah = 0;
            foreach (var item in trans)
            {
                string[] parsedVerse = item.Split('|');

                if (parsedVerse != null && parsedVerse.Length >= 3)
                {
                    if (CurSurah != Convert.ToInt32(parsedVerse[0]))
                    {
                        CurSurah = Convert.ToInt32(parsedVerse[0]);
                        ProgressBar1.Value = CurSurah;
                    }
                    BLL.quran_data.updateVerse(CurSurah, Convert.ToInt32(parsedVerse[1]), LangName, parsedVerse[2]);
                }

            }
        }
    }
}
