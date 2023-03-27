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
    /// Interaction logic for ListJuz.xaml
    /// </summary>
    public partial class ListJuz : UserControl
    {
        public delegate void JuzSelectedEventHandler(BLL.quran_data.JuzData Juz);
        public event JuzSelectedEventHandler JuzSelectEvent;
        private void CallJuzEvent(BLL.quran_data.JuzData Juz)
        {
            // Event will be null if there are no subscribers
            if (JuzSelectEvent != null)
            {
                JuzSelectEvent(Juz);
            }
        }
        public ListJuz()
        {
            InitializeComponent();
            PopulateJuz();
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
        }
        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selitem = (sender as ListView).SelectedItem;
            if (selitem != null)
            {
                CallJuzEvent((BLL.quran_data.JuzData)selitem);
            }
        }
        void PopulateJuz()
        {
            var data = BLL.quran_data.getJuzNames();
            ListData.ItemsSource = data;
        }
    }
}
