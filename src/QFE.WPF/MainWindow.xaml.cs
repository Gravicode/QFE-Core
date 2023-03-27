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
using System.Configuration;
using QFE.WPF.Usercontrols;
using QFE.WPF.Tools;
using System.Speech.Recognition;
using System.Threading;
using System.Globalization;
using System.Speech.Synthesis;
using Microsoft.AspNetCore.Components.Web;
using QFE.BLL;
//using SharpSenses;
//using SharpSenses.Gestures;
//using SharpSenses.Poses;

namespace QFE.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum ClickMode { Recite = 0, ReadTranslation = 1, JustMark = 2 }
        public enum PlayMode { PerAyah = 0, PerSurah = 1, Continuous = 2 }
        public struct QuranState
        {
            public int Ayah { set; get; }
            public int Surah { set; get; }
            public int Juz { set; get; }

            public int TotalAyah { set; get; }
            public int ReciterId { set; get; }
            public int LanguageId { set; get; }

            public bool isPlaying { set; get; }
            public ListSetting CtlSetting { set; get; }
            public ListAyah CtlAyah { set; get; }
            public ListLanguage CtlLanguage { set; get; }
            public ListReciter CtlReciter { set; get; }
            public ListJuz CtlJuz { set; get; }
            public ListSurah CtlSurah { set; get; }
            public ListBookmark CtlBookmark { set; get; }
            public Konfigurasi config { set; get; }
        }

        public static QuranState CurrentState;
        public static bool InternetState { set; get; }

        //Speech recognition & synthetizer
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        public static bool isRecognizing { set; get; }
        public enum BossCommand { Stop, Read, Exit, NextSurah, PrevSurah, NoCommand, NextVerse, PrevVerse, GoToChapter, GoToVerse, ZoomIn, ZoomOut, VolumeUp, VolumeDown,AddBookmark,LoadBookmark,OpenBookmark };
        public static Dictionary<string, BossCommand> Perintah { set; get; }
        static SpeechSynthesizer speechSynthesizer;
        //static System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //Realsense
        //static ICamera cam;

        //timer shutdown
        //static int FaceOffCount = 0;
        #region Init App
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //read config
            string PathDB = Logs.getPath() + "\\DB\\quran.db";
            System.IO.FileInfo ConFile = new System.IO.FileInfo(PathDB);
            if (!ConFile.Exists)
            {
                MessageBox.Show("Database not found.");
                Application.Current.Shutdown();

            }
            string ConStr = string.Format("Data Source={0};", ConFile.FullName);
            BLL.quran_data.Conn = ConStr;

            //string ConStr = ConfigurationManager.ConnectionStrings["quran_context"].ConnectionString;
            //UrlRecitation = ConfigurationManager.AppSettings["UrlRecitation"];

            //player events
            QuranPlayer.MediaFailed += media_MediaFailed;
            QuranPlayer.MediaEnded += QuranPlayer_MediaEnded;

            //button events
            StopBtn.Click += StopBtn_Click;
            PlayBtn.Click += PlayBtn_Click;
            NextBtn.Click += NextBtn_Click;
            PrevBtn.Click += PrevBtn_Click;
            BookmarkBtn.Click += BookmarkBtn_Click;
            //QFE.WPF.Tools.CacheManager<string> Cache = new Tools.CacheManager<string>();
            //Cache["hosni"] = "gendut";
            //MessageBox.Show(Cache["hosni"]);

            InitQuran();

            //Init speech recognizer
            manualResetEvent = new ManualResetEvent(false);

            ListenToBoss();

            /*
            //real sense start
            cam = Camera.Create(); //autodiscovers your sdk (perceptual or realsense)

            //cam.Face.Visible += Face_Visible;
            //cam.Face.NotVisible += Face_NotVisible;
            cam.Gestures.SwipeLeft += Gestures_SwipeLeft;
            cam.Gestures.SwipeRight += Gestures_SwipeRight;
            //cam.Gestures.SwipeUp += Gestures_SwipeUp;
            //cam.Gestures.SwipeDown += Gestures_SwipeDown;
            cam.RightHand.Moved += RightHand_Moved;
            cam.LeftHand.Moved += LeftHand_Moved;
            cam.Start();


            //timer for human detection
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, CurrentState.config.ShutdownTime, 0);
            dispatcherTimer.Start();
            */
        }

        
        #endregion

        /*
        #region Real Sense
        void LeftHand_Moved(Position obj)
        {
            if (!CurrentState.CtlSetting.isGestureEnable) return;
            if (obj.Image.Y < 200)
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlAyah.OnScrollDown(null, null);
                   }
                   ));

            }
            else
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlAyah.OnScrollUp(null, null);
                   }
                   ));

            }
        }
        void RightHand_Moved(Position obj)
        {
            if (!CurrentState.CtlSetting.isGestureEnable) return;
            if (obj.Image.Y < 200)
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlAyah.OnScrollDown(null, null);
                   }
                   ));

            }
            else
            {

                Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {
                       CurrentState.CtlAyah.OnScrollUp(null, null);
                   }
                   ));

            }
        }
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!cam.Face.IsVisible)
            {
                FaceOffCount++;
            }
            else
            {
                FaceOffCount = 0;
            }
            if (FaceOffCount > 1 && CurrentState.CtlSetting.isAutoShutdownEnable)
            {
                Dispatcher.BeginInvoke(
                     new ThreadStart(() =>
                     {
                         speechSynthesizer.Speak("I will shutdown this application");
                         Application.Current.Shutdown();
                     }
                     ));
            }
        }

        void Gestures_SwipeRight(Hand obj)
        {
            if (CurrentState.CtlSetting.isGestureEnable)
            {
                Dispatcher.BeginInvoke(
                    new ThreadStart(() =>
                    {
                        NextBtn_Click(null, null);
                    }
                    ));
            }
            //throw new NotImplementedException();
        }

        void Gestures_SwipeLeft(Hand obj)
        {
            if (CurrentState.CtlSetting.isGestureEnable)
            {
                Dispatcher.BeginInvoke(
                      new ThreadStart(() =>
                      {
                          PrevBtn_Click(null, null);
                      }
                      ));
            }
            //throw new NotImplementedException();
        }

        void Face_NotVisible()
        {

            //throw new NotImplementedException();
        }

        void Face_Visible()
        {
            //FaceOffCount = 0;
            //throw new NotImplementedException();
        }
        #endregion
        */

        #region Speech
        void ListenToBoss()
        {
            CultureInfo ci = new CultureInfo("en-US");
            _recognizer = new SpeechRecognitionEngine(ci);
            // Select a voice that matches a specific gender.  
            Perintah = new Dictionary<string, BossCommand>();

            Perintah.Add("please turn off", BossCommand.Exit);
            Perintah.Add("recite now", BossCommand.Read);
            Perintah.Add("stop recite", BossCommand.Stop);
            Perintah.Add("next chapter", BossCommand.NextSurah);
            Perintah.Add("previous chapter", BossCommand.PrevSurah);
            Perintah.Add("next verse", BossCommand.NextVerse);
            Perintah.Add("previous verse", BossCommand.PrevVerse);
            Perintah.Add("zoom in", BossCommand.ZoomIn);
            Perintah.Add("zoom out", BossCommand.ZoomOut);
            Perintah.Add("volume up", BossCommand.VolumeUp);
            Perintah.Add("volume down", BossCommand.VolumeDown);
            Perintah.Add("add bookmark", BossCommand.AddBookmark);
            Perintah.Add("open bookmark", BossCommand.OpenBookmark);

            foreach (KeyValuePair<string, BossCommand> entry in Perintah)
            {
                _recognizer.LoadGrammar(new Grammar(new GrammarBuilder(entry.Key)));
            }
            //special grammar
            _recognizer.LoadGrammar(specialGrammar());

            isRecognizing = false;
            _recognizer.SpeechRecognized += _recognizer_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizer_SpeechRecognitionRejected;
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
        }

        private static Grammar specialGrammar()
        {
            Choices navigations = new Choices(new string[] { "chapter", "verse","bookmark" });
            SemanticResultKey navigationKeys = new SemanticResultKey("navigation", navigations);

            Choices values = new Choices();

            for (int i = 1; i <= 286; i++)
                values.Add(i.ToString());
            SemanticResultKey valueKeys = new SemanticResultKey("values", values);

            // 2. navigation commands.
            GrammarBuilder navigationGrammarBuilder = new GrammarBuilder();
            navigationGrammarBuilder.Append(navigationKeys);
            navigationGrammarBuilder.Append(valueKeys);
            navigationGrammarBuilder.Culture = CultureInfo.GetCultureInfo("en-US");
            navigationGrammarBuilder.Append("go");

            // Create a Grammar object from the GrammarBuilder.
            return new Grammar(navigationGrammarBuilder);
        }

        void _recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (!CurrentState.CtlSetting.isVoiceEnable) return;
            if (e.Result.Alternates.Count == 0)
            {
                Console.WriteLine("Perintah tidak dikenal.");
                return;
            }
            Console.WriteLine("Perintah tidak dikenali, mungkin maksud tuan ini:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (!CurrentState.CtlSetting.isVoiceEnable) return;
            try
            {
                if (isRecognizing) return;
                if (e.Result.Confidence < 0.85) return;
                int NavigationNum = -1;

                isRecognizing = true;

                BossCommand selCmd = BossCommand.NoCommand;
                if (Perintah.ContainsKey(e.Result.Text))
                {
                    selCmd = Perintah[e.Result.Text];
                }
                if (e.Result.Semantics != null && e.Result.Semantics.Count != 0)
                {
                    if (e.Result.Semantics.ContainsKey("navigation"))
                    {
                        if (e.Result.Semantics["navigation"].Value.ToString().Contains("chapter"))
                            selCmd = BossCommand.GoToChapter;
                        else if (e.Result.Semantics["navigation"].Value.ToString().Contains("verse"))
                            selCmd = BossCommand.GoToVerse;
                        else selCmd = BossCommand.LoadBookmark;
                        // Checks whether a step has been specified.
                        if (e.Result.Semantics.ContainsKey("values"))
                        {
                            if (!int.TryParse(e.Result.Semantics["values"].Value.ToString(), out NavigationNum)) return;
                        }
                    }
                }
                if (selCmd == BossCommand.NoCommand) return;

                switch (selCmd)
                {
                    case BossCommand.Read:
                        //speechSynthesizer.SpeakAsync("Ok sir");
                        PlayBtn_Click(null, null);
                        break;
                    case BossCommand.Stop:
                        StopBtn_Click(null, null);
                        //speechSynthesizer.SpeakAsync("I have stopped the reciter");
                        break;
                    case BossCommand.Exit:
                        StopBtn_Click(null, null);
                        speechSynthesizer.Speak("Turning off the application");
                        Application.Current.Shutdown();
                        break;
                    case BossCommand.NextSurah:
                        NextBtn_Click(null, null);
                        break;
                    case BossCommand.PrevSurah:
                        PrevBtn_Click(null, null);
                        break;
                    case BossCommand.NextVerse:
                        if (CurrentState.isPlaying)
                            StopBtn_Click(null, null);
                        NextVerse(false);
                        break;
                    case BossCommand.PrevVerse:
                        if (CurrentState.isPlaying)
                            StopBtn_Click(null, null);
                        PreviousVerse(false);
                        break;
                    case BossCommand.GoToVerse:
                        if (NavigationNum > 0 && NavigationNum <= CurrentState.TotalAyah)
                        {
                            CurrentState.Ayah = NavigationNum;
                            StopBtn_Click(null, null);
                            if (CurrentState.CtlAyah.selBefore > -1)
                                CurrentState.CtlAyah.SetItemAyah(CurrentState.CtlAyah.selBefore, Brushes.White, false, false);
                            
                            CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, false);
                        }
                        break;
                    case BossCommand.GoToChapter:
                        if (NavigationNum >= CurrentState.CtlSurah.MinSurah && NavigationNum <= CurrentState.CtlSurah.MaxSurah)
                        {
                            if (CurrentState.isPlaying) StopBtn_Click(null, null);
                            LoadSpecificSurah(NavigationNum);
                        }
                        break;
                    case BossCommand.VolumeUp:
                        if (CurrentState.CtlSetting.Volume + 0.2 <= ListSetting.MaxVolume)
                        {
                            CurrentState.CtlSetting.setVolume(CurrentState.CtlSetting.Volume + 0.2);
                        }
                        else
                        {
                            CurrentState.CtlSetting.setVolume(ListSetting.MaxVolume);
                        }
                        break;
                    case BossCommand.VolumeDown:
                        if (CurrentState.CtlSetting.Volume - 0.2 >= ListSetting.MinVolume)
                        {
                            CurrentState.CtlSetting.setVolume(CurrentState.CtlSetting.Volume - 0.2);
                        }
                        else
                        {
                            CurrentState.CtlSetting.setVolume(ListSetting.MinVolume);
                        }
                        break;
                    case BossCommand.ZoomIn:
                        if (CurrentState.CtlSetting.VerseSize + 2 <= ListSetting.MaxVerseSize)
                        {
                            CurrentState.CtlSetting.setVerseSize(CurrentState.CtlSetting.VerseSize + 2);
                        }
                        else
                        {
                            CurrentState.CtlSetting.setVerseSize(ListSetting.MaxVerseSize);
                        }
                        break;
                    case BossCommand.ZoomOut:
                        if (CurrentState.CtlSetting.VerseSize - 2 >= ListSetting.MinVerseSize)
                        {
                            CurrentState.CtlSetting.setVerseSize(CurrentState.CtlSetting.VerseSize - 2);
                        }
                        else
                        {
                            CurrentState.CtlSetting.setVerseSize(ListSetting.MinVerseSize);
                        }
                        break;
                    case BossCommand.AddBookmark:
                        AddBookmark(false);
                        speechSynthesizer.SpeakAsync("bookmark added");
                        break;
                    case BossCommand.LoadBookmark:
                        var res = CurrentState.CtlBookmark.LoadBookmark(NavigationNum);
                        if (!res)
                        {
                            speechSynthesizer.SpeakAsync(string.Format("bookmark {0} not found",NavigationNum));
                        }
                        break;
                    case BossCommand.OpenBookmark:
                        if (!ExpanderBookmark.IsExpanded)
                        {
                            ExpanderBookmark.IsExpanded = true;
                        }
                        break;
                }
                //speechSynthesizer.Dispose();
            }
            catch
            {
            }
            finally
            {
                isRecognizing = false;
            }
        }
        #endregion

        #region Button Handler
        void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            int NewSurah = CurrentState.Surah - 1;
            if (NewSurah < CurrentState.CtlSurah.MinSurah) NewSurah = CurrentState.CtlSurah.MaxSurah;
            StopBtn_Click(null, null);
            LoadSpecificSurah(NewSurah);
        }

        void BookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            AddBookmark(true);
        }

        void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            int NewSurah = CurrentState.Surah + 1;
            if (NewSurah > CurrentState.CtlSurah.MaxSurah) NewSurah = CurrentState.CtlSurah.MinSurah;
            StopBtn_Click(null, null);
            LoadSpecificSurah(NewSurah);
        }

        void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentState.isPlaying)
            {
                if (CurrentState.CtlAyah.selBefore > -1)
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.CtlAyah.selBefore, Brushes.White, false, false);
                CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, true);
            }
        }

        void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            switch ((ClickMode)CurrentState.CtlSetting.ClickMode)
            {
                case ClickMode.Recite:
                    if (CurrentState.isPlaying)
                    {
                        QuranPlayer.Stop();
                        CurrentState.isPlaying = false;
                    }
                    break;
                case ClickMode.ReadTranslation:
                    //isSpeaking = false;
                    speechSynthesizer.SpeakAsyncCancelAll();
                    break;
            }
        }
        #endregion

        #region Forms Controls & Handler
        async void InitQuran()
        {

            //setup state
            CurrentState = new QuranState();

            //setup data from config
            CurrentState.config = new Konfigurasi();
            CurrentState.Ayah = CurrentState.config.AyahLastOpen;
            CurrentState.Surah = CurrentState.config.SurahLastOpen;
            CurrentState.LanguageId = CurrentState.config.LanguageLastOpen;
            CurrentState.ReciterId = CurrentState.config.ReciterLastOpen;
            CurrentState.Juz = CurrentState.config.JuzLastOpen;

            //speech synth
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
            speechSynthesizer.Volume = Convert.ToInt32(CurrentState.config.Volume * 100);
            speechSynthesizer.SpeakCompleted += speechSynthesizer_SpeakCompleted;

            //setup setting
            CurrentState.CtlSetting = new ListSetting();
            ExpanderSetting.Content = CurrentState.CtlSetting;
            //get configuration from config
            CurrentState.CtlSetting.setVolume(CurrentState.config.Volume);
            CurrentState.CtlSetting.setVerseSize(CurrentState.config.VerseSize);
            CurrentState.CtlSetting.setClickMode(CurrentState.config.ClickMode);
            CurrentState.CtlSetting.setPlayMode(CurrentState.config.PlayMode);
            CurrentState.CtlSetting.setVoice(CurrentState.config.isVoiceEnable);
            CurrentState.CtlSetting.setGesture(CurrentState.config.isGestureEnable);
            CurrentState.CtlSetting.setAutoShutdown(CurrentState.config.isAutoShutdownEnable);
            CurrentState.CtlSetting.VerseChangeEvent += CtlSetting_VerseChangeEvent;
            CurrentState.CtlSetting.VolumeChangeEvent += CtlSetting_VolumeChangeEvent;

            //setup expander
            var Jz = BLL.quran_data.getJuz(CurrentState.Juz);
            setExpanderTitle(ExpanderJuz, "Juzlbl", "Selected Juz: " + Jz.idx + ". " + Jz.name);

            var Rct = BLL.quran_data.getReciter(CurrentState.ReciterId);
            setExpanderTitle(ExpanderReciter, "Reciterlbl", Rct.name);

            var Lng = BLL.quran_data.getLanguage(CurrentState.LanguageId);
            setExpanderTitle(ExpanderLanguage, "Langlbl", Lng.lang);

            //player state
            CurrentState.isPlaying = false;
            QuranPlayer.LoadedBehavior = MediaState.Manual;
            QuranPlayer.UnloadedBehavior = MediaState.Stop;
            //QuranPlayer.Volume = CurrentState.config.Volume;
            QuranPlayer.Stop();
            Binding volBInding = new Binding("Volume");
            volBInding.Source = CurrentState.CtlSetting;
            QuranPlayer.SetBinding(MediaElement.VolumeProperty, volBInding);

            //setup language
            CurrentState.CtlLanguage = new ListLanguage();
            CurrentState.CtlLanguage.Height = 250;
            CurrentState.CtlLanguage.LanguageSelectEvent += CtlLanguage_LanguageSelectEvent;
            ExpanderLanguage.Content = CurrentState.CtlLanguage;

            //setup reciter
            CurrentState.CtlReciter = new ListReciter();
            CurrentState.CtlReciter.Height = 250;
            CurrentState.CtlReciter.ReciterSelectEvent += CtlReciter_ReciterSelectEvent;
            ExpanderReciter.Content = CurrentState.CtlReciter;

            //setup juz
            CurrentState.CtlJuz = new ListJuz();
            CurrentState.CtlJuz.Height = 250;
            CurrentState.CtlJuz.JuzSelectEvent += CtlJuz_JuzSelectEvent;
            ExpanderJuz.Content = CurrentState.CtlJuz;

            //setup surah
            CurrentState.CtlSurah = new ListSurah();
            CurrentState.CtlSurah.SurahSelectEvent += CtlSurah_SurahSelectEvent;
            CurrentState.CtlSurah.Height = 250;
            ExpanderSurah.Content = CurrentState.CtlSurah;
            CurrentState.CtlSurah.LoadSurah(CurrentState.Juz);

            //setup ayah
            CurrentState.CtlAyah = new ListAyah();
            CurrentState.CtlAyah.AyahSelectEvent += CtlAyah_AyahSelectEvent;

            AyahPanel.Children.Add(CurrentState.CtlAyah);

            //select last opened ayah
            LoadSpecificSurah(CurrentState.Surah, CurrentState.Ayah);
            CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, false);

            //load bookmark
            CurrentState.CtlBookmark = new ListBookmark();
            CurrentState.CtlBookmark.BookmarkSelectEvent += CtlBookmark_BookmarkSelectEvent;
            CurrentState.CtlBookmark.Height = 250;
            ExpanderBookmark.Content = CurrentState.CtlBookmark;

            //Internet check
            InternetState = await QFE.WPF.Tools.Internet.CheckConnection(CurrentState.config.UrlRecitation);
            if (!InternetState) MessageBox.Show("No Internet connection.", "Warning");

        }

        void AddBookmark(bool Prompt)
        {
            string Judul = DateTime.Now.ToString("dd-MMM-yy HH:mm");
            if (Prompt)
            {
                var selSurah = BLL.quran_data.getSurah(CurrentState.Surah);
                string NewJudul = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Please type bookmark title for {0} : {1}",selSurah.latin,CurrentState.Ayah), "Add Bookmark", Judul);
                if (string.IsNullOrEmpty(NewJudul)) return;
                Judul = NewJudul;
            }
            QFE.BLL.quran_data.bookmarkext item = new BLL.quran_data.bookmarkext(){
            juz = CurrentState.Juz,
            surah = CurrentState.Surah,
            ayah = CurrentState.Ayah,
            title = Judul};
            BLL.quran_data.InsertBookmark(item);
            CurrentState.CtlBookmark.LoadBookmark();
        }

        void speechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            /*
            if (e.Cancelled)
            {
                //isSpeaking = false;
            }
            else if(e.Prompt.IsCompleted)
            {
                NextVerse(true);
            }*/
        }
        void CtlBookmark_BookmarkSelectEvent(BLL.quran_data.bookmarkext Bookmark)
        {
            CurrentState.Juz = Bookmark.juz;
            CurrentState.CtlSurah.LoadSurah(CurrentState.Juz);
            //setup expander juz
            var Jz = BLL.quran_data.getJuz(CurrentState.Juz);
            setExpanderTitle(ExpanderJuz, "Juzlbl", "Selected Juz: " + Jz.idx + ". " + Jz.name);
            //load surah and ayah
            LoadSpecificSurah(Bookmark.surah, Bookmark.ayah);
            CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, false);

        }
        void CtlSetting_VolumeChangeEvent(double VolumeSize)
        {
            speechSynthesizer.Volume = Convert.ToInt32(VolumeSize * 100);
        }

        void CtlSetting_VerseChangeEvent(int VerseSize)
        {
            CurrentState.CtlAyah.ChangeVerseSize(VerseSize);
        }

        void CtlJuz_JuzSelectEvent(BLL.quran_data.JuzData Juz)
        {
            //juz click
            CurrentState.Juz = Juz.idx;
            setExpanderTitle(ExpanderJuz, "Juzlbl", "Selected Juz: " + Juz.idx + ". " + Juz.name);
            CurrentState.CtlSurah.LoadSurah(CurrentState.Juz);
            LoadSpecificSurah(CurrentState.CtlSurah.MinSurah, CurrentState.CtlSurah.MinAyah);
            CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, false);
        }

        void CtlReciter_ReciterSelectEvent(DAL.reciter Reciter)
        {
            //reciter click
            CurrentState.ReciterId = Reciter.idx;
            setExpanderTitle(ExpanderReciter, "Reciterlbl", Reciter.name);
        }

        void CtlLanguage_LanguageSelectEvent(DAL.language Language)
        {
            //language click
            CurrentState.LanguageId = Language.langid;
            setExpanderTitle(ExpanderLanguage, "Langlbl", Language.lang);
            CurrentState.CtlAyah.ChangeLanguage(CurrentState.LanguageId);
        }

        void setExpanderTitle(Expander exp, string LabelName, string Val)
        {
            StackPanel panel1 = (StackPanel)exp.Header;
            foreach (UIElement item in panel1.Children)
            {
                if (item is Label)
                {
                    Label lbl1 = (Label)item;
                    if (lbl1.Name == LabelName)
                    {
                        lbl1.Content = Val;
                    }
                }
            }
        }

        void NextVerse(bool PlayState)
        {
            switch ((PlayMode)CurrentState.CtlSetting.PlayMode)
            {
                case PlayMode.Continuous:
                    if (CurrentState.Ayah + 1 > CurrentState.TotalAyah || (CurrentState.Surah == CurrentState.CtlSurah.MaxSurah && CurrentState.Ayah + 1 > CurrentState.CtlSurah.MaxAyah))
                    {
                        if (CurrentState.Surah + 1 > CurrentState.CtlSurah.MaxSurah)
                        {
                            CurrentState.Surah = CurrentState.CtlSurah.MinSurah;
                            CurrentState.Ayah = CurrentState.CtlSurah.MinAyah;
                        }
                        else
                        {
                            CurrentState.Surah += 1;
                            CurrentState.Ayah = 1;
                        }
                        LoadSpecificSurah(CurrentState.Surah, CurrentState.Ayah);
                    }
                    else
                    {
                        CurrentState.Ayah += 1;
                    }

                    if (CurrentState.CtlAyah.selBefore > -1)
                        CurrentState.CtlAyah.SetItemAyah(CurrentState.CtlAyah.selBefore, Brushes.White, false, false);
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, PlayState);

                    break;
                case PlayMode.PerAyah:
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, PlayState);
                    break;
                case PlayMode.PerSurah:
                    if (CurrentState.Ayah + 1 > CurrentState.TotalAyah || (CurrentState.Surah == CurrentState.CtlSurah.MaxSurah && CurrentState.Ayah + 1 > CurrentState.CtlSurah.MaxAyah))
                    {
                        CurrentState.Ayah = CurrentState.CtlSurah.MinAyah;
                    }
                    else
                    {
                        CurrentState.Ayah += 1;
                    }
                    if (CurrentState.CtlAyah.selBefore > -1)
                        CurrentState.CtlAyah.SetItemAyah(CurrentState.CtlAyah.selBefore, Brushes.White, false, false);
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, PlayState);
                    break;
            }
        }

        void PreviousVerse(bool PlayState)
        {
            switch ((PlayMode)CurrentState.CtlSetting.PlayMode)
            {
                case PlayMode.Continuous:
                    if (CurrentState.Ayah - 1 < 1 || (CurrentState.Surah == CurrentState.CtlSurah.MinSurah && CurrentState.Ayah - 1 < CurrentState.CtlSurah.MinAyah))
                    {
                        if (CurrentState.Surah - 1 < CurrentState.CtlSurah.MinSurah)
                        {
                            CurrentState.Surah = CurrentState.CtlSurah.MaxSurah;
                            CurrentState.Ayah = CurrentState.CtlSurah.MinAyah;
                        }
                        else
                        {
                            CurrentState.Surah -= 1;
                            CurrentState.Ayah = 1;
                        }
                        LoadSpecificSurah(CurrentState.Surah, CurrentState.Ayah);
                    }
                    else
                    {
                        CurrentState.Ayah -= 1;
                    }

                    if (CurrentState.CtlAyah.selBefore > -1)
                        CurrentState.CtlAyah.SetItemAyah(CurrentState.CtlAyah.selBefore, Brushes.White, false, false);
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, PlayState);

                    break;
                case PlayMode.PerAyah:
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, PlayState);
                    break;
                case PlayMode.PerSurah:
                    if (CurrentState.Ayah - 1 < 1 || (CurrentState.Surah == CurrentState.CtlSurah.MinSurah && CurrentState.Ayah - 1 < CurrentState.CtlSurah.MinAyah))
                    {
                        CurrentState.Ayah = CurrentState.CtlSurah.MaxAyah;
                    }
                    else
                    {
                        CurrentState.Ayah -= 1;
                    }
                    if (CurrentState.CtlAyah.selBefore > -1)
                        CurrentState.CtlAyah.SetItemAyah(CurrentState.CtlAyah.selBefore, Brushes.White, false, false);
                    CurrentState.CtlAyah.SetItemAyah(CurrentState.Ayah - 1, Brushes.Red, true, PlayState);
                    break;
            }
        }

        void QuranPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            NextVerse(true);
        }

        void CtlSurah_SurahSelectEvent(DAL.surah menu)
        {
            LoadSpecificSurah(menu.idx);
        }

        void LoadSpecificSurah(int NoSurah, int NoAyah = 1)
        {
            
            var item = BLL.quran_data.getSurah(NoSurah);
            CurrentState.Surah = NoSurah;
            CurrentState.TotalAyah = item.totalayah;
            CurrentState.Ayah = NoAyah;
            CurrentState.CtlAyah.LoadAyah(NoSurah, CurrentState.LanguageId, CurrentState.CtlSetting.VerseSize);
            SurahIdxLbl.Content = "("+BLL.quran_data.AyahInArabic[item.idx] + ")";
            SurahLbl.Content = item.name + " - " + item.latin;
            setExpanderTitle(ExpanderSurah, "Surahlbl", "Now Reciting: " + item.latin);
        }

        void CtlAyah_AyahSelectEvent(BLL.quran_data.AyahData Ayah)
        {
            //play audio from internet
            CurrentState.Ayah = Ayah.idx;
            Recite();
        }

        void Recite()
        {
            switch ((ClickMode)CurrentState.CtlSetting.ClickMode)
            {
                case ClickMode.Recite:
                    var rec = BLL.quran_data.getReciter(CurrentState.ReciterId);
                    string _Prefix = rec.mediaurl;
                    string SurahKey = CurrentState.Surah.ToString().PadLeft(3, '0');
                    string AyahKey = CurrentState.Ayah.ToString().PadLeft(3, '0');
                    string MediaUrl = string.Format(_Prefix, SurahKey, AyahKey);
                    if (!CurrentState.isPlaying)
                    {
                        CurrentState.isPlaying = true;
                    }
                    else QuranPlayer.Stop();
                    string NamaFile = string.Format("{0}_{1}.mp3", SurahKey, AyahKey);
                    string SubFolder = string.Format("reciter_{0}", rec.idx);
                    string UrlMedia = null;
                    if (InternetState)
                    {
                        UrlMedia = MediaDownloader.DownloadAndPlay(MediaUrl, SubFolder, NamaFile);
                    }
                    else
                    {
                        UrlMedia = MediaDownloader.CheckOfflineMedia(MediaUrl, SubFolder, NamaFile);
                    }
                    if (!string.IsNullOrEmpty(UrlMedia))
                    {
                        QuranPlayer.Source = new Uri(UrlMedia, UriKind.RelativeOrAbsolute);
                        QuranPlayer.Play();
                    }
                    break;
                case ClickMode.ReadTranslation:
                    //only english
                    if (CurrentState.LanguageId == 11)
                    {
                        BLL.quran_data.AyahData item = (BLL.quran_data.AyahData)CurrentState.CtlAyah.ListData.SelectedItem;
                        speechSynthesizer.SpeakAsync(item.translation);
                    }
                    break;
            }
        }

        void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CurrentState.config.AyahLastOpen = CurrentState.Ayah;
            CurrentState.config.SurahLastOpen = CurrentState.Surah;
            CurrentState.config.JuzLastOpen = CurrentState.Juz;
            CurrentState.config.ReciterLastOpen = CurrentState.ReciterId;
            CurrentState.config.LanguageLastOpen = CurrentState.LanguageId;
            CurrentState.config.Volume = CurrentState.CtlSetting.Volume;

            CurrentState.config.VerseSize = CurrentState.CtlSetting.VerseSize;
            CurrentState.config.ClickMode = CurrentState.CtlSetting.ClickMode;
            CurrentState.config.PlayMode = CurrentState.CtlSetting.PlayMode;
            CurrentState.config.isVoiceEnable = CurrentState.CtlSetting.isVoiceEnable;
            CurrentState.config.isGestureEnable = CurrentState.CtlSetting.isGestureEnable;
            CurrentState.config.isAutoShutdownEnable = CurrentState.CtlSetting.isAutoShutdownEnable;

            CurrentState.config.WriteSettings();
            //dispatcherTimer.Stop();

            //Dispose speech engine
            manualResetEvent.Set();

            if (_recognizer != null)
            {
                _recognizer.Dispose();
            }

            if (speechSynthesizer != null)
                speechSynthesizer.Dispose();
            /*
            try
            {
                if (cam != null)
                    cam.Dispose();
            }
            catch
            {
            }
            */

        }
        #endregion
    }

}
