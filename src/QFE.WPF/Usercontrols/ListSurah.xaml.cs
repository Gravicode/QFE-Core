using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QFE.WPF.Usercontrols
{
    /// <summary>
    /// Interaction logic for ListSurah.xaml
    /// </summary>
    public partial class ListSurah : UserControl
    {
        public delegate void SurahSelectedEventHandler(DAL.surah Surah);
        public event SurahSelectedEventHandler SurahSelectEvent;
        private void CallSurahEvent(DAL.surah Surah)
        {
            // Event will be null if there are no subscribers
            if (SurahSelectEvent != null)
            {
                SurahSelectEvent(Surah);
            }
        }

        public int MinSurah { set; get; }
        public int MaxSurah { set; get; }
        public int MinAyah { set; get; }
        public int MaxAyah { set; get; }

        public ListSurah(int Juz=0)
        {
            InitializeComponent();
            LoadSurah(Juz);
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
        }

        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                SurahSelectEvent((DAL.surah)item);
            }
        }

        public void LoadSurah(int Juz=0)
        {
            IList<DAL.surah> data =null;
            if (Juz <= 0)
            {
                data = BLL.quran_data.getSurahNames();
                MinSurah = 1;
                MaxSurah = 114;
                MinAyah = 1;
                MaxAyah = 6;
            }
            else
            {
                data = BLL.quran_data.getSurahFromJuz(Juz);
                var item = BLL.quran_data.getJuz(Juz);
                MinSurah = item.surahfrom;
                MaxSurah = item.surahto;
                MinAyah = item.ayahfrom;
                MaxAyah = item.ayahto;
            }
           ListData.ItemsSource = data;
        }
    }
}
