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
    /// Interaction logic for ListLanguage.xaml
    /// </summary>
    public partial class ListLanguage : UserControl
    {
        public delegate void LanguageSelectedEventHandler(DAL.language Language);
        public event LanguageSelectedEventHandler LanguageSelectEvent;
        private void CallLanguageEvent(DAL.language Language)
        {
            // Event will be null if there are no subscribers
            if (LanguageSelectEvent != null)
            {
                LanguageSelectEvent(Language);
            }
        }
        public ListLanguage()
        {
            InitializeComponent();
            PopulateLanguage();
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
        }
        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selitem = (sender as ListView).SelectedItem;
            if (selitem != null)
            {
                CallLanguageEvent((DAL.language)selitem);
            }
        }
        void PopulateLanguage()
        {
            var data = BLL.quran_data.getLanguage();
            ListData.ItemsSource = data;
        }
    }
}
