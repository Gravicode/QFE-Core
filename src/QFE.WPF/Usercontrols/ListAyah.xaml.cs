using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ListAyah.xaml
    /// </summary>
    public partial class ListAyah : UserControl
    {
        public delegate void AyahSelectedEventHandler(BLL.quran_data.AyahData Ayah);
        public event AyahSelectedEventHandler AyahSelectEvent;
        private void CallAyahEvent(BLL.quran_data.AyahData Ayah)
        {
            // Event will be null if there are no subscribers
            if (AyahSelectEvent != null)
            {
                AyahSelectEvent(Ayah);
            }
        }

        private static ObservableCollection<BLL.quran_data.AyahData> Ayahs { set; get; }
        public int selBefore { set; get; }
        public int SurahNo { set; get; }

        public ListAyah(int SurahNo, int LangId = 11, int verseSize = 30)
        {
            InitializeComponent();

            if (SurahNo > 0)
            {
                LoadAyah(SurahNo, LangId, verseSize);
            }
        }

        public ListAyah()
        {
            InitializeComponent();

            //setup list event
            ListData.PreviewMouseLeftButtonUp += ListData_PreviewMouseLeftButtonUp;
        }

        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

        public void OnScrollUp(object sender, RoutedEventArgs e)
        {
            var scrollViwer = GetScrollViewer(ListData) as ScrollViewer;
            
            if (scrollViwer != null)
            {
                // Logical Scrolling by Item
                scrollViwer.LineUp();
                // Physical Scrolling by Offset
                //scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset + 30);
            }
        }

        public void OnScrollDown(object sender, RoutedEventArgs e)
        {
            var scrollViwer = GetScrollViewer(ListData) as ScrollViewer;

            if (scrollViwer != null)
            {
                // Logical Scrolling by Item
                scrollViwer.LineDown();
                // Physical Scrolling by Offset
                //scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset - 30);
            }
        }

        public void LoadAyah(int SurahNo, int LangId = 11, int verseSize = 30)
        {
            this.SurahNo = SurahNo;
            selBefore = -1;
            Ayahs = BLL.quran_data.getVerses2(SurahNo, LangId, verseSize);
            ListData.ItemsSource = Ayahs;
        }

        public void ChangeLanguage(int langId)
        {
            var trans = BLL.quran_data.getTranslationDictionary(langId, SurahNo);
            for (int i = 0; i < Ayahs.Count; i++)
            {
                var item = Ayahs[i];
                if (trans.ContainsKey(item.idx))
                    item.translation = trans[item.idx];
                else
                    item.translation = string.Empty;
            }
        }

        public void ChangeVerseSize(int SizeVerse)
        {
            for (int i = 0; i < Ayahs.Count; i++)
            {
                var item = Ayahs[i];
                item.VerseSize = SizeVerse;
            }
        }

        void ListData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selBefore > -1 && selBefore != (sender as ListView).SelectedIndex)
            {
                SetItemAyah(selBefore, Brushes.White, false, false);
            }
            if (selBefore != (sender as ListView).SelectedIndex)
            {
                SetItemAyah((sender as ListView).SelectedIndex, Brushes.Red, true, true);
            }
        }

        public void SetItemAyah(int index, SolidColorBrush warna, bool Choose, bool Play)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
                       {
                           if (Choose)
                           {
                               ListData.SelectedIndex = index;
                               selBefore = ListData.SelectedIndex;
                               var selitem = ListData.SelectedItem;
                               ListData.ScrollIntoView(ListData.SelectedItem);
                               ListData.UpdateLayout();
                           }

                           ListViewItem myListBoxItem = (ListViewItem)(ListData.ItemContainerGenerator.ContainerFromIndex(index));
                           if (myListBoxItem != null)
                           {
                               ContentPresenter myContentPresenter = QFE.WPF.Tools.VisualHelper.FindVisualChild<ContentPresenter>(myListBoxItem);
                               DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                               TextBlock target = (TextBlock)myDataTemplate.FindName("AyatLbl", myContentPresenter);
                               if (target != null)
                               {
                                   foreach (Inline item in target.Inlines)
                                   {
                                       if (item.Name == "Ayat")
                                       {
                                           item.Foreground = warna;
                                       }
                                   }

                               }
                               //ListData.ScrollIntoView(Ayahs[index]);
                               //ListData.UpdateLayout();
                           }

                           if (Choose && ListData.SelectedItem != null && Play)
                           {
                               CallAyahEvent((BLL.quran_data.AyahData)ListData.SelectedItem);
                           }

                       }));
        }
    }
}
