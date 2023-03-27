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
    /// Interaction logic for ListReciter.xaml
    /// </summary>
    public partial class ListReciter : UserControl
    {
        public delegate void ReciterSelectedEventHandler(DAL.reciter Reciter);
        public event ReciterSelectedEventHandler ReciterSelectEvent;
        private void CallReciterEvent(DAL.reciter Reciter)
        {
            // Event will be null if there are no subscribers
            if (ReciterSelectEvent != null)
            {
                ReciterSelectEvent(Reciter);
            }
        }
        public ListReciter()
        {
            InitializeComponent();
            PopulateReciter();
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
        }
        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selitem = (sender as ListView).SelectedItem;
            if (selitem != null)
            {
                CallReciterEvent((DAL.reciter)selitem);
            }
        }
        void PopulateReciter()
        {
            var data = BLL.quran_data.getReciters();
            ListData.ItemsSource = data;
        }
    }
}
