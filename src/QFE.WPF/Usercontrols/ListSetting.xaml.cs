using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ListSetting.xaml
    /// </summary>
    public partial class ListSetting : UserControl, INotifyPropertyChanged
    {
        #region constants
        public const double MaxVolume = 1.0;
        public const double MinVolume = 0.0;
        public const int MaxVerseSize = 50;
        public const int MinVerseSize = 20;
        #endregion

        #region Handler
        public delegate void VerseChangedEventHandler(int VerseSize);
        public event VerseChangedEventHandler VerseChangeEvent;
        private void CallVerseEvent(int VerseSize)
        {
            // Event will be null if there are no subscribers
            if (VerseChangeEvent != null)
            {
                VerseChangeEvent(VerseSize);
            }
        }

        public delegate void VolumeChangedEventHandler(double VolumeSize);
        public event VolumeChangedEventHandler VolumeChangeEvent;
        private void CallVolumeEvent(double VolumeSize)
        {
            // Event will be null if there are no subscribers
            if (VolumeChangeEvent != null)
            {
                VolumeChangeEvent(VolumeSize);
            }
        }
        #endregion

        #region properties
        private double volume;
        public double Volume
        {
            private set
            {
                volume = value; 
                OnPropertyChanged("Volume");
            }
            get { return volume; }
        }
      
        public int VerseSize { private set; get; }
        public int ClickMode { private set; get; }
        public int PlayMode { private set; get; }
        public bool isVoiceEnable { private set; get; }
        public bool isGestureEnable { private set; get; }
        public bool isAutoShutdownEnable { private set; get; }
        //const double Variance = 20;
        #endregion

        public ListSetting()
        {
            InitializeComponent();

            //Events
            VolumeSlide.ValueChanged += VolumeSlide_ValueChanged;
            VerseSlider.ValueChanged += VerseSlider_ValueChanged;
            ClickCmb.SelectionChanged += ClickCmb_SelectionChanged;
            PlayCmb.SelectionChanged += PlayCmb_SelectionChanged;
            VoiceChk.Checked += VoiceChk_Checked;
            VoiceChk.Unchecked += VoiceChk_Unchecked;
            GestureChk.Checked += GestureChk_Checked;
            GestureChk.Unchecked += GestureChk_Unchecked;
            AutoShutdownChk.Checked += AutoShutdownChk_Checked;
            AutoShutdownChk.Unchecked += AutoShutdownChk_Unchecked;
        }
        #region Events Method
        void AutoShutdownChk_Unchecked(object sender, RoutedEventArgs e)
        {
            isAutoShutdownEnable = AutoShutdownChk.IsChecked.Value;
        }

        void AutoShutdownChk_Checked(object sender, RoutedEventArgs e)
        {
            isAutoShutdownEnable = AutoShutdownChk.IsChecked.Value;
        }

        void GestureChk_Unchecked(object sender, RoutedEventArgs e)
        {
            isGestureEnable = GestureChk.IsChecked.Value;
        }

        void GestureChk_Checked(object sender, RoutedEventArgs e)
        {
            isGestureEnable = GestureChk.IsChecked.Value;
        }

        void VoiceChk_Unchecked(object sender, RoutedEventArgs e)
        {
            isVoiceEnable = VoiceChk.IsChecked.Value;
        }

        void VoiceChk_Checked(object sender, RoutedEventArgs e)
        {
            isVoiceEnable = VoiceChk.IsChecked.Value;
        }

        void PlayCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayMode = PlayCmb.SelectedIndex;
        }

        void ClickCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClickMode = ClickCmb.SelectedIndex;
        }

        void VerseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerseSize = (int)e.NewValue;
            CallVerseEvent(VerseSize);
        }

        void VolumeSlide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Volume = e.NewValue;
            CallVolumeEvent(e.NewValue);
        }
        #endregion

        #region Setter Props
        public void setVolume(double Volume)
        {
            VolumeSlide.Value = Volume;
            this.Volume = Volume;
        }
        public void setVerseSize(int FontSize)
        {
            VerseSlider.Value = FontSize;
            this.VerseSize = FontSize;
        }
        public void setClickMode(int ClickMode)
        {
            ClickCmb.SelectedIndex = ClickMode;
            this.ClickMode = ClickMode;
        }
        public void setPlayMode(int PlayMode)
        {
            PlayCmb.SelectedIndex = PlayMode;
            this.PlayMode = PlayMode;
        }
        public void setGesture(bool isGesture)
        {
            GestureChk.IsChecked = isGesture;
            this.isGestureEnable = isGesture;
        }
        public void setVoice(bool isVoice)
        {
            VoiceChk.IsChecked = isVoice;
            this.isVoiceEnable = isVoice;
        }
        public void setAutoShutdown(bool isAutoShutdown)
        {
            AutoShutdownChk.IsChecked = isAutoShutdown;
            this.isAutoShutdownEnable = isAutoShutdown;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
