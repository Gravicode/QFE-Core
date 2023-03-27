using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QFE.DAL;
using System.Data.SQLite;
//using System.Data.Linq.Mapping;
//using System.Data.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QFE.BLL
{
    public class quran_data
    {
        public static string Conn { set; get; }

        #region Verse
        private static Dictionary<int, string> _AyahInArabic;
        public static Dictionary<int, string> AyahInArabic
        {
            get
            {

                if (_AyahInArabic == null)
                {
                    _AyahInArabic = new Dictionary<int, string>();
                    try
                    {
                        var connection = new SQLiteConnection(Conn);
                        using (var context = new DataContext())
                        {

                            var data = (from a in context.ayahs
                                        orderby a.idx ascending
                                        select a).ToList();
                            _AyahInArabic.Add(0, ".");
                            foreach (var item in data)
                            {
                                _AyahInArabic.Add(item.idx, item.arabic);
                            }
                        }
                        connection.Close();
                    }
                    catch
                    {
                        throw;
                    }
                    return _AyahInArabic;
                }
                else
                {
                    return _AyahInArabic;
                }
            }
        }
        /*
        public static AyahData getVerse(int Surah, int Ayah)
        {
            AyahData SelAyah = default(AyahData);

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    SelAyah = (from a in context.qurans
                               where a.surah_id == Surah && a.ayah_id == Ayah
                               select new AyahData() { idx = a.ayah_id, ayah = AyahInArabic[a.ayah_id], content = a.Arabic }).SingleOrDefault();

                }

            }
            catch
            {
                throw;
            }
            return SelAyah;
        }
        public static List<AyahData> getVerses(int Surah)
        {
            List<AyahData> SelAyah = new List<AyahData>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    SelAyah = (from a in context.qurans
                               where a.surah_id == Surah
                               select new AyahData() { idx = a.ayah_id, ayah = AyahInArabic[a.ayah_id], content = a.Arabic }).ToList();

                }

            }
            catch
            {
                throw;
            }
            return SelAyah;
        }*/
        
        public static AyahData getVerse(int Surah, int Ayah)
        {
            AyahData SelAyah = default(AyahData);

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    SelAyah = (from a in context.quran_texts
                               where a.sura == Surah && a.aya == Ayah
                               select new AyahData() { idx = a.aya, ayah = AyahInArabic[a.aya], content = a.text }).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return SelAyah;
        }
        //second fastest
        public static ObservableCollection<AyahData> getVerses1(int Surah, int LangId = 11,int verseSize=30)
        {
            ObservableCollection<AyahData> SelAyah = new ObservableCollection<AyahData>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    //baru ada english untuk transliteration
                    var translit = getTransliteration(11, Surah);
                    //sudah ada 39 bahasa
                    var translang = getTranslation(LangId, Surah);
                    //get quran
                    var quranteks = (from a in context.quran_texts
                               where a.sura == Surah
                               select new AyahData() { idx = a.aya, ayah = AyahInArabic[a.aya], content = a.text }).ToList();
                    SelAyah = (from a in quranteks
                              join b in translit on a.idx equals b.ayahidx
                              join c in translang.DefaultIfEmpty() on a.idx equals c.ayah_id
                              into temp
                              from d in temp.DefaultIfEmpty(new TranslationData(){ ayah_id=a.idx, surah_id= Surah, content=default(string) })
                              select new AyahData() { idx=a.idx, ayah=a.ayah, content=a.content, translation = d.content, transliteration = b.content, VerseSize=verseSize  }).ToObservableCollection();
                               
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return SelAyah;
        }
    
        //fastest of all
        public static ObservableCollection<AyahData> getVerses2(int Surah, int LangId = 11, int verseSize = 30)
        {
            ObservableCollection<AyahData> SelAyah = new ObservableCollection<AyahData>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    //baru ada english untuk transliteration
                    var translit = getTransliterationDictionary(11, Surah);
                    //sudah ada 39 bahasa
                    var translang = getTranslationDictionary(LangId, Surah);
                    //get quran
                    var quranteks = (from a in context.quran_texts
                                     where a.sura == Surah
                                     select new AyahData() { idx = a.aya, ayah = AyahInArabic[a.aya], content = a.text, VerseSize=verseSize }).ToArray();
                    for (int i = 0; i < quranteks.Length; i++)
                    {
                        var item = quranteks[i];
                        if(translang.ContainsKey(item.idx))
                            item.translation = translang[item.idx];
                        if (translit.ContainsKey(item.idx))
                            item.transliteration = translit[item.idx];
                        SelAyah.Add(item);
                    }

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return SelAyah;
        }

        //Verses 3 : lambat baget, jangan pake
        public static ObservableCollection<AyahData> getVerses3(int Surah, int LangId = 11, int verseSize = 30)
        {
            ObservableCollection<AyahData> SelAyah = new ObservableCollection<AyahData>();

            try
            {
                var lng = getLanguage(LangId);

                FungsiDB.KoneksiStr = Conn;
                string Qry = string.Format(@"select a.sura,a.aya,a.text,b.content as translit, c.{0} as translate from quran_text a left join transliteration b on a.sura=b.surahidx and a.aya=b.ayahidx 
left join quran c on a.aya=c.ayah_id and a.sura=c.surah_id
where a.sura={1} and b.langid={2}", lng.lang, Surah, LangId);
                System.Data.DataTable dt = FungsiDB.RetrieveData(Qry);
                //dt.TableName = "data";
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    SelAyah.Add(new AyahData() { idx = Convert.ToInt32(dr["aya"]), ayah = AyahInArabic[Convert.ToInt32(dr["aya"])], translation = dr["translate"].ToString(), transliteration = dr["translit"].ToString(), content = dr["text"].ToString(), VerseSize = verseSize });
                }
            }
            catch
            {
                throw;
            }
            return SelAyah;
        }

        public static void updateVerse(int Surah,int Ayah,string LangName, string Content){
       
            try
            {
                FungsiDB.KoneksiStr = Conn;
                //update
                Content = Content.Replace("'", "''");
                string Qry = string.Format("update quran set {0}='{1}' where surah_id = {2} and ayah_id= {3}", LangName, Content, Surah, Ayah);
                FungsiDB.ExecuteNonQuery(Qry);
                /*
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Content = Content.Replace("'","''");

                    var sel = (from a in context.qurans
                           where a.surah_id == Surah && a.ayah_id == Ayah
                           select a).ToList();
                    if (sel.Count > 0)
                    {
                        //update
                        string Qry = string.Format("update quran set {0}='{1}' where surah_id = {2} and ayah_id= {3}",LangName,Content,Surah,Ayah);
                        FungsiDB.ExecuteNonQuery(Qry);
                    }
                    else
                    {
                        //insert
                   
                        string Qry = string.Format("insert into quran (surah_id,ayah_id,{0}) values ({1},{2},'{3}')", LangName, Surah, Ayah,Content);
                        FungsiDB.ExecuteNonQuery(Qry);
                    }

                }
                connection.Close();*/
            }
            catch
            {
                throw;
            }
            
        
        }
        #endregion

        #region Surah
        public static string getSurahName(int Index)
        {
            string Data = string.Empty;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.surahs
                            where a.idx == Index
                            select a).Single().name;

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<surah> getSurahNames()
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.surahs
                            select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static surah getSurah(int idx)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.surahs
                            where a.idx==idx
                            select a).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        /*
        public static int getSurahCount(int Surah)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    var SelAyah = (from a in context.quran_texts
                                   where a.sura == Surah
                                   select a).Count();
                    return SelAyah;
                }
            }
            catch
            {
                return -1;
            }

        }
        */
        public static int getSurahCount(int Surah)
        {
            int SelAyah = -1;
            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    SelAyah = (from a in context.surahs
                                   where a.idx == Surah
                                   select a.totalayah).SingleOrDefault();
                   
                }
                connection.Close();
            }
            catch
            {
                return -1;
            }
            return SelAyah;
        }

        public static void UpdateSurah(List<surah> data)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                foreach (var item in data)
                {
                    string sql = string.Format("update surah set place='{1}',totalayah={2},latin='{3}' where idx = {0}", item.idx, item.place.Replace("'", "''"), item.totalayah, item.latin.Replace("'", "''"));
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }
            catch
            {
                throw;
            }
        }

        public static void InsertSurah(int No, string Name)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                string sql = string.Format("insert into surah (idx, name) values ({0},'{1}')", No, Name);
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Reciter
        public static IList<reciter> getReciters()
        {
            List<reciter> sel = new List<reciter>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    sel = (from a in context.reciters
                           orderby a.name ascending
                           select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return sel;
        }

        public static reciter getReciter(int idx)
        {
            reciter sel = new reciter();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    sel = (from a in context.reciters
                           where a.idx == idx
                           select a).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return sel;
        }

        public static void InsertReciter(List<reciter> items)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                foreach (var item in items)
                {
                    string sql = string.Format("insert into reciter (idx, name,mediaurl) values ({0},'{1}','{2}')", item.idx, item.name, item.mediaurl);
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Hizb
        public static List<hizb> getHizb()
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.hizbs
                            select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static hizb getHizb(int Surah, int Ayah)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.hizbs
                            where Surah >= a.surahfrom && Surah <= a.surahto && Ayah >= a.ayahfrom && Ayah <= a.ayahto
                            select a).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<surah> getSurahFromHizb(int Hizb)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);

                using (var context = new DataContext())
                {
                    hizb selHizb = context.hizbs.Where(a => a.idx == Hizb).SingleOrDefault();
                    Data = (from a in context.surahs
                            where a.idx >= selHizb.surahfrom && a.idx <= selHizb.surahto
                            select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        #endregion

        #region Juz
        public static IList<JuzData> getJuzNames()
        {
            var Data = new List<JuzData>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from c in context.juzs
                            select new BLL.quran_data.JuzData { no = BLL.quran_data.AyahInArabic[c.idx], arabic = c.arabic, name = c.name, idx = c.idx }).ToList();
                    Data.Insert(0, new JuzData() { idx = 0, name = "All", arabic="-" });

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<juz> getJuz()
        {
            var Data = new List<juz>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from a in context.juzs
                            select a).ToList();
                    Data.Insert(0, new juz() { idx = 0, name="All", surahfrom = 0, surahto = 114, ayahfrom = 1, ayahto = 6 });
    
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static juz getJuz(int Juz)
        {
            juz Data = null;

            if (Juz == 0)
            {
                return new juz() { idx = 0, name = "All", ayahfrom = 1, surahfrom = 0, surahto = 114, ayahto = 6, arabic = "" };
            }

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from a in context.juzs
                            where a.idx == Juz
                            select a).SingleOrDefault();
                    

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static juz getJuz(int Surah, int Ayah)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.juzs
                            where Surah >= a.surahfrom && Surah <= a.surahto && Ayah >= a.ayahfrom && Ayah <= a.ayahto
                            select a).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<surah> getSurahFromJuz(int Juz)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);

                using (var context = new DataContext())
                {
                    juz selJuz = context.juzs.Where(a => a.idx == Juz).SingleOrDefault();
                    Data = (from a in context.surahs
                            where a.idx >= selJuz.surahfrom && a.idx <= selJuz.surahto
                            select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        #endregion

        #region Manzil

        public static List<manzil> getManzil()
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.manzils
                            select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static manzil getManzil(int Surah, int Ayah)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.manzils
                            where Surah >= a.surahfrom && Surah <= a.surahto 
                            select a).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<surah> getSurahFromManzil(int Manzil)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);

                using (var context = new DataContext())
                {
                    manzil selManzil = context.manzils.Where(a => a.idx == Manzil).SingleOrDefault();
                    Data = (from a in context.surahs
                            where a.idx >= selManzil.surahfrom && a.idx <= selManzil.surahto
                            select a).ToList();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
    
        #endregion

        #region Language
        public static IList<language> getLanguage()
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from a in context.languages
                            orderby a.lang
                            select a).ToList();
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        public static language getLanguage(int langid)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.languages
                            where a.langid==langid
                            select a).SingleOrDefault();

                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        #endregion

        #region bookmark
        public static void DeleteBookmark(bookmarkext item)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                string sql = string.Format("delete from bookmark where idx = {0}", item.idx);
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        public static void InsertBookmark(bookmarkext item)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                string sql = string.Format("insert into bookmark (title, juz,surah,ayah) values ('{0}',{1},{2},{3})", item.title,item.juz,item.surah,item.ayah);
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        public static IList<bookmarkext> getBookmark()
        {
            dynamic Data;

            try
            {
                int Counter = 1;
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    var Temp = from a in context.bookmarks
                            orderby a.idx ascending
                            select new bookmarkext { ayah=a.ayah, idx=a.idx, juz=a.juz, surah=a.surah, title=a.title  };
                    var data = new List<bookmarkext>();
                    foreach (var item in Temp)
                    {
                        item.Counter = Counter++;
                        item.SurahArabic = AyahInArabic[item.surah];
                        item.AyahArabic = AyahInArabic[item.ayah];
                        item.SurahName = getSurah(item.surah).latin;
                        data.Add(item);
                    }
                    Data = data;
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        #endregion

        #region Transliteration
        public static void InsertTransliteration(int surahidx, int ayahidx, int langid, string content)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                string sql = string.Format("insert into transliteration (langid,surahidx,ayahidx,content) values ({0},{1},{2},'{3}')", langid, surahidx, ayahidx, content);
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch
            {
                throw;
            }
        }

        public static List<transliteration> getTransliteration()
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from a in context.transliterations
                            select a).ToList();
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static transliteration getTransliteration(int langid)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from a in context.transliterations
                            where a.langid == langid
                            select a).SingleOrDefault();
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<transliteration> getTransliteration(int langid,int SurahId)
        {
            dynamic Data;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    Data = (from a in context.transliterations
                            where a.langid == langid && a.surahidx == SurahId
                            select a).ToList();
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static Dictionary<int, string> getTransliterationDictionary(int langid, int SurahId)
        {
            Dictionary<int, string> Data = new Dictionary<int, string>();

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    var temp = (from a in context.transliterations
                                where a.langid == langid && a.surahidx == SurahId
                                select a);
                    foreach (var item in temp)
                    {
                        Data.Add(item.ayahidx, item.content);
                    }
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }
        #endregion

        #region Translation
        public static List<TranslationData> getTranslation(int LangId,int SurahId){
            List<TranslationData> Data = new List<TranslationData> ();

            try
            {
                var lng = getLanguage(LangId);
                if (lng != null)
                {
                    FungsiDB.KoneksiStr = Conn;
                    string Qry = string.Format("select surah_id, ayah_id, {0} from quran where surah_id = {1}",lng.lang,SurahId);
                    System.Data.DataTable dt = FungsiDB.RetrieveData(Qry);
                    //dt.TableName = "data";
                    if (dt != null)
                    {
                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            TranslationData item = new TranslationData() { ayah_id = Convert.ToInt32(dr["ayah_id"]), surah_id=Convert.ToInt32(dr["surah_id"]), content=dr[lng.lang].ToString()};
                            Data.Add(item);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static Dictionary<int,string> getTranslationDictionary(int LangId, int SurahId)
        {
            Dictionary<int,string> Data = new Dictionary<int,string>();

            try
            {
                var lng = getLanguage(LangId);
                if (lng != null)
                {
                    FungsiDB.KoneksiStr = Conn;
                    string Qry = string.Format("select surah_id, ayah_id, {0} from quran where surah_id = {1}", lng.lang, SurahId);
                    System.Data.DataTable dt = FungsiDB.RetrieveData(Qry);
                    //dt.TableName = "data";
                    if (dt != null)
                    {
                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            TranslationData item = new TranslationData() { ayah_id = Convert.ToInt32(dr["ayah_id"]), surah_id = Convert.ToInt32(dr["surah_id"]), content = dr[lng.lang].ToString() };
                            Data.Add(item.ayah_id,item.content);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static List<quran> getQuran()
        {
            List<quran> Data = null;

            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {

                    Data = (from a in context.qurans
                            select a).ToList();
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
            return Data;
        }

        public static void UpdateQuranAyah(List<quran> data)
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                connection.Open();
                foreach (var item in data)
                {
                    string sql = string.Format("update quran set ayah_id='{0}' where idx = {1}", item.ayah_id, item.idx);
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Properties
        public class bookmarkext : bookmark
        {
            public string SurahName { set; get; }
            public string SurahArabic { set; get; }
            public string AyahArabic { set; get; }
            public int Counter { set; get; }
        }

        public struct SurahData
        {
            public int idx { set; get; }
            public string name { set; get; }
            public string arabic { set; get; }
        }

        public class AyahData:INotifyPropertyChanged
        {
            public int idx { set; get; }
            public string content { set; get; }
            public string ayah { set; get; }

            private int versesize;
            public int VerseSize { set { versesize = value; NotifyPropertyChanged(); } get { return versesize; } }
           
            private string _translation { set; get; }
            public string translation { set { _translation = value; NotifyPropertyChanged(); } get { return _translation; } }
            
            public string transliteration { set; get; }

            public event PropertyChangedEventHandler PropertyChanged;

            // This method is called by the Set accessor of each property. 
            // The CallerMemberName attribute that is applied to the optional propertyName 
            // parameter causes the property name of the caller to be substituted as an argument. 
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public struct JuzData
        {
            public int idx { set; get; }
            public string no { set; get; }
            public string name { set; get; }
            public string arabic { set; get; }

        }

        public struct TranslationData
        {
            public int surah_id { set; get; }
            public int  ayah_id { set; get; }
            public string content { set; get; }
        }
        #endregion

        #region benchmark
        public static void ReadQuranWithLinq()
        {
            try
            {
                var connection = new SQLiteConnection(Conn);
                using (var context = new DataContext())
                {
                    var Data = (from a in context.quran_texts
                                where a.sura==2
                            select a).ToList();
                }
                connection.Close();
            }
            catch
            {
                throw;
            }
        }

        public static void ReadQuranWithFungsiDB()
        {
            List<quran_text> data = new List<quran_text>();
            try
            {
                FungsiDB.KoneksiStr = Conn;
                string Qry = string.Format("select * from quran_text where sura=2");
                System.Data.DataTable dt = FungsiDB.RetrieveData(Qry);
                dt.TableName = "data";
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    data.Add(new quran_text() { aya = Convert.ToInt32(dr["aya"]), sura = Convert.ToInt32(dr["sura"]), index = Convert.ToInt32(dr["index"]), text = dr["text"].ToString() });
                }

            }
            catch
            {
                throw;
            }
        }
        #endregion
    }

    public static class MyExtensions
    {
        #region Extension
        public static ObservableCollection<T> ToObservableCollection<T>
    (this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ObservableCollection<T>(source);
        }
        #endregion
    }
}
