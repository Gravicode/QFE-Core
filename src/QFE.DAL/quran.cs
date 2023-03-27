using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "quran")]
    public class quran {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "ayah_id")]
        public int ayah_id { get; set; }

        [Column(name: "surah_id")]
        public int? surah_id { get; set; }

        [Column(name: "ayah_location")]
        public string ayah_location { get; set; }

        [Column(name: "Arabic")]
        public string Arabic { get; set; }

        [Column(name: "Albanian")]
        public string Albanian { get; set; }

        [Column(name: "Azerbaijani")]
        public string Azerbaijani { get; set; }

        [Column(name: "Bangali")]
        public string Bangali { get; set; }

        [Column(name: "Bosnian")]
        public string Bosnian { get; set; }

        [Column(name: "Bulgarian")]
        public string Bulgarian { get; set; }

        [Column(name: "Chinese")]
        public string Chinese { get; set; }

        [Column(name: "Czech")]
        public string Czech { get; set; }

        [Column(name: "Divehi")]
        public string Divehi { get; set; }

        [Column(name: "Dutch")]
        public string Dutch { get; set; }

        [Column(name: "English")]
        public string English { get; set; }

        [Column(name: "German")]
        public string German { get; set; }

        [Column(name: "Hausa")]
        public string Hausa { get; set; }

        [Column(name: "Hindi")]
        public string Hindi { get; set; }

        [Column(name: "Indonesian")]
        public string Indonesian { get; set; }

        [Column(name: "Italian")]
        public string Italian { get; set; }

        [Column(name: "Japanese")]
        public string Japanese { get; set; }

        [Column(name: "Korean")]
        public string Korean { get; set; }

        [Column(name: "Kurdish")]
        public string Kurdish { get; set; }

        [Column(name: "Malay")]
        public string Malay { get; set; }

        [Column(name: "Malayalam")]
        public string Malayalam { get; set; }

        [Column(name: "Norwegian")]
        public string Norwegian { get; set; }

        [Column(name: "Persian")]
        public string Persian { get; set; }

        [Column(name: "Polish")]
        public string Polish { get; set; }

        [Column(name: "Portuguese")]
        public string Portuguese { get; set; }

        [Column(name: "Romanian")]
        public string Romanian { get; set; }

        [Column(name: "Russian")]
        public string Russian { get; set; }

        [Column(name: "Sindhi")]
        public string Sindhi { get; set; }

        [Column(name: "Somali")]
        public string Somali { get; set; }

        [Column(name: "Spanish")]
        public string Spanish { get; set; }

        [Column(name: "Swahili")]
        public string Swahili { get; set; }

        [Column(name: "Swedish")]
        public string Swedish { get; set; }

        [Column(name: "Tajik")]
        public string Tajik { get; set; }

        [Column(name: "Tamil")]
        public string Tamil { get; set; }

        [Column(name: "Tatar")]
        public string Tatar { get; set; }

        [Column(name: "Thai")]
        public string Thai { get; set; }

        [Column(name: "Turkish")]
        public string Turkish { get; set; }

        [Column(name: "Urdu")]
        public string Urdu { get; set; }

        [Column(name: "Uzbek")]
        public string Uzbek { get; set; }
    }
}
