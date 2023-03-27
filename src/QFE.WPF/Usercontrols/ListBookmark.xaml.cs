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
    /// Interaction logic for ListBookmark.xaml
    /// </summary>
    public partial class ListBookmark : UserControl
    {
        IList<BLL.quran_data.bookmarkext> data = null;
        public delegate void BookmarkSelectedEventHandler(BLL.quran_data.bookmarkext Bookmark);
        public event BookmarkSelectedEventHandler BookmarkSelectEvent;
        private void CallBookmarkEvent(BLL.quran_data.bookmarkext Bookmark)
        {
            // Event will be null if there are no subscribers
            if (BookmarkSelectEvent != null)
            {
                BookmarkSelectEvent(Bookmark);
            }
        }

        public ListBookmark()
        {
            InitializeComponent();
            LoadBookmark();
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
        }

        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                BookmarkSelectEvent((BLL.quran_data.bookmarkext)item);
            }
        }

        public void LoadBookmark()
        {
            data = BLL.quran_data.getBookmark();

            ListData.ItemsSource = data;
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure to delete this bookmark", "Delete Bookmark Confirmation",
                MessageBoxButton.YesNo);
            if (dialogResult != MessageBoxResult.Yes)
            {
                return;
            }
            BLL.quran_data.bookmarkext seldata = (BLL.quran_data.bookmarkext)((Button)sender).DataContext;

            BLL.quran_data.DeleteBookmark(seldata);
            LoadBookmark();
        }

        public bool LoadBookmark(int Counter)
        {
            var item = from c in data
                       where c.Counter == Counter
                       select c;
            if (item != null && item.Count() > 0)
            {
                BookmarkSelectEvent(item.SingleOrDefault());
                return true;
            }
            return false;
        }
    }
}
